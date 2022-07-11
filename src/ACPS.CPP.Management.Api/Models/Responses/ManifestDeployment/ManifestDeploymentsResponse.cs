using Newtonsoft.Json;
using System.Collections.Generic;

namespace VOYG.CPP.Management.Api.Models.Responses.ManifestDeployment
{
    public class ManifestDeploymentsResponse
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("next")]
        public string? Next { get; set; }

        [JsonProperty("previous")]
        public string? Previous { get; set; }

        [JsonProperty("results")]
        public IEnumerable<ManifestDeploymentResponse> ManifestDeploymentResponses { get; set; }
    }
}
