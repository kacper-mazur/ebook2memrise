using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ebook2memrise.generator
{
    public class DictProcessor
    {
        public string Process(string word, CookieAwareWebClient client, string languagePair, out string definitions, out string examples)
        {
            var data =
                              client.DownloadData("https://www.dict.com/" + languagePair + "/" + word);
            var fileContent = Encoding.UTF8.GetString(data);

            if (fileContent.Contains("Sorry, no entry was found") || fileContent.Contains("span class=\"no_entry_found\""))
            {
                definitions = "";
                examples = "";
                return word;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(fileContent);

            var result = doc.DocumentNode.SelectSingleNode("//tr[@class='head']//span[@class='lex_ful_entr l1']").InnerText.Replace("&#0769;", "");
            definitions = string.Join(", ",
                doc.DocumentNode.SelectNodes("//span[@class='lex_ful_tran w l2']")?.Select(t => t.InnerText).ToArray()
                ?? new string[] { });

            examples = //lex_ful_coll2
                string.Join(", ",
                        doc.DocumentNode.SelectNodes("//span[@class='lex_ful_coll2']")?.Select(t => t.InnerText)
                        ?? new string[] { })
                    ?.Replace("&#0769;", "");

            return result;
        }
    }
}
