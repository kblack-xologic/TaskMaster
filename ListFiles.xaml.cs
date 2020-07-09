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
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;

namespace TaskMaster
{
    /// <summary>
    /// Interaction logic for ListFiles.xaml
    /// </summary>
    public partial class ListFiles : Page
    {
        private string filelist;

        public string GetFileList()
        {
            return this.filelist;
        }

        public void SetFileList(string files)
        {
            this.filelist = files;
        }


        public ListFiles()
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

        private async void Start_Listing(object sender, RoutedEventArgs e)
        {
            ExportList.Visibility = Visibility.Hidden;
            bool fileExists = true;
            ListReport.Text = "";
            ListingStatus.Text = "In Progress... Do not exit the program";
            int count = 0;
            string targetFile = dirDownload.Text;
            string fileList = "";
            string fileName = "";
            int n = await Task.Run(() =>
            {
                try
                {
                    string[] fileEntries = Directory.GetFiles(targetFile);
                    foreach (string fullPath in fileEntries)
                    {
                        fileName = fullPath.Substring(fullPath.LastIndexOf(@"\") + 1);

                        fileList += fileName + Environment.NewLine;
                        count++;
                    }
                }
                catch
                {
                    fileExists = false;
                    return 0;
                }
                return 0;
            });

            if (fileExists)
            {
                ListingStatus.Text = "Finished!!!";
                string fileCount = count.ToString();
                SetFileList(fileList);
                ListReport.Text = fileCount + " files listed. Export the list by clicking the button to the right.";
                ExportList.Visibility = Visibility.Visible;
            }
            else
            {
                ListingStatus.Text = "The selected directory does not exist - please select another directory";
            }
        }

        private void Save_List(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.Filter = "CSV(*.csv)|*.csv";
            dlg.FileName = "00_fileList.csv";
            dlg.InitialDirectory = dirDownload.Text;

            if (dlg.ShowDialog() == true)
            {
                File.WriteAllText(dlg.FileName, GetFileList());
            }
        }

        private void Help_Box(object sender, RoutedEventArgs e)
        {
            PopupHelp.IsOpen = true;
        }

        private void Hide_Help(object sender, RoutedEventArgs e)
        {
            PopupHelp.IsOpen = false;
        }

        
    }
}
