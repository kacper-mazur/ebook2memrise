using Google.Apis.Services;
using Google.Apis.Translate.v2;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            if (!File.Exists(tbSource.Text) || !Directory.Exists(System.IO.Path.GetDirectoryName(tbDestination.Text)))
                return;
            var srcText = File.ReadAllText(tbSource.Text).Split(new[] { ' ', ',', '.', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            var service = new TranslateService(new BaseClientService.Initializer()
            {
                ApiKey = File.ReadAllText("key.txt"),
                ApplicationName = "Translate API Sample"
            });

            var response = service.Translations.List(srcText, "pl").Execute();
            var translations = new Dictionary<string,string>();

            for(int i =0;i<response.Translations.Count;++i)
            {
                translations.Add(srcText[i], response.Translations[i].TranslatedText);
            }
            File.WriteAllText(tbDestination.Text, string.Join(Environment.NewLine, translations.Select(d => d.Key + ";" + d.Value)));
        }
    }
}
