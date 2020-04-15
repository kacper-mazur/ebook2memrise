using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ebook2memrise.generator
{
    public class ForvoProcessor
    {
        public string Process(string fileContent, string countryCode)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(fileContent);

            var result = doc.DocumentNode.SelectSingleNode("//p[@class='download']/span[@data-p3='" + countryCode + "']/@data-p4").Attributes.First(a => a.Name == "data-p4").Value;
            return result;
        }
    }
}
