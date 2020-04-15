using System;
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

        static void Main(string[] args)
        {
            //new ReversoProcessor().Process(File.ReadAllText("Reverso.html"), "проворный");
            //new ReversoProcessor().ProcessForvo(File.ReadAllText("Forvo.html"));
            //string definition1;
            //var word1 = new ReversoProcessor().ProcessDictCom(File.ReadAllText("Dict.com.2.html"), out definition1);

            SkipExistingWords();

            var wordList = File.ReadAllLines($"GoldenDict-{countryCode}.txt");
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
                        localWord = localWord.Replace("1", "").Replace("*", "");

                        fileContent += localWord + "\t" + definition + "\t" + examples + "\r\n";

                        DownloadAudio(client, word);
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
            var wordlist = File.ReadAllLines(@"GoldenDict-" + countryCode + ".txt");
            var existing = File.ReadAllLines(@"Files\\Ready-" + countryCode + ".txt");

            Console.Write("Before: " + wordlist.Length);
            wordlist = wordlist.Where(w => !existing.Contains(w)).ToArray();

            var duplicates = wordlist
                .GroupBy(v => v)
                .ToDictionary(g => g.Key, v => v.Count())
                .Where(v => v.Value > 1)
                .Select(v => v.Key);

            wordlist = wordlist.Where(w => !duplicates.Contains(w)).ToArray();

            Console.Write("After: " + wordlist.Length);

            File.Delete("GoldenDict-history.txt");
            File.WriteAllLines("GoldenDict-" + countryCode + ".txt", wordlist);
        }
    }
}
