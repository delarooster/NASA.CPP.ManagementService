using Newtonsoft.Json;

namespace VOYG.CPP.Management.Api.Models.Responses.Registration
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RegistrationStatusResponse
    {
        [JsonProperty("device-id")]
        public string DeviceId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
