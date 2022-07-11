using Newtonsoft.Json;
using System.Collections.Generic;

namespace VOYG.CPP.Management.Api.Models.Responses.Deployment
{
    public class DeploymentsResponse
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("previous")]
        public string Previous { get; set; }

        [JsonProperty("results")]
        public IEnumerable<DeploymentResponse> Results { get; set; }
    }
}