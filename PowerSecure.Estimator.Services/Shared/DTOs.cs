using Microsoft.Azure.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PowerSecure.Estimator.Services.Shared {
    public class DTOs {
        public class Document : IEquatable<Document> {
            public int CreatedBy { get; set; }
            public string FileName { get; set; }
            public Guid BlobGuid { get; set; }
            public string Uri { get; set; }
            public string DocType { get; set; }

            bool IEquatable<Document>.Equals(Document other) {
                //TODO: Implementation needed
                return true;
            }
        }


        public class ABSDTO {
            public ABSDTO() { }

            public string blobStorageAccountName { get; set; }
            public string blobStorageConnectionString { get; set; }
            public string blobStorageKey { get; set; }
            public string containerName { get; set; }
            public string sasToken { get; set; }
        }

       public class DocumentDTO {
            public DocumentDTO(Document data): base() {
                createdBy = data.CreatedBy;                
                fileName = data.FileName;
                blobGuid = data.BlobGuid;
                uri = data.Uri;
                docType = data.DocType;
            }
            public string docType { get; set; }
            public int createdBy { get; set; }
            public int documentTypeClassId { get; set; }
            public string fileName { get; set; }
            public Guid blobGuid { get; set; }
            public string uploadedBy { get; set; }
            public string uri { get; set; }
        }
    }
}
