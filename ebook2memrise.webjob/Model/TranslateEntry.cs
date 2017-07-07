using System.Collections.Generic;

namespace ebook2memrise.webjob.Model
{
    public class TranslateTextResponseTranslation
    {
        public string detectedSourceLanguage { get; set; }
        public string model { get; set; }
        public string translatedText { get; set; }
    }

    public class Response
    {
        public IList<TranslateTextResponseTranslation> translations { get; set; }
    }

    public class TranslateEntry
    {
        public Response data { get; set; }
    }
}
