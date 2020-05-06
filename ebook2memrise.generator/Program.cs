using ebook2memrise.generator.Processors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ebook2memrise.generator
{
    class Program
    {
        static string countryCode = "es";
        static DictProcessor dictProcessor = new DictProcessor();
        static ForvoProcessor forvoProcessor = new ForvoProcessor();
        static AudioUploader audioUploader = new AudioUploader();

        static void Main(string[] args)
        {
            SkipExistingWords();

            var wordList = File.ReadAllLines($"GoldenDict-{countryCode}.txt");
            IList<string> toUpload = new List<string>();
            string notFound = "";
            string fileContent = "";
            foreach (var word in wordList)
            {
                using (var client = new CookieAwareWebClient())
                {
                    try
                    {
                        var data =
                            client.DownloadData("https://www.dict.com/" + GetLanguagePair() + "/" + word);

                        var response = Encoding.UTF8.GetString(data);
                        var localWord = dictProcessor.Process(word, response, out var definition, out var examples);
                        localWord = localWord.Replace("1", "").Replace("*", "").Trim();

                        fileContent += localWord + "\t" + definition + "\t" + examples + "\r\n";

                        //var localWord = word;
                        DownloadAudio(client, localWord);
                        toUpload.Add(localWord);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("The remote server returned an error: (404) Not Found."))
                            continue;
                        notFound += word + "\r\n";
                    }
                }
            }

            if (File.Exists("memrise.txt"))
                File.Delete("memrise.txt");

            File.WriteAllText("memrise.txt", fileContent);
            File.WriteAllText("notFound.txt", notFound);
            
            audioUploader.OpenBrowser("https://www.memrise.com/course/5712969/spanish-from-books-an-movies/edit/");

            System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Notepad++\notepad++.exe", new FileInfo("memrise.txt").FullName);

            audioUploader.Upload(toUpload, countryCode);
        }

        private static void DownloadAudio(CookieAwareWebClient client, string localWord)
        {
            try
            {
                var forvoData =
                    client.DownloadData("https://forvo.com/word/" + localWord.Replace(" ", "_") + "/#" + countryCode);
                var forvoResponse = Encoding.UTF8.GetString(forvoData);

                var id = forvoProcessor.Process(forvoResponse, countryCode);
                var url = "https://forvo.com/download/mp3/" + localWord.Replace(" ", "_") + "/" + countryCode + "/" + id;

                System.Diagnostics.Process.Start(url);
            }
            catch
            {
                //ignore :-( 
            }
        }

        static string GetLanguagePair()
        {
            switch (countryCode)
            {
                case "ru":
                    return "russian-english";
                case "en":
                    return "english-polish";
                case "es":
                    return "spanish-english";
            }

            throw new NotImplementedException();
        }

        static void SkipExistingWords()
        {
            var wordlist = File.ReadAllLines(@"GoldenDict-" + countryCode + ".txt").ToList();
            var existing = File.ReadAllLines(@"Files\\Ready-" + countryCode + ".txt").ToList();

            wordlist = wordlist.ConvertAll(d => d.ToLower());
            existing = existing.ConvertAll(d => d.ToLower());

            Console.Write("Before: " + wordlist.Count);
            wordlist = wordlist.Where(w => !existing.Contains(w)).ToList();

            var duplicates = wordlist
                .GroupBy(v => v)
                .ToDictionary(g => g.Key, v => v.Count())
                .Where(v => v.Value > 1)
                .Select(v => v.Key);

            wordlist = wordlist.Where(w => !duplicates.Contains(w)).ToList();
            wordlist.AddRange(duplicates);

            Console.Write("After: " + wordlist.Count);

            File.Delete("GoldenDict-history.txt");
            File.WriteAllLines("GoldenDict-" + countryCode + ".txt", wordlist);
        }
    }
}
