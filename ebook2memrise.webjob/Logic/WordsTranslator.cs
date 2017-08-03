using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using ebook2memrise.webjob.Model;
using ebook2memrise.model;
using System.Threading;

namespace ebook2memrise.webjob.Logic
{
    public class WordsTranslator
    {
        private const string LogFileName = "translateLog.txt";
        private IList<string> toIgnore = new List<string>();

        public List<DictionaryEntry> Process()
        {
            if (File.Exists(LogFileName))
                File.Delete(LogFileName);

            toIgnore.Clear();
            var translations = new List<DictionaryEntry>();
            using (var context = new ebook2memriseEntities())
            {
                foreach (var word in context.raw_words.Take(Constants.FileSize*5))
                {
                    var t = GetDefinition(word.word);
                    if (t != null)
                    {
                        t.Translation = TranslateText2(word.word);
                        if (!string.IsNullOrEmpty(t.Translation))
                        {
                            translations.Add(t);
                            if (translations.Count >= Constants.FileSize)
                                break;
                        }
                        else
                            toIgnore.Add(word.word);
                    }
                }

                foreach (var t in toIgnore)
                {
                    if (!context.words.Any(db => db.word == t))
                        context.words.Add(new words() { word = t, exported = true, translation = "IGNORED" });
                    var raw = context.raw_words.FirstOrDefault(r => r.word == t);
                    if (raw != null)
                        context.raw_words.Remove(raw);
                }
                context.SaveChanges();
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
                Console.Out.WriteLine("----------------- " + word);
                Console.Out.WriteLine(ex.Message);
                File.AppendAllLines(LogFileName, new List<string>() { word });
                toIgnore.Add(word);
                return null;
            }
            finally
            {
                Thread.Sleep(1000);
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

        private string Query = @"https://glosbe.com/gapi/translate?from=eng&dest=pol&format=json&phrase={0}&pretty=true";
        public string TranslateText2(string word)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(Query,word));
            request.Method = "GET";
            request.ContentType = "application/json";
            try
            {
                WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                var t = JsonConvert.DeserializeObject<GlosbeTranslateEntry>(response);
                responseReader.Close();
                return string.Join(", ", t.tuc.Where(tuc=> tuc.phrase != null && !string.IsNullOrEmpty(tuc.phrase.text)).Take(3).Select(tuc=> tuc.phrase.text));
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("-----------------");
                Console.Out.WriteLine(ex.Message);
                return null;
            }
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
