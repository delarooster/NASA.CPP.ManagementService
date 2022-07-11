using VOYG.CPP.Management.Api.Config.Options;
using VOYG.CPP.Management.Api.Helpers;
using VOYG.CPP.Management.Api.Models;
using VOYG.CPP.Management.Api.Models.DeviceTwin;
using VOYG.CPP.Management.Api.Models.Manifest;
using VOYG.CPP.Management.Api.Models.Requests.ManifestDeployment;
using VOYG.CPP.Management.Api.Models.Responses.ManifestDeployment;
using VOYG.CPP.Management.Api.Models.TableStorage;
using VOYG.CPP.Management.Api.Services.Interfaces;
using VOYG.CPP.Models;
using VOYG.CPP.Models.Entities;
using VOYG.CPP.Models.Entities.Enums;
using AutoMapper;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PolylineEncoder.Net.Models;
using PolylineEncoder.Net.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Services
{
    public class ManifestDeploymentService : IManifestDeploymentService
    {
        private const string PendingStatus = "pending";
        private readonly Func<IUnitOfWork> _unitOfWorkFactory;
        private readonly RegistryManager _registryManager;
        private readonly IMapper _mapper;
        private readonly IBlobServiceProvider _blobServiceProvider;
        private readonly ContainerNamesOptions _containersOptions;
        private readonly IUrlService _urlService;
        private readonly TableServiceClient _tableServiceClient;
        private readonly StorageOptions _storageOptions;
        private readonly PolylineUtility _polylineUtility;

        public ManifestDeploymentService(
            Func<IUnitOfWork> unitOfWorkFactory,
            RegistryManager registryManager,
            IMapper mapper,
            IBlobServiceProvider blobServiceProvider,
            IOptions<ContainerNamesOptions> containersOptions,
            IUrlService urlService,
            TableServiceClient tableServiceClient,
            IOptions<StorageOptions> storageOptions,
            PolylineUtility polylineUtility)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _registryManager = registryManager;
            _mapper = mapper;
            _blobServiceProvider = blobServiceProvider;
            _containersOptions = containersOptions.Value;
            _urlService = urlService;
            _tableServiceClient = tableServiceClient;
            _storageOptions = storageOptions.Value;
            _polylineUtility = polylineUtility;
        }

        public async Task<IServiceResult<ManifestDeploymentResponse>> RegisterManifestDeployment(
            PostRegisterManifestDeploymentRequest postRegisterManifestDeploymentRequest,
            CancellationToken cancellationToken)
        {
            using var unitOfWork = _unitOfWorkFactory();

            var manifest = await unitOfWork.ManifestRepository.SingleOrDefaultAsync(x => x.Id == postRegisterManifestDeploymentRequest.ManifestId);

            if (manifest == null)
            {
                return ResponseHelper.UnsuccessfulResult<ManifestDeploymentResponse>(
                    new Dictionary<string, string>() { { "detail", $"Manifest does not exist for {{'id': '{postRegisterManifestDeploymentRequest.ManifestId}'}}" } },
                    StatusCodes.Status404NotFound);
            }
            if (postRegisterManifestDeploymentRequest.Strategy.ToLower() != Enum.GetName(typeof(ManifestDeploymentStrategy), ManifestDeploymentStrategy.Set)?.ToLower())
            {
                return ResponseHelper.UnsuccessfulResult<ManifestDeploymentResponse>(
                    new Dictionary<string, string>() { { "detail", "Manifest Deployment Strategy must be equal to 'set'." } },
                    StatusCodes.Status400BadRequest);
            }

            var device = await _registryManager.GetDeviceAsync(postRegisterManifestDeploymentRequest.DeviceId);
            if (device == null)
            {
                return ResponseHelper.UnsuccessfulResult<ManifestDeploymentResponse>(
                    new Dictionary<string, string>() { { "detail", $"Device does not exist for {{'deviceId': '{postRegisterManifestDeploymentRequest.DeviceId}'}}" } },
                    StatusCodes.Status404NotFound);
            }

            var blobContent = await _blobServiceProvider.DownloadBlobContentAsString(_containersOptions.Manifests, $"{postRegisterManifestDeploymentRequest.ManifestId}.json", cancellationToken);
            if (string.IsNullOrEmpty(blobContent))
            {
                return ResponseHelper.UnsuccessfulResult<ManifestDeploymentResponse>(
                    new Dictionary<string, string>() { { "detail", "Could not download blob content." } },
                    StatusCodes.Status404NotFound);
            }

            var parsedManifest = ParseManifest(postRegisterManifestDeploymentRequest, blobContent);
            if (parsedManifest == null)
            {
                return ResponseHelper.UnsuccessfulResult<ManifestDeploymentResponse>(new Dictionary<string, string>() { { "detail", "Could not parse manifest." } },
                    StatusCodes.Status500InternalServerError);
            }

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            await _blobServiceProvider.UploadBlob(_containersOptions.ParsedManifests, $"{postRegisterManifestDeploymentRequest.ManifestId}.json",
                parsedManifest, jsonSerializerSettings, cancellationToken);

            var manifestDeployment = await AddManifestDeployment(postRegisterManifestDeploymentRequest, unitOfWork);
            await UpdateDeviceTwinDesiredProperties(manifestDeployment, cancellationToken);

            return ResponseHelper.SuccessfulResult(_mapper.Map<ManifestDeploymentResponse>(manifestDeployment, options => options.AfterMap((src, dest) =>
            {
                dest.Status = PendingStatus;
                dest.Strategy = postRegisterManifestDeploymentRequest.Strategy.ToLower();
            })), StatusCodes.Status201Created);

            #region Local functions

            static async Task<ManifestDeployment> AddManifestDeployment(PostRegisterManifestDeploymentRequest postRegisterManifestDeploymentRequest, IUnitOfWork unitOfWork)
            {
                var manifestDeployment = await unitOfWork.ManifestDeploymentRepository.AddAsync(new ManifestDeployment
                {
                    Strategy = ManifestDeploymentStrategy.Set,
                    DeviceId = postRegisterManifestDeploymentRequest.DeviceId,
                    Tag = postRegisterManifestDeploymentRequest.Tag,
                    ManifestId = postRegisterManifestDeploymentRequest.ManifestId
                });

                await unitOfWork.SaveChangesAsync();

                return manifestDeployment;
            }

            async Task UpdateDeviceTwinDesiredProperties(ManifestDeployment manifestDeployment, CancellationToken cancellationToken)
            {
                var blobFile = await _blobServiceProvider.GenerateBlobFileUrlAndHash(_containersOptions.ParsedManifests, $"{manifestDeployment.ManifestId}.json", cancellationToken);

                var deviceTwinPatch = new DeviceTwin
                {
                    Properties = new Models.DeviceTwin.Properties
                    {
                        Desired = new DesiredProperties
                        {
                            ManifestDeployment = new Models.DeviceTwin.Desired.ManifestDeployment
                            {
                                ManifestDeploymentId = manifestDeployment.Id,
                                Manifest = _mapper.Map<Models.DeviceTwin.Desired.Manifest>(blobFile)
                            }
                        }
                    }
                };

                var deviceTwin = await _registryManager.GetTwinAsync(manifestDeployment.DeviceId, cancellationToken);

                await _registryManager.UpdateTwinAsync(deviceTwin.DeviceId, JsonConvert.SerializeObject(deviceTwinPatch), deviceTwin.ETag, cancellationToken);
            }

            #endregion Local functions
        }

        public async Task<IServiceResult<ManifestDeploymentsResponse>> GetManifestDeployments(int limit, int offset, string? deviceId, CancellationToken cancellationToken)
        {
            using var unitOfWork = _unitOfWorkFactory();

            var manifestDeployments = string.IsNullOrWhiteSpace(deviceId) ?
                await unitOfWork.ManifestDeploymentRepository.GetAllPagedLastAsync(limit, offset, x => x.Id) :
                await unitOfWork.ManifestDeploymentRepository.GetPagedLastAsync(x => x.DeviceId == deviceId, limit, offset, x => x.Id);

            if (!manifestDeployments.Any())
            {
                return ResponseHelper.SuccessfulResult(new ManifestDeploymentsResponse
                {
                    Count = 0,
                    Next = null,
                    Previous = null,
                    ManifestDeploymentResponses = Enumerable.Empty<ManifestDeploymentResponse>()
                });
            }

            var next = _urlService.GenerateUrl("GetManifestDeployments", "ManifestDeployment", new { limit, offset = limit + offset, deviceId });
            var previous = _urlService.GenerateUrl("GetManifestDeployments", "ManifestDeployment", new { limit, offset = offset - limit < 0 ? 0 : offset - limit, deviceId });

            var manifestDeploymentsStatuses = await GetManifestDeploymentStatuses(BuildQueryString(), cancellationToken);
            var manifestDeploymentsStatusesDictionary = manifestDeploymentsStatuses.GroupBy(x => x.ManifestDeploymentId).ToDictionary(x => x.Key, y => y.OrderByDescending(x => x.CreatedUtc).FirstOrDefault());

            return ResponseHelper.SuccessfulResult(new ManifestDeploymentsResponse
            {
                Count = manifestDeployments.Count,
                Next = next,
                Previous = previous,
                ManifestDeploymentResponses = manifestDeployments.Select(x => _mapper.Map<ManifestDeploymentResponse>(x, options => options.AfterMap((src, dest) =>
                 {
                     if (manifestDeploymentsStatusesDictionary.TryGetValue(dest.Id, out var status))
                     {
                         dest.Status = status?.Status ?? PendingStatus;
                         dest.Message = status?.Message ?? PendingStatus;
                     }
                     else
                     {
                         dest.Status = PendingStatus;
                         dest.Message = PendingStatus;
                     }
                 })))
            });

            #region Local functions

            string BuildQueryString()
            {
                var queryParts = new List<string>();

                var minManifestDeploymentId = manifestDeployments.Min(x => x.Id);
                var maxManifestDeploymentId = manifestDeployments.Max(x => x.Id);

                queryParts.Add($"PartitionKey ge '{minManifestDeploymentId}'");
                queryParts.Add($"PartitionKey le '{maxManifestDeploymentId}'");

                return string.Join(" and ", queryParts);
            }

            #endregion
        }

        public async Task<IServiceResult<ManifestDeploymentResponse>> GetManifestDeployment(int id, CancellationToken cancellationToken)
        {
            using var unitOfWork = _unitOfWorkFactory();

            var manifestDeployment = await unitOfWork.ManifestDeploymentRepository.SingleOrDefaultAsync(x => x.Id == id);

            if (manifestDeployment == null)
            {
                return ResponseHelper.UnsuccessfulResult<ManifestDeploymentResponse>(new Dictionary<string, string>() { { "detail", "Not found." } }, StatusCodes.Status404NotFound);
            }

            var manifestDeploymentStatus = (await GetManifestDeploymentStatuses(BuildQueryString(), cancellationToken)).OrderByDescending(x => x.CreatedUtc).FirstOrDefault();

            return ResponseHelper.SuccessfulResult(_mapper.Map<ManifestDeploymentResponse>(manifestDeployment, options => options.AfterMap((src, dest) =>
            {
                dest.Status = manifestDeploymentStatus?.Status ?? PendingStatus;
                dest.Message = manifestDeploymentStatus?.Message ?? PendingStatus;
            })), StatusCodes.Status200OK);

            #region Local functions

            string BuildQueryString()
            {
                return $"PartitionKey eq '{manifestDeployment?.Id}'";
            }

            #endregion
        }

        private async Task<List<ManifestDeploymentStatus>> GetManifestDeploymentStatuses(string queryString, CancellationToken cancellationToken)
        {
            var tableClient = _tableServiceClient.GetTableClient(_storageOptions.D2CManifestDeploymentStatusTableName);
            var query = tableClient.QueryAsync<ManifestDeploymentStatus>(
                queryString,
                cancellationToken: cancellationToken);
            return await query.ToListAsync(cancellationToken);
        }

        private ParsedManifestContent? ParseManifest(PostRegisterManifestDeploymentRequest postRegisterManifestDeploymentRequest, string blobContent)
        {
            const string SetFileItemType = "Item.SetFile";
            const string GeofencingItemType = "Item.Geofencing";
            const string GeofencingAdvancedItemType = "Item.GeofencingAdvancedSettings";
            const string GeofencingZoneItemType = "Item.GeofencingZone";
            const string GeofencingTokenItemType = "Item.GeofencingToken";

            blobContent = blobContent.Replace("\\r", "").Replace("\\n", "").Replace("\\\"", "\"").Replace("\"{", "{").Replace("}\"", "}").Replace("\"[{", "[{").Replace("}]\"", "}]");
            var manifestContentObject = JsonConvert.DeserializeObject<ManifestContent>(blobContent);
            if (manifestContentObject == null || manifestContentObject.Data == null)
            {
                return null;
            }

            var setFileItemContent = manifestContentObject.Data.Items.First().Children.Single(x => x.ItemType == SetFileItemType).SetFileContent;

            var parsedSetFileContent = new ParsedManifestContent(setFileItemContent);

            if (!string.IsNullOrEmpty(parsedSetFileContent.Globals.tag.Value))
            {
                parsedSetFileContent.Globals.tag.Value = postRegisterManifestDeploymentRequest.Tag;
            }

            var geoFencingItem = manifestContentObject.Data.Items.First().Children.SingleOrDefault(x => x.ItemType == GeofencingItemType);
            if (geoFencingItem != null)
            {
                AddGeoTag(parsedSetFileContent, geoFencingItem);
            }

            return parsedSetFileContent;

            #region Local Functions

            void AddGeoTag(ParsedManifestContent parsedSetFileContent, Child? geoFencingItem)
            {
                var geofencingAdvanced = geoFencingItem.Children.SingleOrDefault(x => x.ItemType == GeofencingAdvancedItemType);
                var geofencingZones = geoFencingItem.Children.Where(x => x.ItemType == GeofencingZoneItemType);

                var canLink = geofencingAdvanced != null ? geofencingAdvanced.Canlink.ToString() : "1";
                var features = GetFeatures(geofencingZones);

                parsedSetFileContent.Geo = new Geo
                {
                    Enable = geoFencingItem.Enabled,
                    Name = "Geofence",
                    Modes = new[]
                    {
                        new Mode
                        {
                            Name = "Default",
                            Default = true,
                            Output = new Output
                            {
                                Link = canLink,
                                Msg = "18FF031C#00.FF.FF.FF.FF.5F.FF.FF",
                                Rate = "1"
                            }
                        },
                        new Mode
                        {
                            Name = "EV Pre-Charge",
                            Default = false,
                            Output = new Output
                            {
                                Link = canLink,
                                Msg = "18FF031C#06.FF.FF.FF.FF.5F.FF.FF",
                                Rate = "1"
                            }
                        },
                        new Mode
                        {
                            Name = "EV",
                            Default = false,
                            Output = new Output
                            {
                                Link = canLink,
                                Msg = "18FF031C#03.FF.FF.FF.FF.5F.FF.FF",
                                Rate = "1"
                            }
                        }
                    },
                    Zones = new Zones
                    {
                        Type = "FeatureCollection",
                        Features = features.ToArray()
                    }
                };

                IEnumerable<Feature> GetFeatures(IEnumerable<GeofencingChild> geofencingZones)
                {
                    var features = new List<Feature>();
                    foreach (var zone in geofencingZones)
                    {
                        var coords = zone.ZoneCoords.Select(x => new GeoCoordinate(x.Lat, x.Lng));
                        var zoneTokens = zone.Children.Where(x => x.ItemType == GeofencingTokenItemType);

                        var feature = new Feature
                        {
                            Type = "Feature",
                            Geometry = new Geometry
                            {
                                Type = "Polygon",
                                Polylines = new[] { _polylineUtility.Encode(coords) }
                            },
                            Properties = new Models.Manifest.Properties
                            {
                                Description = zone.ZoneName,
                                Zoneid = zone.ItemId,
                                Zonetype = zoneTokens.Any() ? "LinkedZone" : "Zone",
                                Zonedata = new ZoneData
                                {
                                    Mode = zone.ZoneType == "EV" ? "EV" : "EV Pre-Charge"
                                }
                            }
                        };

                        features.Add(feature);

                        foreach (var zoneToken in zoneTokens)
                        {
                            var tokenCoords = zoneToken.TokenCoords.Select(x => new GeoCoordinate(x.Lat, x.Lng));

                            var tokenFeature = new Feature
                            {
                                Type = "Feature",
                                Geometry = new Geometry
                                {
                                    Type = "Polygon",
                                    Polylines = new[] { _polylineUtility.Encode(tokenCoords) }
                                },
                                Properties = new Models.Manifest.Properties
                                {
                                    Description = $"Token for {zone.ZoneName}",
                                    Zoneid = zoneToken.ItemId,
                                    Zonetype = "Token",
                                    Zonedata = new ZoneData
                                    {
                                        Zoneidref = new[] { zone.ItemId }
                                    }
                                }
                            };

                            features.Add(tokenFeature);
                        }
                    }

                    return features;
                }
            }

            #endregion Local Functions
        }
    }
}
