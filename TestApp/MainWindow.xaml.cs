using Google.Apis.Services;
using Google.Apis.Translate.v2;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;
using TestApp.Model;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSourceBrowse_Click(object sender, RoutedEventArgs e)
        {
            var fd = new OpenFileDialog();
            fd.AddExtension = true;
            fd.Filter = "Pliki tekstowe|*.txt";
            fd.Multiselect = false;
            if (fd.ShowDialog() == true)
            {
                tbSource.Text = fd.FileName;
            }
        }

        private void btnDestinationBrowse_Click(object sender, RoutedEventArgs e)
        {

            var fd = new SaveFileDialog();
            fd.AddExtension = true;
            fd.DefaultExt = "Pliki tekstowe|*.txt";
            fd.Filter = "Pliki tekstowe|*.txt";
            if (fd.ShowDialog() == true)
            {
                tbDestination.Text = fd.FileName;
            }
        }

        static HttpClient client = new HttpClient();
        private const string URL = "https://od-api.oxforddictionaries.com/api/v1";
        private const string DictionaryEntries = "/entries/{source_lang}/{word_id}";
        private const string TranslateEntries = "/entries/{source_translation_language}/{word_id}/translations={target_translation_language}";

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(tbSource.Text) || !Directory.Exists(System.IO.Path.GetDirectoryName(tbDestination.Text)))
                return;
            var srcText = File.ReadAllText(tbSource.Text).Split(new[] { ' ', ',', '.', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var translations = new List<DictionaryEntry>();

            var service = new TranslateService(new BaseClientService.Initializer()
            {
                ApiKey = ConfigurationManager.AppSettings["googleKey"],
                ApplicationName = "Translate API Sample"
            });

            foreach (var word in srcText)
            {
                var t = GetDefinition(word);
                if (t != null)
                {
                    t.Translation = TranslateText(word);
                    translations.Add(t);
                }
            }

            var sb = new StringBuilder();
            foreach (var translate in translations)
            {
                foreach (var result in translate.results)
                {
                    sb.AppendFormat("{0}\t{1}\t{2}\t{3}\r\n",
                        result.word,
                        string.Join("; ", result.lexicalEntries.Select(x => x.lexicalCategory + " - "
                            + string.Join(". ", x.entries.SelectMany(t => t.senses).SelectMany(t => t.definitions)))),
                         string.Join("; ", result.lexicalEntries.SelectMany(x => x.entries).SelectMany(x => x.senses).SelectMany(x => x.examples).Select(x => x.text)),
                         translate.Translation
                        );
                }
            }
            File.WriteAllText(tbDestination.Text, sb.ToString());
            foreach (var translate in translations)
            {
                foreach (var result in translate.results)
                {
                    var pronunciations = result.lexicalEntries.SelectMany(x => x.pronunciations).Select(x => x.audioFile).ToList();
                    pronunciations.AddRange(result.pronunciations?.Select(x => x.audioFile) ?? new List<string>());
                    pronunciations = pronunciations.Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();

                    for (int i = 0; i < pronunciations.Count; ++i)
                        using (var client = new WebClient())
                        {
                            try
                            {
                                if (File.Exists(Path.Combine(Path.GetDirectoryName(tbDestination.Text), result.word + "_" + i + ".mp3")))
                                    File.Delete(Path.Combine(Path.GetDirectoryName(tbDestination.Text), result.word + "_" + i + ".mp3"));
                                client.DownloadFile(
                                    pronunciations[i],
                                    Path.Combine(Path.GetDirectoryName(tbDestination.Text), result.word + "_" + i + ".mp3"));
                            }
                            catch (Exception ex)
                            {
                                Console.Out.WriteLine("-----------------");
                                Console.Out.WriteLine(ex.Message);
                            }
                        }
                }
            }
        }

        private string GetDictionaryEntries(string language, string word)
        {
            return DictionaryEntries.Replace("{source_lang}", language).Replace("{word_id}", word);
        }


        private DictionaryEntry GetDefinition(string word)
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
