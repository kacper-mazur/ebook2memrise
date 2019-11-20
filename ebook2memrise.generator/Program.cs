using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HtmlAgilityPack;

namespace ebook2memrise.generator
{
    class Program
    {
        static void Main(string[] args)
        {
            //new ReversoProcessor().Process(File.ReadAllText("Reverso.html"), "проворный");

            var wordlist = File.ReadAllLines(@"GoldenDict-history.txt");
            
            string fileContent = "";
            var processor = new ReversoProcessor();
            foreach (var word in wordlist)
            {
                using (var client = new CookieAwareWebClient())
                {
                    var data = client.DownloadData("https://context.reverso.net/translation/russian-english/" + word);
                    var response = Encoding.UTF8.GetString(data);
                    fileContent += processor.Process(response, word) + "\r\n";
                }

                //forvo- copy cookies here, after logging in
                //Cookies.SetCookies(new Uri("https://forvo.com"), "");
                //using (var Client = new CookieAwareWebClient())
                //{
                //    var response = Client.DownloadString("https://forvo.com/word/" + word + "/#ru");

                //    var id = "3185443";
                //    Client.DownloadFile("https://forvo.com/download/mp3/" + word + "/ru/" + id, @"C:\Repos\MemriseGenerator\Forvo\" + response + ".mp3");
                //}
            }

            File.WriteAllText("memrise.txt", fileContent);
        }
    }

    
}
