using Newtonsoft.Json;

namespace VOYG.CPP.Management.Api.Models.DeviceTwin.Desired
{
    public class ManifestDeployment
    {
        [JsonProperty("manifestDeploymentId")]
        public int ManifestDeploymentId { get; set; }

        [JsonProperty("manifest")]
        public Manifest Manifest { get; set; }
    }
}
