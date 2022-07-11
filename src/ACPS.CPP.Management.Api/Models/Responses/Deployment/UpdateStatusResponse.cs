using Newtonsoft.Json;
using System;

namespace VOYG.CPP.Management.Api.Models.Responses.Deployment
{
    public class UpdateStatusResponse
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("created")]
        public DateTime CreatedUtc { get; set; }

        [JsonProperty("device")]
        public string DeviceId { get; set; }

        [JsonProperty("package")]
        public string PackageUrl { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}