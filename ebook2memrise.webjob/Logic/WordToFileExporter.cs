using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ebook2memrise.webjob.Model;

namespace ebook2memrise.webjob.Logic
{
    public class WordToFileExporter
    {
        public void Process(IList<DictionaryEntry> translations, string destination)
        {
            var sb = new StringBuilder();
            foreach (var translate in translations)
            {
                foreach (var result in translate.results)
                {
                    sb.AppendFormat("{0}\t{1}\t{2}\t{3}\r\n",
                        result.word,
                        string.Join("; ", result?.lexicalEntries?.Select(x => x?.lexicalCategory + " - "
                            + string.Join(". ", x?.entries?.SelectMany(t => t?.senses)?.SelectMany(t => t?.definitions)))),
                         string.Join("; ", result?.lexicalEntries?.SelectMany(x => x?.entries)?.SelectMany(x => x?.senses)?.SelectMany(x => x.examples).Select(x => x.text)),
                         translate.Translation
                        );
                }
            }
            File.WriteAllText(destination, sb.ToString());
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
                                if (File.Exists(Path.Combine(Path.GetDirectoryName(destination), result.word + "_" + i + ".mp3")))
                                    File.Delete(Path.Combine(Path.GetDirectoryName(destination), result.word + "_" + i + ".mp3"));
                                client.DownloadFile(
                                    pronunciations[i],
                                    Path.Combine(Path.GetDirectoryName(destination), result.word + "_" + i + ".mp3"));
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
    }
}
