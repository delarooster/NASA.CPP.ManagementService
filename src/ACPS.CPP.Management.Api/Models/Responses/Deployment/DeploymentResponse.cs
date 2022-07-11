using Newtonsoft.Json;
using System;

namespace VOYG.CPP.Management.Api.Models.Responses.Deployment
{
    public class DeploymentResponse
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("device")]
        public string DeviceId { get; set; }

        [JsonProperty("package")]
        public PackageResponse Package { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class PackageResponse
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}
