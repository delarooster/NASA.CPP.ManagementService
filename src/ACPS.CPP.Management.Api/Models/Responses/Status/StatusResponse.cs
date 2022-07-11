using Newtonsoft.Json;
using System;

namespace VOYG.CPP.Management.Api.Models.Responses.Status
{
    public class StatusResponse
    {
        [JsonProperty("created")]
        public DateTime CreatedUtc { get; set; }

        [JsonProperty("deployment")]
        public string DeploymentUrl { get; set; }

        [JsonProperty("device")]
        public string DeviceId { get; set; }

        [JsonProperty("timestamp")]
        public double TimeStamp { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
