
using CatalogService.SharedKernel;

namespace CatalogService.Core.ValueObjects;
public class Image : ValueObject
{
  public Image(string? url, string? altText = null)
  {
    Url = url;
    AltText = altText;
  }

  public string? Url { get; set; }
  public string? AltText { get; set; }

  public string GenerateHtmlTag()
  {
    return $"<img src=\"{Url}\" alt=\"{AltText}\" />";
  }
}
