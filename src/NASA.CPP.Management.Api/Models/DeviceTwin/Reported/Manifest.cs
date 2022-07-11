using Newtonsoft.Json;

namespace VOYG.CPP.Management.Api.Models.DeviceTwin.Reported
{
    public class Manifest
    {
        [JsonProperty("blobFileSasUrl")]
        public string BlobFileSasUrl { get; set; }

        [JsonProperty("blobFileMd5Hash")]
        public string BlobFileMd5Hash { get; set; }

        [JsonProperty("contentLength")]
        public long ContentLength { get; set; }
    }
}
