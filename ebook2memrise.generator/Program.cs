using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using HtmlAgilityPack;

namespace ebook2memrise.generator
{
    class Program
    {
        static void Main(string[] args)
        {
            //new ReversoProcessor().Process(File.ReadAllText("Reverso.html"), "проворный");
            //new ReversoProcessor().ProcessForvo(File.ReadAllText("Forvo.html"));
            SkipExistingWords();

            var wordlist = File.ReadAllLines(@"GoldenDict-history.txt");


            string fileContent = "";
            var processor = new ReversoProcessor();
            int i = 0;
            foreach (var word in wordlist)
            {
                using (var client = new CookieAwareWebClient())
                {
                    try
                    {
                        var data = client.DownloadData(
                            "https://context.reverso.net/translation/russian-english/" + word);

                        var response = Encoding.UTF8.GetString(data);
                        var definition = processor.Process(response);
                        string url = string.Empty;

                        try
                        {
                            data = client.DownloadData("https://forvo.com/word/" + word + "/#ru");
                            response = Encoding.UTF8.GetString(data);

                            var id = processor.ProcessForvo(response);
                            url = "https://forvo.com/download/mp3/" + word + "/ru/" + id;

                            data = client.DownloadData(url);
                            response = Encoding.UTF8.GetString(data);
                            if (response.Contains("To see this page you need to be logged in."))
                            {
                                // skip
                            }
                            else
                                client.DownloadFile(url, "Forvo/" + (i++) + "_" + word + ".mp3");

                            System.Diagnostics.Process.Start(url);
                        }
                        catch
                        {
                            //ignore :-( 
                        }
                        fileContent += word + "\t" + definition + "\t" + url + "\r\n";
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("The remote server returned an error: (404) Not Found."))
                            continue;
                    }
                }
                //Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            if (File.Exists("memrise.txt"))
                File.Delete("memrise.txt");

            File.WriteAllText("memrise.txt", fileContent);
        }

        static void SkipExistingWords()
        {
            var wordlist = File.ReadAllLines(@"GoldenDict-history.txt");
            var existing = File.ReadAllLines(@"Russian-ready.txt");

            wordlist = wordlist.Where(w => !existing.Contains(w)).ToArray();

            var duplicates = wordlist
                .GroupBy(v => v)
                .ToDictionary(g => g.Key, v => v.Count())
                .Where(v=> v.Value >1)
                .Select(v=> v.Key);

            wordlist = wordlist.Where(w => !duplicates.Contains(w)).ToArray();

            File.Delete("GoldenDict-history.txt");
            File.WriteAllLines("GoldenDict-history.txt", wordlist);
        }
    }    
}
