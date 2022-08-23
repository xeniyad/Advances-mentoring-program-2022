using Microsoft.EntityFrameworkCore;
using System;

namespace Carting.DL
{
    [Keyless]
    public class Image
    {
        private const char Delimeter = ';'; 
        public Image() { }
        public Image(string url, string altText)
        {
            Url = url;
            AltText = altText;
        }

        public Image(string dbString)
        {
            var args = dbString.Split(Delimeter);
            try
            {
                Url = args[0];
                AltText =args[1];
            }
            catch (Exception)
            {
                Url = string.Empty;
                AltText = string.Empty;
            }
        }

        public string Url { get; set; }
        public string AltText { get; set; }
        public string ToDbString()
        {
            return $"{Url}{Delimeter}{AltText}";
        }
        public string GenerateHtmlTag()
        {
            return $"<img src=\"{Url}\" alt=\"{AltText}\" />";
        }
    }
}
