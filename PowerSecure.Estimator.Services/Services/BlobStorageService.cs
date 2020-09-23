using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSecure.Estimator.Services.Services
{
    public class BlobStorageService
    {
        private static readonly string BLOB_STORAGE_CONNECTION_STRING = AppSettings.Get("BlobStorageConnectionString");
        private static readonly string BLOB_STORAGE_CONTAINER_NAME = AppSettings.Get("BlobStorageContainerName");

        private static CloudBlobContainer GetCloudBlobContainer(string containerName = null)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(BLOB_STORAGE_CONNECTION_STRING);
            var blobClient = cloudStorageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(string.IsNullOrEmpty(containerName) ? BLOB_STORAGE_CONTAINER_NAME : containerName);
        }

        public async Task<(object, string)> UploadFile(Stream blob, string path, ILogger log)
        {
            log.LogInformation($"Uploading file - {path}");
            var blobContainer = GetCloudBlobContainer();
            var blobBlock = blobContainer.GetBlockBlobReference(path);
            using (var stream = blob)
            {
                await blobBlock.UploadFromStreamAsync(stream);
            }

            return (path, "OK");
        }

        public async Task<(object, string)> DownloadFile(string path, ILogger log)
        {
            log.LogInformation($"Downloading file - {path}");
            var blobContainer = GetCloudBlobContainer();
            var blobBlock = blobContainer.GetBlockBlobReference(path);
            if (await blobBlock.ExistsAsync())
            {
                Stream blobStream = await blobBlock.OpenReadAsync();
                return (blobStream, "OK");
            }
            return (null, "Error");
        }

        public async Task<(object,string)> DeleteFile(string path, ILogger log)
        {
            log.LogInformation($"Deleting file - {path}");
            var blobContainer = GetCloudBlobContainer();
            var blobBlock = blobContainer.GetBlockBlobReference(path);
            var deleted = await blobBlock.DeleteIfExistsAsync();

            return (deleted ? path : null, deleted ? "1 file deleted" : "No files deleted");
        }

        public async Task<Stream> GetResource(string path, ILogger log)
        {
            log.LogInformation($"Getting resource - {path}");
            var blobContainer = GetCloudBlobContainer("resources");
            var blobBlock = blobContainer.GetBlockBlobReference(path);
            if (await blobBlock.ExistsAsync())
            {
                return await blobBlock.OpenReadAsync();
            }
            return null;
        }
    }
}
