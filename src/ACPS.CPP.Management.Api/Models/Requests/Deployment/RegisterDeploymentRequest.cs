using Newtonsoft.Json;

namespace VOYG.CPP.Management.Api.Models.Requests.Deployment
{
    public class RegisterDeploymentRequest
    {
        [JsonRequired]
        [JsonProperty("device")]
        public string DeviceId { get; set; }

        [JsonRequired]
        public Package Package { get; set; }
    }

    public class Package
    {
        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public string Version { get; set; }
    }
}
