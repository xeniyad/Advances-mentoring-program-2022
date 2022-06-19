using Microsoft.EntityFrameworkCore;

namespace Carting.DL
{
    [Keyless]
    public class Image
    {
        public Image() { }
        public Image(string url, string altText)
        {
            Url = url;
            AltText = altText;
        }

        public string Url { get; set; }
        public string AltText { get; set; }

        public string GenerateHtmlTag()
        {
            return $"<img src=\"{Url}\" alt=\"{AltText}\" />";
        }
    }
}
