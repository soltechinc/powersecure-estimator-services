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
        static string _storageConnection = "BlobEndpoint=https://powersecureestimatorblob.blob.core.windows.net/;TableEndpoint=https://powersecureestimatorblob.table.core.windows.net/;SharedAccessSignature=sv=2019-02-02&ss=b&srt=sco&sp=rwdlac&se=2099-03-25T03:59:59Z&st=2020-03-24T15:10:40Z&spr=https&sig=j53pQUYsB7IU7GXexc4cm3kAknx9BDC8n%2BdNrUczacs%3D"; // AppSettings.Get("BlobStorageConnectionString");
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
        public static List<object> ConvertBlobListToFile(dynamic values) {
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
