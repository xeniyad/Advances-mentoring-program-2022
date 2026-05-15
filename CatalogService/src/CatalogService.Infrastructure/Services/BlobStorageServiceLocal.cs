using CatalogService.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CatalogService.Infrastructure.Services;

public class BlobStorageServiceLocal : IBlobStorageService
{
  private readonly string _folderPath;
  private readonly string _baseUrl;

  public BlobStorageServiceLocal(IConfiguration config)
  {
    _folderPath = config["LocalStorage:FolderPath"]
        ?? throw new InvalidOperationException("LocalStorage:FolderPath is not configured.");
    _baseUrl = config["LocalStorage:BaseUrl"]?.TrimEnd('/')
        ?? throw new InvalidOperationException("LocalStorage:BaseUrl is not configured.");

    Directory.CreateDirectory(_folderPath);
  }

  public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
  {
    var blobName = $"{Guid.NewGuid()}-{Path.GetFileName(fileName)}";
    var filePath = Path.Combine(_folderPath, blobName);

    await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
    await stream.CopyToAsync(fileStream, ct);

    return $"{_baseUrl}/{blobName}";
  }
}
