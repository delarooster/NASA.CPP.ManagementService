using Azure;
using Azure.Data.Tables;
using System;

namespace VOYG.CPP.Management.Api.Models.TableStorage
{
    public class DeploymentStatus : ITableEntity
    {
        public DateTimeOffset? Timestamp { get; set; }

        public string DeviceId { get; set; }

        public string Status { get; set; }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public DateTime CreatedUtc { get; set; }

        public ETag ETag { get; set; }
    }
}
