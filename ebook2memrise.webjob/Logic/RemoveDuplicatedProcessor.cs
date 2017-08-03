using ebook2memrise.model;
using System.Collections.Generic;
using System.Linq;

namespace ebook2memrise.webjob.Logic
{
    public class RemoveDuplicatedProcessor
    {
        public void Process()
        {
            using (var context = new ebook2memriseEntities())
            {
                var excluded = context.words.Select(r => r.word).ToList();
                for (int i = 0; i < excluded.Count; i += 100)
                {
                    var toExclude = excluded.Skip(i).Take(100);
                    var toDelete = context.raw_words.Where(r => toExclude.Contains(r.word));
                    context.raw_words.RemoveRange(toDelete);
                    context.SaveChanges();
                }

                var duplicates = context.raw_words.GroupBy(k => k.word).ToDictionary(k => k, k => k.Count()).Where(k => k.Value > 1).Select(k => k.Key.Key);
                var toRemove = new List<raw_words>();
                foreach (var duplicate in duplicates)
                {
                    toRemove.AddRange(context.raw_words.Where(r => r.word == duplicate).OrderBy(r=> r.Id).Skip(1));
                }
                context.raw_words.RemoveRange(toRemove);
                context.SaveChanges();
            }
        }
    }
}
