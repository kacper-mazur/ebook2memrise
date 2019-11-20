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

            string word = "проворный";
            using (var Client = new CookieAwareWebClient("https://context.reverso.net", ""))
            {
                var response = Client.DownloadString("https://context.reverso.net/translation/russian-english/" + word);
            }

            //forvo- copy cookies here, after logging in
            //Cookies.SetCookies(new Uri("https://forvo.com"), "");
            using (var Client = new CookieAwareWebClient("https://forvo.com", ""))
            {
                var response = Client.DownloadString("https://forvo.com/word/" + word + "/#ru");

                var id = "3185443";
                Client.DownloadFile("https://forvo.com/download/mp3/" + word + "/ru/" + id, @"C:\Repos\MemriseGenerator\Forvo\" + response + ".mp3");
            }
        }
    }

    public class CookieAwareWebClient : System.Net.WebClient
    {
        private System.Net.CookieContainer Cookies = new System.Net.CookieContainer();

        public CookieAwareWebClient(string url, string cookies)
        {
            Cookies.SetCookies(new Uri(url), cookies);
        }

        protected override System.Net.WebRequest GetWebRequest(Uri address)
        {
            System.Net.WebRequest request = base.GetWebRequest(address);
            if (request is System.Net.HttpWebRequest)
            {
                var hwr = request as System.Net.HttpWebRequest;
                //By Reference, as hwr.CookieContainer gets updated so does Cookies.
                //We then assign the collection back to the collection at the start of the call
                hwr.CookieContainer = Cookies;
            }
            return request;
        }
    }
}
