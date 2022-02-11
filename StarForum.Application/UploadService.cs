using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using StarForum.Infrastructure.Services;

namespace StarForum.Application;

public class UploadService : IUpload
{
    public CloudBlobContainer GetBlobContainer(string connectionString, string containerName)
    {
        var storageAccount = CloudStorageAccount.Parse(connectionString);
        var blobClient = storageAccount.CreateCloudBlobClient();
        return blobClient.GetContainerReference(containerName);
    }
}