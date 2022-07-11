using VOYG.CPP.Management.Api.Models;
using VOYG.CPP.Management.Api.Models.Requests.SetFileUploads;
using VOYG.CPP.Management.Api.Models.Responses.SetFileUploads;
using VOYG.CPP.Management.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Devices;
using System;
using System.Threading;
using System.Threading.Tasks;
using VOYG.CPP.Management.Api.Helpers;
using System.Collections.Generic;
using VOYG.CPP.Models;

namespace VOYG.CPP.Management.Api.Services
{
    public class SetFileUploadsService : ISetFileUploadsService
    {
        private readonly ServiceClient _serviceClient;
        private readonly RegistryManager _registryManager;
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;

        public SetFileUploadsService(ServiceClient serviceClient, RegistryManager registryManager, Func<IUnitOfWork> unitOfWorkFactory)
        {
            _serviceClient = serviceClient;
            _registryManager = registryManager;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public async Task<IServiceResult<PostSetFileUploadsResponse>> InvokeSetFileUpload(PostSetFileUploadsRequest postSetFileUploadsRequest, CancellationToken cancellationToken)
        {
            const string UploadSetFileMethodName = "SETFILE_UPLOAD";
            var device = await _registryManager.GetDeviceAsync(postSetFileUploadsRequest.DeviceId);

            if (device == null)
            {
                return ResponseHelper.UnsuccessfulResult<PostSetFileUploadsResponse>(
                    new Dictionary<string, string>() { { "detail", $"Device does not exist for {{'deviceId': '{postSetFileUploadsRequest.DeviceId}'}}" } },
                    StatusCodes.Status404NotFound);
            }

            try
            {
                using var unitOfWork = _unitOfWorkFactory();

                await unitOfWork.SetFileRepository.Upsert(postSetFileUploadsRequest.DeviceId);
                await unitOfWork.SaveChangesAsync();

                var methodInvocation = new CloudToDeviceMethod(UploadSetFileMethodName) { ResponseTimeout = TimeSpan.FromSeconds(30) };
                var cloudToDeviceMethodResult = await _serviceClient.InvokeDeviceMethodAsync(postSetFileUploadsRequest.DeviceId, methodInvocation, cancellationToken);

                if (cloudToDeviceMethodResult.Status != StatusCodes.Status200OK)
                {
                    return ResponseHelper.UnsuccessfulResult<PostSetFileUploadsResponse>(new Dictionary<string, string>() { { "detail", cloudToDeviceMethodResult.GetPayloadAsJson() } },
                                StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception ex)
            {
                return ResponseHelper.UnsuccessfulResult<PostSetFileUploadsResponse>(new Dictionary<string, string>() { { "detail", ex.Message } }, StatusCodes.Status500InternalServerError);
            }

            return ResponseHelper.SuccessfulResult(new PostSetFileUploadsResponse(postSetFileUploadsRequest.DeviceId), StatusCodes.Status201Created);
        }
    }
}
