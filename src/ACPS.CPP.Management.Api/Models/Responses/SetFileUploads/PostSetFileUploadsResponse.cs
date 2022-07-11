using Newtonsoft.Json;

namespace VOYG.CPP.Management.Api.Models.Responses.SetFileUploads
{
    public class PostSetFileUploadsResponse
    {
        public PostSetFileUploadsResponse(string deviceId)
        {
            DeviceId = deviceId;
        }

        [JsonProperty("device")]
        public string DeviceId { get; set; }
    }
}
