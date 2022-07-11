using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using VOYG.CPP.Management.Api.Models;
using VOYG.CPP.Management.Api.Services.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.IO;
using Newtonsoft.Json;
using Azure.Storage.Blobs.Models;
using Azure;
using System.Linq;

namespace VOYG.CPP.Management.Api.Services
{
    public class BlobServiceProvider : IBlobServiceProvider
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobServiceProvider(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<BlobFile> GenerateBlobFileUrlAndHash(string containerName, string filePath, CancellationToken cancellationToken)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = blobContainerClient.GetBlobClient(filePath);

            var blobSasBuilder = new BlobSasBuilder { ExpiresOn = DateTimeOffset.UtcNow.AddHours(6) };
            blobSasBuilder.SetPermissions(BlobSasPermissions.Read);

            var blobPropertiesValue = (await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken)).Value;

            var md5Hash = Convert.ToHexString(blobPropertiesValue.ContentHash).ToLower();

            return new BlobFile(blobClient.Uri.Segments.Last(), blobClient.GenerateSasUri(blobSasBuilder).AbsoluteUri, md5Hash, blobPropertiesValue.ContentLength);
        }

        public async Task<string?> DownloadBlobContentAsString(string containerName, string filePath, CancellationToken cancellationToken)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = blobContainerClient.GetBlobClient(filePath);

            var blobDownloadResult = await blobClient.DownloadContentAsync(cancellationToken);

            if (blobDownloadResult.GetRawResponse().Status != StatusCodes.Status200OK)
            {
                return null;
            }

            return Encoding.UTF8.GetString(blobDownloadResult.Value.Content);
        }

        public async Task<Response<BlobContentInfo>> UploadBlob(
            string blobContainerName,
            string blobName,
            object objectToSerialize,
            JsonSerializerSettings jsonSerializerSettings,
            CancellationToken cancellationToken)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);

            await blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var blobClient = blobContainerClient.GetBlobClient(blobName);

            var memoryStream = new MemoryStream();

            using var writer = new StreamWriter(memoryStream);
            using var jsonWriter = new JsonTextWriter(writer);

            var serializer = JsonSerializer.Create(jsonSerializerSettings);
            serializer.Serialize(jsonWriter, objectToSerialize);
            jsonWriter.Flush();

            memoryStream.Position = 0;

            return await blobClient.UploadAsync(memoryStream, true, cancellationToken);
        }
    }
}
