using AutoMapper;
using Azure.Data.Tables;
using VOYG.CPP.Management.Api.Config.Options;
using VOYG.CPP.Management.Api.Helpers;
using VOYG.CPP.Management.Api.Models;
using VOYG.CPP.Management.Api.Models.Responses.Status;
using VOYG.CPP.Management.Api.Models.TableStorage;
using VOYG.CPP.Management.Api.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Services
{
    public class StatusService : IStatusService
    {
        private readonly TableServiceClient _tableServiceClient;
        private readonly IMapper _mapper;
        private readonly IUrlService _urlService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly StorageOptions _storageOptions;

        public StatusService(
            TableServiceClient cloudStorageAccount,
            IMapper mapper,
            IUrlService urlService,
            IDateTimeProvider dateTimeProvider,
            IOptions<StorageOptions> storageOptions)
        {
            _tableServiceClient = cloudStorageAccount;
            _mapper = mapper;
            _urlService = urlService;
            _dateTimeProvider = dateTimeProvider;
            _storageOptions = storageOptions.Value;
        }

        public async Task<IServiceResult<StatusesResponse>> GetStatuses(int? limit, int? offset, string? deviceId, string? deploymentId, CancellationToken cancellationToken)
        {
            var tableClient = _tableServiceClient.GetTableClient(_storageOptions.D2CDeploymentStatusTableName);
            var query = tableClient.QueryAsync<DeploymentStatus>(
                BuildQueryString(),
                cancellationToken: cancellationToken);
            var queryResult = await query.Skip(offset ?? 0).Take(limit ?? 0).ToListAsync(cancellationToken);

            var response = new StatusesResponse
            {
                Count = queryResult.Count,
                Next = _urlService.GenerateUrl("GetStatuses", "Status", new { limit, offset = limit + offset, deviceId, deploymentId }),
                Previous = _urlService.GenerateUrl("GetStatuses", "Status", new { limit, offset = offset - limit < 0 ? 0 : offset - limit, deviceId, deploymentId }),
                Results = queryResult.Select(x => _mapper.Map<DeploymentStatus, StatusResponse>(x, options => options.AfterMap((src, dest) =>
                {
                    dest.TimeStamp = GenerateTimestamp(dest.CreatedUtc);
                    dest.DeploymentUrl = _urlService.GenerateUrl("GetDeployment", "Deployment", new { id = src.PartitionKey });
                })))
            };

            return ResponseHelper.SuccessfulResult(response);

            #region Local functions

            string BuildQueryString()
            {
                var queryParts = new List<string>();

                if (deviceId != null)
                {
                    queryParts.Add($"DeviceId eq '{deviceId}'");
                }

                if (deploymentId != null)
                {
                    queryParts.Add($"PartitionKey eq '{deploymentId}'");
                }

                return string.Join(" and ", queryParts);
            }

            double GenerateTimestamp(DateTime createdUtc)
            {
                var epochTicks = new TimeSpan(_dateTimeProvider.UnixEpoch.Ticks);
                var unixTicks = new TimeSpan(createdUtc.Ticks) - epochTicks;
                return unixTicks.TotalSeconds;
            }

            #endregion
        }
    }
}