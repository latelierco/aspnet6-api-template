using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Template.AspNet6.Application.Services.Persistence;

namespace Template.AspNet6.Infra.Persistence.Blob;

public class BlobStorageService : IStoreService
{
    private readonly CloudStorageAccount _storageAccount;

    public BlobStorageService(IConfiguration configuration)
    {
        var storageCnx = configuration["ConnectionStrings:Storage"];
        if (string.IsNullOrWhiteSpace(storageCnx))
            throw new ArgumentNullException(nameof(storageCnx));

        var parseSuccess = CloudStorageAccount.TryParse(storageCnx, out _storageAccount);
        if (!parseSuccess) throw new ArgumentException("storage account parse failed");
    }


    public async Task<string> StoreBlobAsync(string container, string filename, Stream stream, string subDirectory = "", string? contentType = null, bool isPublicStorage = false, int retryCount = 5)
    {
        string? sasToken = null;
        CloudBlockBlob blob;

        var cloudBlobClient = _storageAccount.CreateCloudBlobClient();
        var cloudBlobContainer = cloudBlobClient.GetContainerReference(container);

        //Create container
        if (!await cloudBlobContainer.ExistsAsync())
        {
            if (isPublicStorage)
                await cloudBlobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Container, null,
                    null);
            else
                await cloudBlobContainer.CreateIfNotExistsAsync();
        }

        //Create blob
        if (!string.IsNullOrWhiteSpace(subDirectory))
        {
            var dir = cloudBlobContainer.GetDirectoryReference(subDirectory);
            blob = dir.GetBlockBlobReference(filename);
        }
        else
        {
            blob = cloudBlobContainer.GetBlockBlobReference(filename);
        }

        //Try download
        await blob.UploadFromStreamAsync(stream);
        for (var i = 0; i < retryCount; i++)
        {
            if (blob.Properties.Length > 400) break;

            Thread.Sleep(i * 1_500);

            stream.Seek(0, SeekOrigin.Begin);
            await blob.UploadFromStreamAsync(stream);
        }

        //Generate access token
        if (!isPublicStorage)
        {
            var policy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddYears(2)
            };
            sasToken = blob.GetSharedAccessSignature(policy);
        }

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            blob.Properties.ContentType = contentType;
            await blob.SetPropertiesAsync();
        }

        //Replace storage url by CDN url if exists
        var finalUri = blob.Uri.AbsoluteUri;
        if (!string.IsNullOrWhiteSpace(sasToken))
            finalUri += sasToken;

        return finalUri;
    }

    public async Task<byte[]?> GetBlobContentIfExistsAsync(string container, string filename, string subDirectory = "")
    {
        var cloudBlobClient = _storageAccount.CreateCloudBlobClient();
        var cloudBlobContainer = cloudBlobClient.GetContainerReference(container);

        CloudBlockBlob blob;

        if (!string.IsNullOrWhiteSpace(subDirectory))
        {
            var dir = cloudBlobContainer.GetDirectoryReference(subDirectory);
            blob = dir.GetBlockBlobReference(filename);
        }
        else
        {
            blob = cloudBlobContainer.GetBlockBlobReference(filename);
        }

        await blob.FetchAttributesAsync();
        var fileContent = new byte[blob.Properties.Length];

        await blob.DownloadToByteArrayAsync(fileContent, 0);

        return fileContent;
    }

    public async Task<string?> GetBlobUrlIfExistsAsync(string container, string filename, string subDirectory = "")
    {
        var cloudBlobClient = _storageAccount.CreateCloudBlobClient();
        var cloudBlobContainer = cloudBlobClient.GetContainerReference(container);

        CloudBlockBlob blob;

        if (!string.IsNullOrWhiteSpace(subDirectory))
        {
            var dir = cloudBlobContainer.GetDirectoryReference(subDirectory);
            blob = dir.GetBlockBlobReference(filename);
        }
        else
        {
            blob = cloudBlobContainer.GetBlockBlobReference(filename);
        }

        if (await blob.ExistsAsync())
        {
            var policy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddYears(2)
            };
            var sasToken = blob.GetSharedAccessSignature(policy);
            var finalUri = blob.Uri.AbsoluteUri;
            if (!string.IsNullOrWhiteSpace(sasToken))
                finalUri += sasToken;

            return finalUri;
        }

        return null;
    }

    /// <summary>
    ///     Delete a file in the azure storage.
    /// </summary>
    /// <param name="container">The string name of the container</param>
    /// <param name="filename">the filename of the blob</param>
    /// <param name="subDirectory">subdirectory of the container</param>
    /// <returns></returns>
    public Task<bool> DeleteBlobIfExistsAsync(string container, string filename, string subDirectory = "")
    {
        var cloudBlobClient = _storageAccount.CreateCloudBlobClient();
        var cloudBlobContainer = cloudBlobClient.GetContainerReference(container);
        CloudBlockBlob blob;

        if (!string.IsNullOrWhiteSpace(subDirectory))
        {
            var dir = cloudBlobContainer.GetDirectoryReference(subDirectory);
            blob = dir.GetBlockBlobReference(filename);
        }
        else
        {
            blob = cloudBlobContainer.GetBlockBlobReference(filename);
        }

        return blob.DeleteIfExistsAsync();
    }

    public async Task<MemoryStream?> GetBlobStreamIfExistsAsync(string container, string filename, string subDirectory = "")
    {
        var cloudBlobClient = _storageAccount.CreateCloudBlobClient();
        var cloudBlobContainer = cloudBlobClient.GetContainerReference(container);

        CloudBlockBlob blob;

        if (!string.IsNullOrWhiteSpace(subDirectory))
        {
            var dir = cloudBlobContainer.GetDirectoryReference(subDirectory);
            blob = dir.GetBlockBlobReference(filename);
        }
        else
        {
            blob = cloudBlobContainer.GetBlockBlobReference(filename);
        }

        if (!await blob.ExistsAsync())
            return null;

        var stream = new MemoryStream();
        await blob.DownloadToStreamAsync(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }
}
