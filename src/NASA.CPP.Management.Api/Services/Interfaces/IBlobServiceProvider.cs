using VOYG.CPP.Management.Api.Models;
using Azure;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace VOYG.CPP.Management.Api.Services.Interfaces
{
    public interface IBlobServiceProvider
    {
        Task<string?> DownloadBlobContentAsString(string containerName, string filePath, CancellationToken cancellationToken);
        Task<BlobFile> GenerateBlobFileUrlAndHash(string containerName, string filePath, CancellationToken cancellationToken);
        Task<Response<BlobContentInfo>> UploadBlob(string blobContainerName, string blobName, object objectToSerialize, JsonSerializerSettings jsonSerializerSettings, CancellationToken cancellationToken);
    }
}