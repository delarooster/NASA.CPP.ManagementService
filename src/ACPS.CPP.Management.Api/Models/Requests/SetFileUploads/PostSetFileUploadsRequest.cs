using Newtonsoft.Json;

namespace VOYG.CPP.Management.Api.Models.Requests.SetFileUploads
{
    public class PostSetFileUploadsRequest
    {
        [JsonProperty("device")]
        [JsonRequired]
        public string DeviceId { get; set; }
    }
}
