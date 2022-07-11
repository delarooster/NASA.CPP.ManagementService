using Newtonsoft.Json;

namespace VOYG.CPP.Management.Api.Models.DeviceTwin.Reported
{
    public class ManifestDeployment
    {
        [JsonProperty("manifestDeploymentId")]
        public int ManifestDeploymentId { get; set; }

        [JsonProperty("manifest")]
        public Manifest Manifest { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
