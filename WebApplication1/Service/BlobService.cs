using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace WebApplication1.Service
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<Uri> UploadFileBlobAsync(string blobContainerName, Stream content, string contentType, string fileName)
        {
            var containerClient = GetContainerClient(blobContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType });
            return blobClient.Uri;
        }

        public async Task<Boolean> DeleteFileBlobAsync(string blobContainerName, string fileName)
        {
            try
            {
                var containerClient = GetContainerClient(blobContainerName);
                var blobClient = containerClient.GetBlobClient(fileName);

                await blobClient.DeleteIfExistsAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private BlobContainerClient GetContainerClient(string blobContainerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
            containerClient.CreateIfNotExists(PublicAccessType.Blob);
            return containerClient;
        }
    }
}
