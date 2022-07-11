using Newtonsoft.Json;

namespace VOYG.CPP.Management.Api.Models.Requests.Deployment
{
    public class UpdateStatusRequest
    {
        [JsonProperty("status")]
        public string Status { get; set; } = "INACTIVE";
    }
}
