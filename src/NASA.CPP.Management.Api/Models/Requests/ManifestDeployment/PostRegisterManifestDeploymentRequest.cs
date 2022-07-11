using Newtonsoft.Json;
using System;

namespace VOYG.CPP.Management.Api.Models.Requests.ManifestDeployment
{
    public class PostRegisterManifestDeploymentRequest
    {
        [JsonRequired]
        [JsonProperty("strategy")]
        public string Strategy { get; set; }

        [JsonRequired]
        [JsonProperty("device")]
        public string DeviceId { get; set; }

        [JsonRequired]
        [JsonProperty("manifest")]
        public Guid ManifestId { get; set; }

        [JsonRequired]
        [JsonProperty("tag")]
        public string Tag { get; set; }
    }
}
