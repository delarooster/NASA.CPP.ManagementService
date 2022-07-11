using Newtonsoft.Json;
using System;

namespace VOYG.CPP.Management.Api.Models.Responses.ManifestDeployment
{
    public class ManifestDeploymentResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("created")]
        public DateTime CreatedUtc { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("strategy")]
        public string Strategy { get; set; }

        [JsonProperty("device")]
        public string DeviceId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("manifest")]
        public string ManifestId { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }
    }
}
