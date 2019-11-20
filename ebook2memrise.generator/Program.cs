using System;
using System.Collections.Generic;
using System.IO;

namespace ebook2memrise.generator
{
    class Program
    {

        static void Main(string[] args)
        {
            var wordlist = File.ReadAllLines(@"GoldenDict-history.txt");

            // reverso - copy cookies here, after logging in
            //Cookies.SetCookies(new Uri("https://context.reverso.net"), "");

            foreach (var word in wordlist)
            {
                using (var Client = new CookieAwareWebClient())
                {
                    var response = Client.DownloadString("https://context.reverso.net/translation/russian-english/" + word);
                }

                //forvo- copy cookies here, after logging in
                //Cookies.SetCookies(new Uri("https://forvo.com"), "");
                using (var Client = new CookieAwareWebClient())
                {
                    var response = Client.DownloadString("https://forvo.com/word/" + word + "/#ru");

                    var id = "3185443";
                    Client.DownloadFile("https://forvo.com/download/mp3/" + word + "/ru/" + id, @"C:\Repos\MemriseGenerator\Forvo\" + response + ".mp3");
                }
            }
        }
    }

    
}
