using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Opera;

namespace ebook2memrise.generator.Processors
{
    public class AudioUploader
    {
        private OperaDriver _driver;

        private string cookies = File.ReadAllText("Files\\Cookie.txt");

        public void OpenBrowser(string url)
        {
            try
            {
                _driver = new OperaDriver(@"C:\Users\k403806\Downloads\operadriver_win64\operadriver_win64");
                StringBuilder sb = new StringBuilder();
                _driver.Navigate().GoToUrl("https://www.memrise.com/");

                foreach (var cookie in cookies.Split(new[] { "\r\n" }, StringSplitOptions.None))
                    _driver.Manage().Cookies.AddCookie(Deserialize(cookie));

                //foreach (var cookie in _driver.Manage().Cookies.AllCookies)
                //    sb.AppendLine(Serialize(cookie));

                _driver.Navigate().GoToUrl(url);


                // show full list of words
                //var element = _driver.FindElementByXPath("//h3[contains(@class, 'level-name') and contains(text(),'" +
                //                                         url +
                //                                         "')]/parent::div//a[contains(@class,'show-hide')]");
                //element.Click();
            }
            finally
            {

            }
        }

        public string GetExample(string url, string countryCode)
        {
            _driver.Navigate().GoToUrl(url);
            var xPath = "//section[@id='examples-content']/div";
            var response = _driver.FindElementsByXPath(xPath);

            return response.FirstOrDefault()?.Text.Replace("\r\n", ",");
        }

        public string Upload(IList<string> words, string countryCode)
        {
            IList<string> notFound = new List<string>();
            try
            {
                foreach (var word in words)
                {
                    // upload an audio file
                    var file = GetFileName(countryCode, word);
                    if (!File.Exists(file))
                    {
                        notFound.Add(word);
                        continue;
                    }

                    try
                    {
                        foreach (var el in
                            _driver.FindElementsByXPath("//div[@class='text' and text()='" + word +
                                                        "']//ancestor::tr//div[contains(@class,'files-add')]//input"))
                        {
                            el.SendKeys(file);
                        }
                    }
                    catch
                    {
                        notFound.Add(word);
                    }
                }

                foreach (var word in words)
                {
                    string file =
                        Constants.AudioFileDirectory + "\\pronunciation_" + countryCode + "_"
                        + word
                        + ".mp3";
                    File.Delete(file);
                }
            }
            finally
            {
                _driver.Quit();
            }

            return string.Join("\r\n", notFound);
        }

        private static string GetFileName(string countryCode, string word)
        {
            string file =
                Constants.AudioFileDirectory + "\\pronunciation_" + countryCode +
                "_"
                + word
                + ".mp3";
            return file;
        }

        private string Serialize(Cookie c)
        {
            return
                c.Name + ";" + c.Value + ";" + c.Domain + ";" + c.Path;
        }

        private Cookie Deserialize(string s)
        {
            var c = s.Split(';');
            return new Cookie(c[0], c[1], c[2], c[3], null);
        }

        public static void Concatenate(string first, string second, string output)
        {
            Stream w = File.OpenWrite(output);

            File.OpenRead(first).CopyTo(w);
            File.OpenRead(second).CopyTo(w);

            w.Flush();
            w.Close();
        }
    }
}
