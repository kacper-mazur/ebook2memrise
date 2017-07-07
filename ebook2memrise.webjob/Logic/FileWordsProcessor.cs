﻿using ebook2memrise.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ebook2memrise.webjob.Logic
{
    public class FileWordsProcessor
    {
        public IList<string> Process()
        {
            var fileContent = "";
            var fileID = -1;
            var excluded = new List<string>();

            using (var context = new ebook2memriseEntities())
            {
                if (!context.file_raw.Any())
                    return null;
                var file = context.file_raw.First();
                fileContent = System.Text.Encoding.Default.GetString(file.file_content);
                fileID = file.Id;

                excluded = context.words.Select(r => r.word).ToList();

                var srcText = fileContent.Split(new[] { ' ', ',', '.', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                var tmp = new List<string>();

                for (int i = 0; i < srcText.Count(); ++i)
                {
                    // replace digits
                    var tmpWord = Regex.Replace(srcText[i], @"[\d]", string.Empty);
                    // replace non word characters (except - ')
                    tmpWord = Regex.Replace(tmpWord, @"[+*\\.,\/;:'\[\]|}{<>""@#$%^&*()_+=`~]*", string.Empty);
                    // to lower
                    tmpWord = tmpWord.ToLower().Trim();
                    if (tmpWord.Count() > 2 && !excluded.Contains(tmpWord))
                        tmp.Add(tmpWord);
                }
                tmp = tmp.Distinct().ToList();
                for (int i = 0; i < tmp.Count; i += 100)
                {
                    context.raw_words.AddRange(tmp.Skip(i).Take(100).Select(t => new raw_words() { word = t, exported = false, translated = false }));
                    context.SaveChanges();
                }
                context.file_raw.Remove(file);
                context.SaveChanges();
            }
            return null;
        }
    }
}
