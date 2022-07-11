using Newtonsoft.Json;

namespace VOYG.CPP.Management.Api.Models.DeviceTwin.Desired
{
    public class DownloadFile
    {
        public DownloadFile(string fileName, string blobFileSasUrl, string blobFileMd5Hash, long contentLength)
        {
            FileName = fileName;
            BlobFileSasUrl = blobFileSasUrl;
            BlobFileMd5Hash = blobFileMd5Hash;
            ContentLength = contentLength;
        }

        [JsonProperty("name")]
        public string FileName { get; set; }

        [JsonProperty("blobFileSasUrl")]
        public string BlobFileSasUrl { get; set; }

        [JsonProperty("blobFileMd5Hash")]
        public string BlobFileMd5Hash { get; set; }

        [JsonProperty("contentLength")]
        public long ContentLength { get; set; }
    }
}
