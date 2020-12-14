using Microsoft.WindowsAzure.Storage.Blob;

namespace SForum.Data
{
    public interface IUpload
    {
        CloudBlobContainer GetBlobContainer(string connectionString);
    }
}