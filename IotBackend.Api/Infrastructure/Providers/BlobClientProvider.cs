using Azure.Storage.Blobs;
using IotBackend.Api.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace IotBackend.Api.Infrastructure.Providers
{
    public interface IBlobClientProvider
    {
        BlobClient GetBlobClient(string blobPath);
    }
    public class BlobClientProvider : IBlobClientProvider
    {
        private readonly BlobContainerClient _blobContainerClient;
        public BlobClientProvider(IOptions<BlobConfiguration> configuration)
        {
            _blobContainerClient = new BlobContainerClient(configuration.Value.ConnectionString, configuration.Value.BlobContainerName);
        }

        public BlobClient GetBlobClient(string blobPath)
        {
            return _blobContainerClient.GetBlobClient(blobPath);
        }
    }
}