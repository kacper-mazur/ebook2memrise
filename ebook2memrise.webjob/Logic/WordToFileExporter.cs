using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ebook2memrise.webjob.Model;
using ebook2memrise.model;

namespace ebook2memrise.webjob.Logic
{
    public class WordToFileExporter
    {
        private string PrepareFileName(string destination, int counter)
        {
            return Path.Combine(
                Path.GetDirectoryName(destination),
                Path.GetFileNameWithoutExtension(destination) + "_" + counter.ToString().PadLeft(4, '0') + Path.GetExtension(destination));
        }
        public void Process(IList<DictionaryEntry> translations, string destination)
        {
            var sb = new StringBuilder();
            var words = new List<model.words>();
            foreach (var translate in translations)
            {
                foreach (var result in translate.results)
                {
                    string definition = string.Join("; ",
                        result.lexicalEntries?
                            .Where(x=> x.entries != null)
                            .Select(x => x.lexicalCategory + " - "
                               + string.Join(". ", x.entries
                               .SelectMany(t => t.senses ?? new List<Sens>())
                               .SelectMany(t => t.definitions ?? new List<string>()))));

                    string example = string.Join("; ", result?.lexicalEntries?
                        .SelectMany(x => x.entries ?? new List<Entry>())
                        .SelectMany(x => x.senses ?? new List<Sens>())
                        .SelectMany(x => x.examples ?? new List<Example>())
                        .Select(x => x.text));

                    sb.AppendFormat("{0}\t{1}\t{2}\t{3}\r\n",
                        result.word,
                        definition,
                         example,
                         translate.Translation
                        );

                    words.Add(new model.words() { word = result.word, definition = definition, example = example, translation = translate.Translation });
                }
            }
            var counter = 0;
            var fileName = "";
            //bool append = false;
            do
            {
                counter++;
                fileName = PrepareFileName(destination, counter);
            }
            while (File.Exists(fileName)
            //|| File.ReadAllLines(fileName).Count() < 50
            );

            File.WriteAllText(fileName, sb.ToString());

            var audioFileDirectory = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
            if (!Directory.Exists(audioFileDirectory))
                Directory.CreateDirectory(audioFileDirectory);

            foreach (var translate in translations)
            {
                foreach (var result in translate.results)
                {
                    var pronunciations = result.lexicalEntries.SelectMany(x => x.pronunciations ?? new List<Pronunciation3>()).Select(x => x.audioFile).ToList();
                    pronunciations.AddRange(result.pronunciations?.Select(x => x.audioFile) ?? new List<string>());
                    pronunciations = pronunciations.Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();

                    for (int i = 0; i < pronunciations.Count; ++i)
                        using (var client = new WebClient())
                        {
                            try
                            {
                                if (File.Exists(Path.Combine(audioFileDirectory, result.word + "_" + i + ".mp3")))
                                    File.Delete(Path.Combine(audioFileDirectory, result.word + "_" + i + ".mp3"));
                                client.DownloadFile(
                                    pronunciations[i],
                                    Path.Combine(audioFileDirectory, result.word + "_" + i + ".mp3"));
                            }
                            catch (Exception ex)
                            {
                                Console.Out.WriteLine("-----------------");
                                Console.Out.WriteLine(ex.Message);
                            }
                        }
                }
            }

            using (var context = new ebook2memriseEntities())
            {
                context.words.AddRange(words);
                var strings = words.Select(w => w.word).ToList();
                var processed = context.raw_words.Where(r => strings.Contains(r.word)).ToList();
                context.raw_words.RemoveRange(processed);
                context.SaveChanges();
            }

            File.WriteAllLines(Path.Combine(Constants.ImportDirectory, fileName), words.Select(w => w.word).ToArray());
        }
    }
}
