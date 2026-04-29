using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CatalogService.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CatalogService.Infrastructure.Services;

// CatalogService.Infrastructure/Services/BlobStorageService.cs
public class BlobStorageService : IBlobStorageService
{
  private readonly BlobContainerClient _container;

  public BlobStorageService(IConfiguration config)
  {
    var client = new BlobServiceClient(
        new Uri($"https://{config["BlobStorage:ContainerName"]}.blob.core.windows.net"),
        new DefaultAzureCredential()
    );
    _container = client.GetBlobContainerClient(config["BlobStorage:ContainerName"]);
    _container.CreateIfNotExists(PublicAccessType.Blob); // public read
  }

  public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
  {
    var blobName = $"{Guid.NewGuid()}-{Path.GetFileName(fileName)}";
    var blob = _container.GetBlobClient(blobName);
    await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);
    return blob.Uri.ToString();
  }
}
