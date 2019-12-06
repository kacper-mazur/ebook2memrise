using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ebook2memrise.generator
{
    public class ReversoProcessor
    {
        public string Process(string fileContent)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(fileContent);

                HtmlNode results = doc.DocumentNode.SelectSingleNode(
                    "/html/body/div[@id='wrapper']/section[@id='body-content']/div[@class='left-content']");

                var pos = results.SelectSingleNode("section[@id='top-results']/div[@id='pos-filters']/button").InnerText
                    .Trim();
                var translations = results
                    .SelectNodes(
                        "section[@id='top-results']/div[@id='translations-content']/div | section[@id='top-results']/div[@id='translations-content']/a")
                    .Select(s => s.InnerText.Trim());

                var examples = results.SelectNodes("section[@id='examples-content']/div").Select(s =>
                    Trim(s?.SelectSingleNode("div[@class='src ltr']/span[@lang='ru']")?.InnerText)
                    + " " +
                    Trim(s?.SelectSingleNode("div[@class='trg ltr']/span[@class='text']")?.InnerText?.Trim()));

                var prefix = "";
                if (pos?.ToLowerInvariant() == "verb")
                    prefix = "to ";

                var result =
                    $"{string.Join(", ", translations.Take(4).Select(t => prefix + t))}\t{string.Join(" | ", examples.Take(3))}\t{pos}";

                return result;
            }
            catch (NullReferenceException)
            {
                return string.Empty;
            }
        }

        public string ProcessForvo(string fileContent)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(fileContent);

            var result = doc.DocumentNode.SelectSingleNode("//p[@class='download']/span[@data-p4]/@data-p4").Attributes.First(a => a.Name == "data-p4").Value;
            return result;
        }

        private string Trim(string content)
        {
            if (string.IsNullOrEmpty(content))
                return string.Empty;

            var result = content?.Trim()?.Replace("\r\n", " ");
            while (result.Contains("  "))
            {
                result = result.Replace("  ", " ");
            }

            return result;
        }
    }
}
