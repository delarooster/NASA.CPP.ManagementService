using Newtonsoft.Json;
using System.Collections.Generic;

namespace VOYG.CPP.Management.Api.Models.Responses.Status
{
    public class StatusesResponse
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("previous")]
        public string Previous { get; set; }

        [JsonProperty("results")]
        public IEnumerable<StatusResponse> Results { get; set; }
    }
}
