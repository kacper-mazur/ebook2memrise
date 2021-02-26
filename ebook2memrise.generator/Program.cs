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
        static string application = "fiszkoteka"; // or memrise
        static int wordsToProcess = 50;

        static DictProcessor dictProcessor = new DictProcessor();
        static ForvoProcessor forvoProcessor = new ForvoProcessor();
        static AudioUploader audioUploader = new AudioUploader();
        static ReversoProcessor reversoProcessor = new ReversoProcessor();

        static List<string> toUpload = new List<string>();
        static List<string> processed = new List<string>();
        static string notFound = "";
        static string fileContent = "";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var existingWords = SkipExistingWords();

            var wordList = File.ReadAllLines($"GoldenDict-{countryCode}.txt").ToList();
            if (application == "fiszkoteka")
                audioUploader.OpenBrowser("https://context.reverso.net");
            fileContent = ProcessWords(wordList, existingWords, processed, toUpload, fileContent, ref notFound);

            if (File.Exists("memrise.csv"))
                File.Delete("memrise.csv");

            //File.WriteAllText("memrise.csv", fileContent, Encoding.GetEncoding(1250));
            File.WriteAllText("memrise.csv", fileContent, new UTF8Encoding(true));
            File.WriteAllText("notFound.csv", notFound, new UTF8Encoding(true));

            File.AppendAllLines($"C:\\Repos\\kacper-mazur\\ebook2memrise\\ebook2memrise.generator\\Files\\Ready-{countryCode}.txt", toUpload);
            File.AppendAllLines($"C:\\Repos\\kacper-mazur\\ebook2memrise\\ebook2memrise.generator\\Files\\Ready-{countryCode}.txt", processed);

            if (application == "memrise")
                audioUploader.OpenBrowser(
                    countryCode == "es" ? "https://www.memrise.com/course/5712969/espanol-libros-peliculas/edit/"
                        : (countryCode == "ru" ? "https://www.memrise.com/course/5602608/russkii-knigi-filmy/edit"
                            : "https://www.memrise.com/course/5659032/english-books-movies/edit"));

            //System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Notepad++\notepad++.exe", new FileInfo("memrise.csv").FullName);
            System.Diagnostics.Process.Start(@"C:\Program Files\Microsoft Office\root\Office16\EXCEL.EXE", new FileInfo("memrise.csv").FullName);

            //toUpload.AddRange(new []{ });
            if (application == "memrise")
                notFound = audioUploader.Upload(toUpload, countryCode);
            if (!string.IsNullOrEmpty(notFound))
            {
                File.WriteAllText("notFound.csv", notFound);
                System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Notepad++\notepad++.exe",
                    new FileInfo("notFound.csv").FullName);
            }
        }

        private static string ProcessWords(List<string> wordList, IList<string> existingWords, List<string> processed, List<string> toUpload,
            string fileContent, ref string notFound)
        {
            foreach (var word in wordList)
            {
                Console.WriteLine("Start processing: " + word);
                using (var client = new CookieAwareWebClient())
                {
                    try
                    {
                        var data =
                            client.DownloadData("https://www.dict.com/" + GetLanguagePair() + "/" + word);

                        var response = Encoding.UTF8.GetString(data);
                        var localWord = dictProcessor.Process(word, response, out var definition, out var examples);
                        localWord = localWord.Replace("1", "").Replace("*", "").Trim();
                        Console.WriteLine("Translation downloaded for: " + localWord);

                        if (existingWords.Contains(localWord))
                        {
                            Console.WriteLine("Duplicate: " + localWord);
                            continue;
                        }

                        if (application == "fiszkoteka")
                        {
                            examples = audioUploader.GetExample("https://context.reverso.net/translation/" + GetLanguagePair() + "/" + localWord, countryCode);
                        }

                        processed.Add(word);
                        if (!toUpload.Contains(localWord))
                        {
                            fileContent += localWord + ";" + definition + ";" + examples + "\r\n";

                            //var localWord = word;
                            if (application == "memrise")
                            {
                                Console.WriteLine("Downloading audio for: " + localWord);
                                DownloadAudio(client, localWord);
                            }
                            toUpload.Add(localWord);
                        }
                        else
                        {
                            //ignore :)
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("The remote server returned an error: (404) Not Found."))
                            continue;
                        notFound += word + "\r\n";
                    }

                    if (toUpload.Count >= wordsToProcess)
                    {
                        var index = wordList.IndexOf(word);
                        var waitingWords = wordList.Skip(index + 1);

                        if (File.Exists(
                            $"C:\\Repos\\kacper-mazur\\ebook2memrise\\ebook2memrise.generator\\GoldenDict-{countryCode}.txt"))
                            File.Delete(
                                $"C:\\Repos\\kacper-mazur\\ebook2memrise\\ebook2memrise.generator\\GoldenDict-{countryCode}.txt");

                        File.WriteAllLines(
                            $"C:\\Repos\\kacper-mazur\\ebook2memrise\\ebook2memrise.generator\\GoldenDict-{countryCode}.txt",
                            waitingWords);

                        break;
                    }
                }
            }

            return fileContent;
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
                Console.Error.WriteLine("Error while downloading audio for: " + localWord);
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

        static IList<string> SkipExistingWords()
        {
            var wordlist = File.ReadAllLines(@"GoldenDict-" + countryCode + ".txt").ToList();
            var existing = File.ReadAllLines(@"Files\\Ready-" + countryCode + ".txt").ToList();

            wordlist = wordlist.ConvertAll(d => d.ToLower());
            existing = existing.ConvertAll(d => d.ToLower());

            Console.WriteLine("Total words to process: " + wordlist.Count);
            wordlist = wordlist.Where(w => !existing.Contains(w)).ToList();

            var duplicates = wordlist
                .GroupBy(v => v)
                .ToDictionary(g => g.Key, v => v.Count())
                .Where(v => v.Value > 1)
                .Select(v => v.Key);

            wordlist = wordlist.Where(w => !duplicates.Contains(w)).ToList();
            wordlist.AddRange(duplicates);

            Console.WriteLine("After skipping already processed and duplicated: " + wordlist.Count);

            File.Delete("GoldenDict-history.txt");
            File.WriteAllLines("GoldenDict-" + countryCode + ".txt", wordlist);
            return existing;
        }
    }
}
