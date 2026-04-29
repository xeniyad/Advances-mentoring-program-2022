using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Core.Interfaces;

public interface IBlobStorageService
{
  Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);
}
