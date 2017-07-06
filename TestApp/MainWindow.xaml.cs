using Microsoft.Win32;
using System.Windows;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSourceBrowse_Click(object sender, RoutedEventArgs e)
        {
            var fd = new OpenFileDialog();
            fd.AddExtension = true;
            fd.Filter = "Pliki tekstowe|*.txt";
            fd.Multiselect = false;
            if (fd.ShowDialog() == true)
            {
                tbSource.Text = fd.FileName;
            }
        }

        private void btnDestinationBrowse_Click(object sender, RoutedEventArgs e)
        {
            var fd = new SaveFileDialog();
            fd.AddExtension = true;
            fd.DefaultExt = "Pliki tekstowe|*.txt";
            fd.Filter = "Pliki tekstowe|*.txt";
            if (fd.ShowDialog() == true)
            {
                tbDestination.Text = fd.FileName;
            }
        }

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            var wordsTranslator = new WordsTranslator();
            var fileWordsProcessor = new FileWordsProcessor();
            var wordsToFileExporter = new WordToFileExporter();

            var words = fileWordsProcessor.Process(tbSource.Text, tbDestination.Text);
            var translations = wordsTranslator.Process(words);
            wordsToFileExporter.Process(translations, tbDestination.Text);
        }
    }
}
