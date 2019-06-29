using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Collections.Generic;
using Proxylib;
using Healthcare.Proofing;
using Healthcare.BC.Offchain.Repository.Models;

namespace Healthcare.Proofing.Service
{
    public class ProofStorage
    {
        private string storageConnectionString;         //blob storage connection
        private string apiEndPoint;                     //for proxy connection
        private CloudStorageAccount storageAccount;
        private CloudBlobClient blobClient;
        private IConfiguration _configuration;



        public ProofStorage(IConfiguration configuration)
        {
            _configuration = configuration;
            storageConnectionString = _configuration["proofvault_connectionstring"];
            apiEndPoint = _configuration["api_endpoint"];
            storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            blobClient = storageAccount.CreateCloudBlobClient();


        }

        async public Task<DocProof> PutProof(string bindingId, string citizenIdentifier, Microsoft.AspNetCore.Http.IFormFile proofDocs = null)
        {
            //create storage container and set permissions
            CloudBlobContainer container = blobClient.GetContainerReference(bindingId);
            await container.CreateIfNotExistsAsync();

            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Off
            };

            await container.SetPermissionsAsync(permissions);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(citizenIdentifier);

            // string fileString = "";
            byte[] fileHash = null;

            using (var fileStream = proofDocs.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
                fileStream.Seek(0, SeekOrigin.Begin);
                using (var md5 = MD5.Create())
                {
                    fileHash = md5.ComputeHash(fileStream);
                }
            }

            var storedProofDocs = new DocProof()
            {
                Container = bindingId,
                ContentType = proofDocs.ContentType,
                FileName = citizenIdentifier,
                Hash = GetHash(bindingId, Encoding.UTF8.GetString(fileHash)),
                StorageSharding = "none"
            };

            //Update profile with proof document info
            //Should be controlled by Application  -- Commented by DB
            //await UpdateProofDocument(bindingId, storedProofDocs, "The Proof document was proven and stored");

            return await Task.Run(() => storedProofDocs);
        }

        async public Task<bool> ValidateHash(string bindingId, string citizenIdentifier, string hash)
        {
            CloudBlobContainer container = blobClient.GetContainerReference(bindingId);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(citizenIdentifier);

            //string fileString = "";
            byte[] fileHash = null;

            using (var fileStream = new MemoryStream())
            {
                await blockBlob.DownloadToStreamAsync(fileStream);

                fileStream.Seek(0, SeekOrigin.Begin);
                using (var md5 = MD5.Create())
                {
                    fileHash = md5.ComputeHash(fileStream);
                }
                //fileString = fileStream.ToString();
            }

            //System.Diagnostics.Debugger.Break();

            return (hash == GetHash(bindingId, Encoding.UTF8.GetString(fileHash)));
        }


        async public Task<string> GetProof(string bindingId, string citizenIdentifier)
        {
            CloudBlobContainer container = blobClient.GetContainerReference(bindingId);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(citizenIdentifier);


            //set sasToken time constraints
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-5);
            sasConstraints.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(5);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write;

            string sasBlobToken = blockBlob.GetSharedAccessSignature(sasConstraints);

            return await Task.Run(() => $"{blockBlob.Uri + sasBlobToken}");
        }

        private string GetHash(string bindingId, string fileString)
        {
            using (var algorithm = SHA256.Create())
            {
                var hashedBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(bindingId + fileString));
                return BitConverter.ToString(hashedBytes).Replace("-", "");
            }
        }

    }
}
