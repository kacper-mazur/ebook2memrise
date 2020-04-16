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

        private string cookies =
            @"";


        public void Upload(IList<string> words, string level)
        {
            try
            {
                _driver = new OperaDriver(@"C:\Users\k403806\Downloads\operadriver_win64\operadriver_win64");
                StringBuilder sb = new StringBuilder();
                _driver.Navigate().GoToUrl("https://www.memrise.com/");

                foreach (var cookie in cookies.Split(new[] { "\r\n" }, StringSplitOptions.None))
                    _driver.Manage().Cookies.AddCookie(Deserialize(cookie));

                _driver.Navigate().GoToUrl("https://www.memrise.com/course/2232929/conjugation-of-spanish-verbs/edit/");
                //foreach (var cookie in _driver.Manage().Cookies.AllCookies)
                //    sb.AppendLine(Serialize(cookie));

                // show full list of words
                var element = _driver.FindElementByXPath(
                    "//div[contains(@class, 'level-handle') and contains(text(),'"+ level + "')]//ancestor::div//a[contains(@class,'show-hide')]");
                element.Click();

                foreach (var word in words)
                {
                    // upload an audio file
                    string file =
                        @"C:\Repos\kacper-mazur\ebook2memrise\ebook2memrise.generator\bin\Debug\Español\pronunciation_es_"
                        + word
                        + ".mp3";

                    foreach (var el in
                        _driver.FindElementsByXPath("//div[@class='text' and text()='" + word +
                                                    "']//ancestor::tr//div[contains(@class,'files-add')]//input"))
                    {
                        el.SendKeys(file);
                    }
                }

                Thread.Sleep(2000);

                foreach (var word in words)
                {
                    string file =
                        @"C:\Repos\kacper-mazur\ebook2memrise\ebook2memrise.generator\bin\Debug\Español\pronunciation_es_"
                        + word
                        + ".mp3";
                    File.Delete(file);
                }
            }
            finally
            {
                _driver.Quit();
            }
        }

        private string Serialize(Cookie c)
        {
            return
                c.Name + ";" + c.Value + ";" + c.Domain + ";" + c.Path;
        }

        private Cookie Deserialize(string s)
        {
            var c = s.Split(';');
            return new Cookie(c[0], c[1], c[3], null);
        }
    }
}
