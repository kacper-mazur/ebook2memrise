using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ebook2memrise.generator
{
    public class CookieAwareWebClient : System.Net.WebClient
    {
        private System.Net.CookieContainer Cookies = new System.Net.CookieContainer();

        public CookieAwareWebClient()
        {
            //Cookies.SetCookies(new Uri("https://context.reverso.net"), "context.lastpair=en-fr; JSESSIONID=uwm-HIwY-Wu8tDR5Fj-CZoit.bst-web15; CTXTNODEID=bstweb15; _gid=GA1.2.461396573.1576070063; reverso.net.ReversoRefreshToken=acdb18ac-5e9a-4883-85c1-b2c2e49222c5; _ga=GA1.2.2036515254.1576070063; _fbp=fb.1.1576070096632.1593187666; reverso.net.DeviceId=45ebe4d5-69c8-4648-ab59-297a12b645ad; reverso.net.ReversoAccessToken=ajQfiAfZtqraxUEhGcq_dGR-I_I5-NTsI-JxaFd2vQCwBvXsGr5BEbxNAu3BB3ILn5P_qA-_sQ3sTo1a9ofPhoV--UzLyaB8wQf2WitmlnkjIzhB-vWJ5ZK9rKKQei6dAiV0M0zuhG0VLWZIdK0f9_u6PmLUHVoJlGExSVA6V-pA_T6exkzoS-IcnzyuuUVBOqkQseHXAObubYFQ9kRISsFg2W6Fm25iptCGdRvDlo9hNYd7rfMeogjErUOJlaywI303a3a-F_7IJVFyA9MpLWEZXESjfoR9tjoc6yrjjpFSX6iTlJJKLK5h6N_K8Qx1NBfb_sNeQRMwUzMDT_mJ6IbmSw3tuCK9g1ddLMAad6uJUKsv; __qca=P0-1099270544-1576070111719; _gat=1; _gat_keywordTracker=1");
            //Cookies.SetCookies(new Uri("https://forvo.com"), "__gads=ID=34599f6e3898ebed:T=1576070595:S=ALNI_MapwoHUFCMtmUEm2_7O7uWqEo_28A; forvo_cookies=1; _gid=GA1.2.867128309.1577716367; _ga=GA1.2.2126202531.1576069994; __cfduid=db31e9cbfaff06e5b50c9a0fea338edc41576069988; PHPSESSID=fohjq0grk8rqmkobstkg9muhl1; id_user=Pc5GBq9YvhOm8t1gSBTfkw%3D%3D_84azyZbivIjzMy0pHZgf7g%3D%3D; wsl=ru; _gat=1;");
            //"G_ENABLED_IDPS=google; __gads=ID=34599f6e3898ebed:T=1576070595:S=ALNI_MapwoHUFCMtmUEm2_7O7uWqEo_28A; _gat=1; forvo_cookies=1; _gid=GA1.2.867128309.1577716367; _ga=GA1.2.2126202531.1576069994; __cfduid=db31e9cbfaff06e5b50c9a0fea338edc41576069988; PHPSESSID=fohjq0grk8rqmkobstkg9muhl1; id_user=Pc5GBq9YvhOm8t1gSBTfkw%3D%3D_84azyZbivIjzMy0pHZgf7g%3D%3D; wsl=be"
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

                hwr.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                
            }
            return request;
        }
    }
}
