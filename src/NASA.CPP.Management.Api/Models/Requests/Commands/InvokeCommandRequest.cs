using Newtonsoft.Json;

namespace VOYG.CPP.Management.Api.Models.Requests.Commands
{
    public class InvokeCommandRequest
    {
        [JsonProperty("device")]
        [JsonRequired]
        public string DeviceUid { get; set; }

        [JsonProperty("action")]
        [JsonRequired]
        public string Command { get; set; }
    }
}
