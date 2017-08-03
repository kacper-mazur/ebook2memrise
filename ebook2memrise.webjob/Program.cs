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
            var importKnownWordsProcessor = new ImportKnownWordsProcessor();
            var removeDuplicatedProcessor = new RemoveDuplicatedProcessor();
            var importBooksProcessor = new ImportBooksProcessor();

            importBooksProcessor.Process();
            importKnownWordsProcessor.Process();
            removeDuplicatedProcessor.Process();
            fileWordsProcessor.Process();
            var translations = wordsTranslator.Process();
            wordsToFileExporter.Process(translations, "memoirs.txt");
        }
    }
}
