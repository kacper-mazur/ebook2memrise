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
                    var data = client.DownloadData("https://context.reverso.net/translation/russian-english/" + word);
                    var response = Encoding.UTF8.GetString(data);
                    response = processor.Process(response);
                    fileContent += word + "\t" + response + "\r\n";

                    try
                    {
                        data = client.DownloadData("https://forvo.com/word/" + word + "/#ru");
                        response = Encoding.UTF8.GetString(data);

                        var id = processor.ProcessForvo(response);
                        client.DownloadFile("https://forvo.com/download/mp3/" + word + "/ru/" + id, "Forvo/" + (i++) +"_" + word + ".mp3");
                    }
                    catch { 
                    //ignore :-( 
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
            File.Delete("GoldenDict-history.txt");
            File.WriteAllLines("GoldenDict-history.txt", wordlist);
        }
    }    
}
