using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ebook2memrise.generator
{
    public class CookieAwareWebClient : System.Net.WebClient
    {
        private System.Net.CookieContainer Cookies = new System.Net.CookieContainer();

        public CookieAwareWebClient()
        {
            Cookies.SetCookies(new Uri("https://context.reverso.net"), "");
            Cookies.SetCookies(new Uri("https://forvo.com"), "");
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
