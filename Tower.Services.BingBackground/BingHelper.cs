using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tower.Services.BingBackground
{
    internal static class BingHelper
    {
        private const string UrlBase = "https://www.bing.com";
        private const string ImagesEndpoint = "/HPImageArchive.aspx?format=xml&idx=0&n=8&mkt=en-GB";

        private static readonly HttpClient Client;

        static BingHelper()
        {
            Client = new HttpClient();
        }

        internal static async Task<List<string>> GetImageUrisAsync()
        {
            using (Stream stream = await Client.GetStreamAsync(UrlBase + ImagesEndpoint))
            using (var streamReader = new StreamReader(stream))
            {
                string content = await streamReader.ReadToEndAsync();
                IEnumerable<XElement> images = XElement.Parse(content).Elements("image");

                return (from image in images
                    select image.Element("urlBase")?.Value
                    into relativeUri
                    where relativeUri != null
                    select $"{UrlBase}{relativeUri}_1920x1080.jpg").ToList();
            }
        }
    }
}
