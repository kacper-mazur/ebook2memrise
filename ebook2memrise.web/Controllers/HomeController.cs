using ebook2memrise.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ebook2memrise.web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                var fileName = Path.GetFileName(file.FileName);
                var fileContent = StreamToString(file.InputStream);
                using (var context = new ebook2memriseEntities())
                {
                    context.file_raw.Add(new file_raw() { file_content = Encoding.UTF8.GetBytes(fileContent), file_name = fileName });
                    context.SaveChanges();
                }
            } 

            return RedirectToAction("Index");
        }

        public static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public static Stream StringToStream(string src)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(src);
            return new MemoryStream(byteArray);
        }
    }
}
