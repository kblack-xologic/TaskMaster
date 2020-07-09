using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;
using System.Net;
using Microsoft.VisualBasic.FileIO;
using System.Security.Policy;

namespace TaskMaster
{
    /// <summary>
    /// Interaction logic for ImageScraper.xaml
    /// </summary>
    public partial class ImageScraper : Page
    {
        public ImageScraper()
        {
            InitializeComponent();
        }

        private void Select_Dir(object sender, RoutedEventArgs e)
        {
            WinForms.FolderBrowserDialog folderDialog = new WinForms.FolderBrowserDialog();
            WinForms.DialogResult result = folderDialog.ShowDialog();

            if (result == WinForms.DialogResult.OK)
            {
                string sPath = folderDialog.SelectedPath;
                dirDownload.Text = sPath;
                dirDownload.ToolTip = sPath;
            }
        }

        private void Start_Scraper(object sender, RoutedEventArgs e)
        {
            var url = fullURL.Text;
            GetWebpage(url);
        }

        private void GetWebpage(string url)
        {
            System.Windows.Forms.WebBrowser browser = new System.Windows.Forms.WebBrowser();
            browser.Navigate(url);
            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);

        }

        void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var browser = (System.Windows.Forms.WebBrowser)sender;
            var client = new WebClient();
            foreach (var img in browser.Document.Images)
            {
                var image = img as HtmlElement;
                var src = image.GetAttribute("src").TrimEnd('/');
                if (!Uri.IsWellFormedUriString(src, UriKind.Absolute))
                {
                    src = string.Concat(browser.Document.Url.AbsoluteUri, "/", src);
                }

                //Append any path to filename as needed
                var filename = new string(src.Skip(src.LastIndexOf('/') + 1).ToArray());
                filename = dirDownload.Text + @"\" + filename;
                File.WriteAllBytes(filename, client.DownloadData(src));
            }
        }
    }
}
