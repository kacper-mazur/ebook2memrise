using ebook2memrise.model;
using System.Linq;

namespace ebook2memrise.webjob
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var context = new ebook2memriseEntities())
            {
                if (context.configuration.Count() == 0)
                {
                    context.configuration.Add(new configuration() { key = "test", value = "test" });
                    context.SaveChanges();
                }
            }
        }
    }
}
