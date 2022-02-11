using Microsoft.WindowsAzure.Storage.Blob;

namespace StarForum.Infrastructure.Services;

public interface IUpload
{
    CloudBlobContainer GetBlobContainer(string connectionString, string containerName);
}