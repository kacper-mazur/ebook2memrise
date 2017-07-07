using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using ebook2memrise.webjob.Model;

namespace ebook2memrise.webjob.Logic
{
    public class WordsTranslator
    {
        public List<DictionaryEntry> Process(IList<string> words)
        {
            var translations = new List<DictionaryEntry>();
            foreach (var word in words)
            {
                var t = GetDefinition(word);
                if (t != null)
                {
                    t.Translation = TranslateText(word);
                    translations.Add(t);
                }
            }
            return translations;
        }

        private const string URL = "https://od-api.oxforddictionaries.com/api/v1";
        private const string DictionaryEntries = "/entries/{source_lang}/{word_id}";
        private const string TranslateEntries = "/entries/{source_translation_language}/{word_id}/translations={target_translation_language}";


        private string GetDictionaryEntries(string language, string word)
        {
            return DictionaryEntries.Replace("{source_lang}", language).Replace("{word_id}", word);
        }


        public DictionaryEntry GetDefinition(string word)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL + GetDictionaryEntries("en", word));
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["app_id"] = ConfigurationManager.AppSettings["oxfordID"];
            request.Headers["app_key"] = ConfigurationManager.AppSettings["oxfordKey"];
            try
            {
                WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                var t = JsonConvert.DeserializeObject<DictionaryEntry>(response);
                responseReader.Close();
                return t;
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
                return null;
            }
        }

        private string GetTranslateEntries(string source, string target, string word)
        {
            return @"
{
  'q': '{text}',
  'source': '{source_translation_language}',
  'target': '{target_translation_language}',
  'format': 'text',
   'key': '{key}'
}"
                .Replace("{source_translation_language}", source)
                .Replace("{text}", word)
                .Replace("{target_translation_language}", target)
                .Replace("{key}", ConfigurationManager.AppSettings["googleKey"]);


        }


        public string TranslateText(string word)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://translation.googleapis.com/language/translate/v2?key=" + ConfigurationManager.AppSettings["googleKey"]);
            request.Method = "POST";
            request.ContentType = "application/json";
            var data = GetTranslateEntries("en", "pl", word);
            request.ContentLength = data.Length;
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            requestWriter.Write(data);
            request.Headers["key"] = ConfigurationManager.AppSettings["googleKey"];
            requestWriter.Close();

            try
            {
                WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                var t = JsonConvert.DeserializeObject<TranslateEntry>(response);
                responseReader.Close();
                return string.Join(", ", t.data.translations.Select(x => x.translatedText));
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
                return null;
            }
        }
    }

}
