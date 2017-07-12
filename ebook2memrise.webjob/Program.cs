using ebook2memrise.model;
using ebook2memrise.webjob.Logic;
using System.Linq;

namespace ebook2memrise.webjob
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var wordsTranslator = new WordsTranslator();
            var fileWordsProcessor = new FileWordsProcessor();
            var wordsToFileExporter = new WordToFileExporter();
            var importWordsProcessor = new ImportKnownWordsProcessor();
            var removeDuplicatedProcessor = new RemoveDuplicatedProcessor();

            importWordsProcessor.Process();
            removeDuplicatedProcessor.Process();
            //fileWordsProcessor.Process();
            var translations = wordsTranslator.Process();
            wordsToFileExporter.Process(translations, "destination.txt");
        }
    }
}
