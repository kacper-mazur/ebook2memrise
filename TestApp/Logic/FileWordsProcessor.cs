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

            var tmp = new List<string>();
            foreach(var t in srcText)
            {
                // replace digits
                var tmpWord = Regex.Replace(t, @"[\d]", string.Empty);
                // replace non word characters
                tmpWord = Regex.Replace(tmpWord, @"[\W]", string.Empty);
                // to lower
                tmpWord = tmpWord.ToLower();
                if(tmpWord.Count() > 2)
                tmp.Add(tmpWord);
            }
            tmp = tmp.Distinct().ToList();
            return tmp;
        }
    }
}
