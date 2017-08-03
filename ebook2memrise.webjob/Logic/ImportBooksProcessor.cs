using ebook2memrise.model;
using ebook2memrise.webjob.Model;
using System.IO;

namespace ebook2memrise.webjob.Logic
{
    public class ImportBooksProcessor
    {
        public void Process()
        {
            foreach (var fileName in Directory.EnumerateFiles(Constants.ImportBooksDirectory))
                using (var context = new ebook2memriseEntities())
                {
                    context.file_raw.Add(new file_raw() { file_content = File.ReadAllBytes(fileName), file_name = fileName });
                    context.SaveChanges();
                    File.Delete(fileName);
                }
        }
    }
}
