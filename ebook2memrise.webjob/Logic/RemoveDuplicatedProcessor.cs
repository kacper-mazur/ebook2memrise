using ebook2memrise.model;
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
            }
        }
    }
}
