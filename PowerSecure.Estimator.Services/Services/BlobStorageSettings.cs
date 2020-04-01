using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace PowerSecure.Estimator.Services.Services {
    public static class BlobStorageSettings {
        
        public static List<CloudBlob> BlobList { get; set; }
        static string containerName = "file-uploads";
        static string _storageConnection = AppSettings.Get("BlobStorageConnectionString");
        static CloudStorageAccount _storageAccount = CloudStorageAccount.Parse(_storageConnection);
        static CloudBlobClient _blobClient = _storageAccount.CreateCloudBlobClient();
        static CloudBlobContainer _blobContainer = _blobClient.GetContainerReference(containerName);
        public static CloudBlob Blob { get; set; }


        public static async void UploadIntoBlobStorage(Models.File newFile, IFormFile file) {
            if (await _blobContainer.CreateIfNotExistsAsync()) {
                await _blobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }
            string imageName = "Test-" + Path.GetExtension(file.Name);
            CloudBlockBlob cloudBlockBlob = _blobContainer.GetBlockBlobReference(containerName);
            //using (var filestream = newFile.CreateFileInDirectory()) {
            //    cloudBlockBlob.UploadFromStreamAsync(filestream);

            //}
        }

        private static string FindDescription(IDictionary<string, string> dict) {
            string value = "";
            foreach(var d in dict) {
                if(dict.Count > 1 && d.Key != "description") {
                    continue;
                } else if(dict.Count == 1 && d.Key != "description") {
                    value = "DESCRIPTION WASNT SET";
                } else {
                    value = d.Value;
                }
            }
            return value;
        }
        public static List<Models.File> ConvertBlobListToFile(dynamic values) {
            foreach(var blob in BlobList) {
                Models.File file = new Models.File();
                file.Name = blob.Name;
                file.Uri = blob.Uri.ToString();
                file.Created = (DateTime)blob.Properties.Created?.DateTime;
                file.Description = FindDescription(blob.Metadata);
                values.Add(file);

            }
            return values;
        }

        public async static void GetAllFiles() {           
            BlobList = Task.Run(async () => await ListBlobsFlatListingAsync(_blobContainer, null)).Result;
        }

        private static async Task<List<CloudBlob>> ListBlobsFlatListingAsync(CloudBlobContainer container, int? segmentSize) {
            BlobContinuationToken continuationToken = null;
            List<CloudBlob> blobList = new List<CloudBlob>();
            CloudBlob blob;            
            try {                
                BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(string.Empty,
                    true, BlobListingDetails.Metadata, segmentSize, continuationToken, null, null);
                foreach (var blobItem in resultSegment.Results) {
                    blob = (CloudBlob)blobItem;
                    blobList.Add(blob);
                    Console.WriteLine("Blob name: {0}", blob.Name);
                }                
                return blobList;
            } catch (StorageException e) {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
    }
}
