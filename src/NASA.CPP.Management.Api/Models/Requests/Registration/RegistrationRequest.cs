using Newtonsoft.Json;
using System.Collections.Generic;

namespace VOYG.CPP.Management.Api.Models.Requests.Registration
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RegistrationRequest
    {
        [JsonProperty("operator")]
        public string Operator { get; set; } = default!;

        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; } = default!;

        [JsonProperty("tpmKey")]
        public string TpmEndorsementKey { get; set; } = default!;

        [JsonProperty("imei")]
        public string Imei { get; set; } = default!;

        [JsonProperty("partNumber")]
        public string PartNumber { get; set; } = default!;

        [JsonProperty("desciption")]
        public string Desciption { get; set; } = default!;

        [JsonProperty("imsi")]
        public string Imsi { get; set; } = default!;

        [JsonProperty("ccid")]
        public string Ccid { get; set; } = default!;

        [JsonProperty("ppp_ip")]
        public string Ppp_Ip { get; set; } = default!;

        [JsonProperty("ioTHubDeviceID")]
        public string? IoTHubDeviceId { get; set; }

        [JsonProperty("secrets")]
        public Dictionary<string, string>? Secrets { get; set; }
    }
}
