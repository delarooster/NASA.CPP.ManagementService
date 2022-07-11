using AutoMapper;
using VOYG.CPP.Management.Api.Config.Options;
using VOYG.CPP.Management.Api.Helpers;
using VOYG.CPP.Management.Api.Models;
using VOYG.CPP.Management.Api.Models.DeviceTwin;
using VOYG.CPP.Management.Api.Models.Requests.Deployment;
using VOYG.CPP.Management.Api.Models.Responses.Deployment;
using VOYG.CPP.Management.Api.Services.Interfaces;
using VOYG.CPP.Models;
using VOYG.CPP.Models.Entities;
using VOYG.CPP.Models.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Services
{
    public class DeploymentService : IDeploymentService
    {
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;
        private readonly RegistryManager _registryManager;
        private readonly IMapper _mapper;
        private readonly IUrlService _urlService;
        private readonly IBlobServiceProvider _blobServiceProvider;
        private readonly ContainerNamesOptions _containersOptions;
        private readonly ApisOptions _apisOptions;

        public DeploymentService(
            Func<IUnitOfWork> unitOfWorkFactory,
            RegistryManager registryManager,
            IMapper mapper,
            IUrlService urlService,
            IBlobServiceProvider blobServiceProvider,
            IOptions<ContainerNamesOptions> containersOptions,
            IOptions<ApisOptions> apisOptions)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _registryManager = registryManager;
            _mapper = mapper;
            _urlService = urlService;
            _blobServiceProvider = blobServiceProvider;
            _containersOptions = containersOptions.Value;
            _apisOptions = apisOptions.Value;
        }

        public async Task<IServiceResult<DeploymentResponse>> RegisterDeployment(RegisterDeploymentRequest registerDeploymentRequest, CancellationToken cancellationToken)
        {
            using var unitOfWork = _unitOfWorkFactory();

            var package = await unitOfWork.PackageRepository.SingleOrDefaultAsync(
                x => x.Name == registerDeploymentRequest.Package.Name && x.Version == registerDeploymentRequest.Package.Version,
                x => x.DownloadFiles,
                x => x.ExecCommands);

            if (package == null)
            {
                return ResponseHelper.UnsuccessfulResult<DeploymentResponse>(
                    new Dictionary<string, string>() { { "non_field_errors", $"Package does not exist for {{'name': '{registerDeploymentRequest.Package.Name}', " +
                                                                             $"'version': '{registerDeploymentRequest.Package.Version}'}}" } },
                    StatusCodes.Status400BadRequest);
            }

            var filesExist = await unitOfWork.DownloadFileRepository.AnyAsync(x => x.PackageId == package.Id);
            var hasNotUploadedFiles = await unitOfWork.DownloadFileRepository.AnyAsync(x => x.PackageId == package.Id && !x.Uploaded);

            if (!filesExist || hasNotUploadedFiles)
            {
                return ResponseHelper.UnsuccessfulResult<DeploymentResponse>(
                    new Dictionary<string, string>() { { "non_field_errors", "Package does not have files that can be downloaded." } },
                    StatusCodes.Status400BadRequest);
            }

            var device = await _registryManager.GetDeviceAsync(registerDeploymentRequest.DeviceId);

            if (device == null)
            {
                return ResponseHelper.UnsuccessfulResult<DeploymentResponse>(
                    new Dictionary<string, string>() { { "non_field_errors", $"Device does not exist for {{'deviceId': '{registerDeploymentRequest.DeviceId}'}}" } },
                    StatusCodes.Status400BadRequest);
            }

            var deployment = await CreateDeploymentAndSetPreviousToInactive(registerDeploymentRequest, unitOfWork, package);
            await UpdateDeviceTwinDesiredProperties(deployment, device, package, cancellationToken);

            return ResponseHelper.SuccessfulResult(
                _mapper.Map<DeploymentResponse>(
                    deployment,
                    options => options.AfterMap((src, dest) =>
                    {
                        dest.Url = _urlService.GenerateUrl("GetDeployment", "Deployment", new { id = dest.Id });
                        dest.Package.Url = GetPackageUrl(deployment.PackageId);
                    })),
                StatusCodes.Status201Created);

            #region Local functions

            static async Task<Deployment> CreateDeploymentAndSetPreviousToInactive(RegisterDeploymentRequest registerDeploymentRequest, IUnitOfWork unitOfWork, VOYG.CPP.Models.Entities.Package package)
            {
                var activeDeployment = await unitOfWork.DeploymentRepository.SingleOrDefaultAsync(x =>
                    x.DeviceId == registerDeploymentRequest.DeviceId &&
                    x.PackageId == package.Id &&
                    x.DeploymentStatus == DeploymentStatus.Active);

                if (activeDeployment != null)
                {
                    activeDeployment.DeploymentStatus = DeploymentStatus.Inactive;
                }

                var deployment = await unitOfWork.DeploymentRepository.AddAsync(new Deployment
                {
                    DeviceId = registerDeploymentRequest.DeviceId,
                    DeploymentStatus = DeploymentStatus.Active,
                    Package = package
                });

                await unitOfWork.SaveChangesAsync();

                return deployment;
            }

            async Task UpdateDeviceTwinDesiredProperties(Deployment deployment, Microsoft.Azure.Devices.Device device, VOYG.CPP.Models.Entities.Package package, CancellationToken cancellationToken)
            {
                var downloadFiles = await GetDownloadFiles(package, cancellationToken);

                var packageDeviceTwinPatch = _mapper.Map<Models.DeviceTwin.Desired.Package>(package, options => options.AfterMap(async (object src, Models.DeviceTwin.Desired.Package dest) =>
                {
                    dest.DownloadFiles = downloadFiles;
                }));

                var deviceTwinPatch = new DeviceTwin
                {
                    Properties = new Properties
                    {
                        Desired = new DesiredProperties
                        {
                            Deployment = new Models.DeviceTwin.Desired.Deployment(deployment.Id, packageDeviceTwinPatch)
                        }
                    }
                };

                var deviceTwin = await _registryManager.GetTwinAsync(device.Id, cancellationToken);

                await _registryManager.UpdateTwinAsync(deviceTwin.DeviceId, JsonConvert.SerializeObject(deviceTwinPatch), deviceTwin.ETag, cancellationToken);

                #region Local functions

                async Task<IEnumerable<Models.DeviceTwin.Desired.DownloadFile>> GetDownloadFiles(VOYG.CPP.Models.Entities.Package package, CancellationToken cancellationToken)
                {
                    var downloadFiles = new List<Models.DeviceTwin.Desired.DownloadFile>();

                    foreach (var downloadFile in package.DownloadFiles)
                    {
                        var blobFile = await _blobServiceProvider.GenerateBlobFileUrlAndHash(
                            _containersOptions.Packages,
                            $"{package.Id}/{downloadFile.FileName}",
                            cancellationToken);

                        downloadFiles.Add(_mapper.Map<Models.DeviceTwin.Desired.DownloadFile>(blobFile));
                    }

                    return downloadFiles;
                }

                #endregion Local functions
            }

            #endregion Local functions
        }

        public async Task<IServiceResult<DeploymentsResponse>> GetDeployments(int? limit, int? offset, IEnumerable<string> deviceIds, CancellationToken cancellationToken)
        {
            using var unitOfWork = _unitOfWorkFactory();

            var deployments =
                await unitOfWork.DeploymentRepository.GetAllPagedLastAsync(limit, offset, x => x.Id, x => x.Package);

            var next = _urlService.GenerateUrl("GetDeployments", "Deployment", new { limit, offset = limit + offset });
            var previous = _urlService.GenerateUrl("GetDeployments", "Deployment", new { limit, offset = offset - limit < 0 ? 0 : offset - limit });

            if (deviceIds != null && deviceIds.Any())
            {
                deployments = deployments.Where(x => deviceIds.Contains(x.DeviceId)).ToList().AsReadOnly();
                next = _urlService.GenerateUrl("GetDeployments", "Deployment", new { limit, offset = limit + offset, device = deviceIds });
                previous = _urlService.GenerateUrl("GetDeployments", "Deployment", new { limit, offset = offset - limit < 0 ? 0 : offset - limit, device = deviceIds });
            }

            var results = deployments.Select(deployment => _mapper.Map<DeploymentResponse>(
                deployment,
                options => options.AfterMap((src, dest) =>
                {
                    dest.Url = _urlService.GenerateUrl("GetDeployment", "Deployment", new { id = dest.Id });
                    dest.Package.Url = GetPackageUrl(deployment.PackageId);
                })));

            return ResponseHelper.SuccessfulResult(new DeploymentsResponse
            {
                Count = deployments.Count,
                Next = next,
                Previous = previous,
                Results = results
            });
        }

        public async Task<IServiceResult<DeploymentResponse>> GetDeployment(int id, CancellationToken cancellationToken)
        {
            using var unitOfWork = _unitOfWorkFactory();

            var deployment = await unitOfWork.DeploymentRepository.SingleOrDefaultAsync(x => x.Id == id, x => x.Package);

            if (deployment == null)
            {
                return ResponseHelper.UnsuccessfulResult<DeploymentResponse>(
                    new Dictionary<string, string>() { { "detail", "Not found." } },
                    StatusCodes.Status404NotFound);
            }

            return ResponseHelper.SuccessfulResult(
                _mapper.Map<DeploymentResponse>(
                    deployment,
                    options => options.AfterMap((src, dest) =>
                    {
                        dest.Url = _urlService.GenerateUrl("GetDeployment", "Deployment", new { id = dest.Id });
                        dest.Package.Url = GetPackageUrl(deployment.PackageId);
                    })),
                StatusCodes.Status200OK);
        }

        public async Task<IServiceResult<UpdateStatusResponse>> UpdateStatus(int id, UpdateStatusRequest updateStatusRequest, CancellationToken cancellationToken)
        {
            using var unitOfWork = _unitOfWorkFactory();

            var deployment = await unitOfWork.DeploymentRepository.SingleOrDefaultAsync(x => x.Id == id, x => x.Package);

            if (deployment == null)
            {
                return ResponseHelper.UnsuccessfulResult<UpdateStatusResponse>(
                    new Dictionary<string, string>() { { "detail", "Not found." } },
                    StatusCodes.Status404NotFound);
            }

            if (!updateStatusRequest.Status.ToLower().Equals(Enum.GetName(typeof(DeploymentStatus), DeploymentStatus.Inactive)?.ToLower()))
            {
                return ResponseHelper.UnsuccessfulResult<UpdateStatusResponse>(
                    new Dictionary<string, string>() { { "detail", "Invalid status." } },
                    StatusCodes.Status400BadRequest);
            }

            var device = await _registryManager.GetDeviceAsync(deployment.DeviceId, cancellationToken);

            if (device == null)
            {
                return ResponseHelper.UnsuccessfulResult<UpdateStatusResponse>(
                    new Dictionary<string, string>() { { "non_field_errors", $"Device does not exist for {{'deviceId': '{deployment.DeviceId}'}}" } },
                    StatusCodes.Status404NotFound);
            }

            var deviceTwin = await _registryManager.GetTwinAsync(device.Id, cancellationToken);
            var deviceTwinObject = JsonConvert.DeserializeObject<DeviceTwin>(deviceTwin.ToJson());

            if (deviceTwinObject == null)
            {
                return ResponseHelper.UnsuccessfulResult<UpdateStatusResponse>(
                    new Dictionary<string, string>() { { "non_field_errors", $"Device twin does not exist for {{'deviceId': '{deployment.DeviceId}'}}" } },
                    StatusCodes.Status404NotFound);
            }

            var response = new UpdateStatusResponse
            {
                Url = _urlService.GenerateUrl("GetDeployment", "Deployment", new { id = deployment.Id }),
                CreatedUtc = deployment.CreatedUtc,
                DeviceId = deployment.DeviceId,
                Status = Enum.GetName(typeof(DeploymentStatus), deployment.DeploymentStatus).ToUpper(),
                PackageUrl = GetPackageUrl(deployment.PackageId)
            };

            if (DeploymentWasNotApplied())
            {
                await UpdateDeviceTwinDesiredProperties();
                await ChangeDeploymentsStatuses();
                response.Status = Enum.GetName(typeof(DeploymentStatus), deployment.DeploymentStatus).ToUpper();
            }

            return ResponseHelper.SuccessfulResult(response, StatusCodes.Status200OK);

            #region Local functions

            bool DeploymentWasNotApplied()
            {
                return deviceTwinObject.Properties.Reported == null ||
                deviceTwinObject.Properties.Desired.Deployment.DeploymentId != deviceTwinObject.Properties.Reported.Deployment.DeploymentId ||
                deviceTwinObject.Properties.Reported.Deployment.Status == null;
            }

            async Task UpdateDeviceTwinDesiredProperties()
            {
                deviceTwinObject.Properties.Desired.Deployment = _mapper.Map<Models.DeviceTwin.Desired.Deployment>(deviceTwinObject.Properties.Reported.Deployment);
                await _registryManager.UpdateTwinAsync(deviceTwin.DeviceId, JsonConvert.SerializeObject(deviceTwinObject), deviceTwin.ETag, cancellationToken);
            }

            async Task ChangeDeploymentsStatuses()
            {
                var previousDeployment = await unitOfWork.DeploymentRepository.SingleOrDefaultAsync(x => x.Id == deviceTwinObject.Properties.Reported.Deployment.DeploymentId);
                if (previousDeployment != null)
                {
                    previousDeployment.DeploymentStatus = DeploymentStatus.Active;
                }

                deployment.DeploymentStatus = DeploymentStatus.Inactive;
                await unitOfWork.SaveChangesAsync();
            }

            #endregion
        }

        private string GetPackageUrl(int packageId)
        {
            return $"{_apisOptions.GlobalStorageApiHost}{_apisOptions.GlobalStorageApiGetPackageEndpoint}".Replace("{id}", $"{packageId}");
        }
    }
}