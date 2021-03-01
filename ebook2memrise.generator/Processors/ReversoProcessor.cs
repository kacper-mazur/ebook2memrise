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
        public string Process(string fileContent, string countryCode, bool onlyExamples = false)
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
                    Trim(s?.SelectSingleNode("div[@class='src ltr']/span[@lang='" + countryCode + "']")?.InnerText)
                    + ";" +
                    Trim(s?.SelectSingleNode("div[@class='trg ltr']/span[@class='text']")?.InnerText?.Trim()));

                var prefix = "";
                if (pos?.ToLowerInvariant() == "verb")
                    prefix = "to ";

                string result;
                if (onlyExamples)
                    result = string.Join(" | ", examples.Take(1));
                else
                    result =
                    $"{string.Join(", ", translations.Take(2).Select(t => prefix + t))}\t{string.Join(" | ", examples.Take(1))}\t{pos}";

                return result;
            }
            catch (NullReferenceException)
            {
                return string.Empty;
            }
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
