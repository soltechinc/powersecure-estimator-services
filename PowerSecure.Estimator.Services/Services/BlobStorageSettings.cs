using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;

namespace PowerSecure.Estimator.Services.Services {
    public static class BlobStorageSettings {
        
        public static List<CloudBlob> BlobList { get; set; }
        static string containerName = "file-uploads";
        static string _storageConnection = AppSettings.Get("BlobStorageConnectionString");
        static CloudStorageAccount _storageAccount = CloudStorageAccount.Parse(_storageConnection);
        static CloudBlobClient _blobClient = _storageAccount.CreateCloudBlobClient();
        static CloudBlobContainer _blobContainer = _blobClient.GetContainerReference(containerName);
        public static CloudBlob Blob { get; set; }

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
        public static List<object> ConvertBlobListToFile(dynamic value) {
            List<object> list = new List<object>();
            foreach(var blob in BlobList) {
                value.Name = blob.Name;
                value.Uri = blob.Uri.ToString();
                value.Description = FindDescription(blob.Metadata);
                list.Add(value);                
            }
            return list;
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
