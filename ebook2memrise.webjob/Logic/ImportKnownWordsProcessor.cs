using ebook2memrise.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ebook2memrise.webjob.Logic
{
    public class ImportKnownWordsProcessor
    {
        public void Process()
        {
            if (!File.Exists("words.txt"))
                return;

            var words = File.ReadAllLines("words.txt");
            var list = new List<string>();

            using (var context = new ebook2memriseEntities())
            {
                var excluded = context.words.Select(w => w.word).ToList();
                foreach (var w in words)
                    if (w.Count() > 2 && !excluded.Contains(w))
                        list.Add(w);
                list = list.Distinct().ToList();
                for (int i = 0; i < list.Count; i += 100)
                {
                    context.words.AddRange(list.Skip(i).Take(100).Select(t => new words() { word = t, exported = true, translation = ""}));
                    context.SaveChanges();
                }
            }
        }
    }
}

