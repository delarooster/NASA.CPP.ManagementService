namespace VOYG.CPP.Management.Api.Models
{
    public class BlobFile
    {
        public BlobFile(string fileName, string blobFileSasUrl, string blobFileMd5Hash, long contentLength)
        {
            FileName = fileName;
            BlobFileSasUrl = blobFileSasUrl;
            BlobFileMd5Hash = blobFileMd5Hash;
            ContentLength = contentLength;
        }

        public string FileName { get; set; }

        public string BlobFileSasUrl { get; set; }

        public string BlobFileMd5Hash { get; set; }

        public long ContentLength { get; set; }
    }
}
