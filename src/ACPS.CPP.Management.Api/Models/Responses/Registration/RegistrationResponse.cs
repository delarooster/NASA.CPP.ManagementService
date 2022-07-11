using Newtonsoft.Json;

namespace VOYG.CPP.Management.Api.Models.Responses.Registration
{
    [JsonObject(MemberSerialization.OptIn)]

    public class RegistrationResponse
    {
        [JsonProperty("registrationId")]
        public string RegistrationId { get; set; } = default!;

        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } = default!;
    }
}
