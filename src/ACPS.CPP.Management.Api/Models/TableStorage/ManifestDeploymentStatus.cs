using Azure;
using Azure.Data.Tables;
using System;

namespace VOYG.CPP.Management.Api.Models.TableStorage
{
    public class ManifestDeploymentStatus : ITableEntity
    {
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public int ManifestDeploymentId { get; set; }

        public string DeviceId { get; set; }

        public DateTime CreatedUtc { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }
    }
}
