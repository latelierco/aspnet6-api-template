namespace Template.AspNet6.Application.Services.Persistence;

public interface IStoreService
{
    Task<MemoryStream?> GetBlobStreamIfExistsAsync(string container, string filename, string subDirectory = "");
    Task<byte[]?> GetBlobContentIfExistsAsync(string container, string filename, string subDirectory = "");

    Task<string> StoreBlobAsync(string container, string filename, Stream stream, string subDirectory = "", string? contentType = null, bool isPublicStorage = false, int retryCount = 5);

    Task<string?> GetBlobUrlIfExistsAsync(string container, string filename, string subDirectory = "");

    Task<bool> DeleteBlobIfExistsAsync(string container, string filename, string subDirectory = "");
}
