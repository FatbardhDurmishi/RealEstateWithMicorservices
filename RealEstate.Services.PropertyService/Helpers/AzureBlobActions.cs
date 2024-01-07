using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace RealEstate.Services.PropertyService.Helpers
{
    public class AzureBlobActions
    {

        public static async Task<bool> UploadToBlob(BlobContainerClient containerClient, IFormFile file)
        {
            try
            {
                var blobClient = containerClient.GetBlobClient(file.FileName);
                var blobExists = await blobClient.ExistsAsync();

                if (blobExists)
                {
                    return true;
                }

                using (var inputStream = file.OpenReadStream())
                {
                    await containerClient.UploadBlobAsync(file.FileName, inputStream);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static async Task DeleteBlob(BlobContainerClient containerClient, string blobName)
        {
            var blobClient = containerClient.GetBlobClient(blobName);
            var blobExists = await blobClient.ExistsAsync();
            if (blobExists)
            {
                await containerClient.DeleteBlobAsync(blobName);
            }
        }

        public static async Task<string> GetBlobUrl(BlobContainerClient containerClient, string blobName)
        {
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            var blobExists = await blobClient.ExistsAsync();
            if (!blobExists)
            {
                return string.Empty;
            }
            DateTimeOffset expiresOn = DateTimeOffset.UtcNow.AddHours(1);

            Uri blobSasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, expiresOn);

            string blobUrl = blobSasUri.ToString();
            return blobUrl;
        }
    }
}
