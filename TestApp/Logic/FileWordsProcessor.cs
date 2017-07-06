using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestApp
{
    public class FileWordsProcessor
    {
        public IList<string> Process(string source, string target)
        {
            if (!File.Exists(source) || !Directory.Exists(System.IO.Path.GetDirectoryName(target)))
                return null;
            var srcText = File.ReadAllText(source).Split(new[] { ' ', ',', '.', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            var prepositions = new[] { "aback", "about", "across", "after", "against", "ahead", "along", "apart", "around", "aside", "at", "away", "back", "before", "behind", "between", "beyond", "by", "down", "for", "forth", "forward", "from", "in", "into", "of", "off", "on", "onto", "out", "over", "round", "through", "to", "together", "towards", "under", "up", "with", "without" };

            var tmp = new List<string>();

            for (int i = 0; i < srcText.Count(); ++i)
            {
                // replace digits
                var tmpWord = Regex.Replace(srcText[i], @"[\d]", string.Empty);
                // replace non word characters (except - ')
                tmpWord = Regex.Replace(tmpWord, @"[+*\\.,\/;:'\[\]|}{<>""@#$%^&*()_+=`~]*", string.Empty);
                // to lower
                tmpWord = tmpWord.ToLower().Trim();
                if (tmpWord.Count() > 2)
                    tmp.Add(tmpWord);

                // add potential phrasal verb
                if (i > 0 && prepositions.Contains(tmpWord))
                {
                    tmp.Add(srcText[i - 1] + " " + srcText[i]);
                }
            }
            tmp = tmp.Distinct().ToList();
            return tmp;
        }
    }
}
