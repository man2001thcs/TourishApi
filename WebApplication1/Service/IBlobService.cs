namespace WebApplication1.Service
{
    public interface IBlobService
    {
        Task<Uri> UploadFileBlobAsync(string blobContainerName, Stream content, string contentType, string fileName);
        Task<Boolean> DeleteFileBlobAsync(string blobContainerName, string fileName);
    }
}
