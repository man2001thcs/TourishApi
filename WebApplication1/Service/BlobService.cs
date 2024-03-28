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

        public async Task<Uri> UploadStringBlobAsync(string blobContainerName, string content, string contentType, string fileName)
        {
            // Convert the string content to a byte array
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(content);

            // Create a memory stream from the byte array
            using (MemoryStream stream = new MemoryStream(byteArray))
            {
                // Call the original UploadFileBlobAsync method with the memory stream
                return await UploadFileBlobAsync(blobContainerName, stream, contentType, fileName);
            }
        }

        public async Task<string> GetBlobContentAsync(string blobContainerName, string blobName)
        {
            // Get a reference to the container client
            BlobContainerClient containerClient = GetContainerClient(blobContainerName);

            // Get a reference to the blob client
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            // Check if the blob exists
            if (!await blobClient.ExistsAsync())
            {
                throw new InvalidOperationException($"Blob '{blobName}' does not exist in container '{blobContainerName}'.");
            }

            // Download the blob content
            BlobDownloadInfo download = await blobClient.DownloadAsync();

            // Read the blob content into a string
            using (StreamReader reader = new StreamReader(download.Content))
            {
                return await reader.ReadToEndAsync();
            }
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
