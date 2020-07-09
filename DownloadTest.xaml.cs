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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;
using System.Net;
using Microsoft.VisualBasic.FileIO;
using System.Security.Policy;
using System.Windows.Controls.Primitives;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

// Simply a GitHub test

namespace TaskMaster
{
    /// <summary>
    /// Interaction logic for DownloadTest.xaml
    /// </summary>
    public partial class DownloadTest : Page
    {
        private int lineCount;
        private string downloadErrors = "";

        public int GetLineCount()
        {
            return this.lineCount;
        }

        public void SetLineCount(int n)
        {
            this.lineCount = n;
        }

        public string GetDownloadErrors()
        {
            return this.downloadErrors;
        }

        public void SetDownloadErrors(string errors)
        {
            this.downloadErrors = errors;
        }

        public DownloadTest()
        {
            InitializeComponent();
        }

        private void Select_File(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension
            dlg.Filter = "CSV (.csv)|*.csv"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                csvFile.Text = filename;
                csvFile.ToolTip = filename;
            }

            int lineCount = 0;
            using (TextFieldParser parser = new TextFieldParser(csvFile.Text))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    string newLine = parser.ReadLine();
                    if (!(newLine.StartsWith(",") || newLine == "" || newLine == null))
                    {
                        lineCount++;
                    }
                }

            }


            DownloadStatus.Text = "Total Files to Download: " + lineCount.ToString();
            SetLineCount(lineCount);
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

        private async void Start_Downloads(object sender, RoutedEventArgs e)
        {
            SubmitButton.IsEnabled = false;
            DownloadStatus.Text = "";
            Status.Text = "In Progress - DO NOT EXIT THIS PROGRAM";
            DownloadStatus.Text = "";
            bool fileExists = true;
            string fileError = "";
            string csvfile = csvFile.Text;
            int o = await Task.Run(() =>
            {
                try
                {
                    int lineCount = 0;
                    using (TextFieldParser parser = new TextFieldParser(csvfile))
                    {
                        parser.TextFieldType = FieldType.Delimited;
                        parser.SetDelimiters(",");
                        while (!parser.EndOfData)
                        {
                            string newLine = parser.ReadLine();
                            if (!(newLine.StartsWith(",") || newLine == ""))
                            {
                                lineCount++;
                            }
                        }

                    }
                    SetLineCount(lineCount);
                }
                catch
                {
                    fileExists = false;
                    fileError = "The selected File could not be found or opened. Please select a valid file";
                    return 0;
                }
                return 0;
            });

            DirectoryInfo dirInfo;
            try
            {
                foreach (var file in Directory.GetFiles(dirDownload.Text))
                { }
                dirInfo = new DirectoryInfo(dirDownload.Text);
            }
            catch
            {
                fileExists = false;
                fileError = "The directory does not exist - please select another directory or create this one";
                Status.Text = fileError;
                return;
            }

            if (!fileExists)
            {
                Status.Text = fileError;
                return;
            }
            ExportErrors.Visibility = Visibility.Hidden;
            using (TextFieldParser parser = new TextFieldParser(csvFile.Text))
            {
                int successcount = 0;
                int failcount = 0;
                string errors = "";
                string downloadedCount = "";
                string failedCount = "";
                string lines = GetLineCount().ToString();
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //Process row
                    string[] fields = parser.ReadFields();
                    string targetfile = dirDownload.Text;
                    int n = await Task.Run(() =>
                    {
                        using (var wc = new WebClient())
                        {
                            string str = fields[0];
                            if (!(str.StartsWith("http")))
                            {
                                str = "http://" + str;
                            }
                            var fileName = targetfile + @"\" + fields[1];

                            try
                            {
                                wc.DownloadFile(str, fileName);
                                successcount++;
                            }
                            catch
                            {
                                if (!(fields[0] == "" && fields[1] == ""))
                                {
                                    errors += fields[0] + "," + fields[1] + Environment.NewLine;
                                    failcount++;
                                }
                            }
                            
                        }
                        return 0;

                    });

                    downloadedCount = successcount.ToString();
                    failedCount = failcount.ToString();
                    DownloadStatus.Text = downloadedCount + " of " + lines + " files successfully downloaded\r\n" +
                        failedCount + " of " + lines + " files failed to download";
                }

                Status.Text = "Finished!!!";
                SubmitButton.IsEnabled = true;
                int downloadedCountNum = Int32.Parse(downloadedCount);
                int linesNum = Int32.Parse(lines);

                if (downloadedCountNum == linesNum)
                {
                    DownloadStatus.Text = downloadedCount + " of " + lines + " files were successfully downloaded.";
                }
                else
                {
                    DownloadStatus.Text = downloadedCount + " of " + lines + " files were successfully downloaded.\r\n" + failedCount + " files could not be downloaded. Click 'Export Errors' to view them.";
                    SetDownloadErrors(errors);
                    ExportErrors.Visibility = Visibility.Visible;
                }

            }
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyMMddHHmmss");
        }

        private void Save_Errors(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            int slashLocation = csvFile.Text.LastIndexOf(@"\");

            dlg.Filter = "CSV(*.csv)|*.csv";
            dlg.FileName = GetTimestamp(DateTime.Now) + "_errors_" + csvFile.Text.Substring(slashLocation + 1);
            dlg.InitialDirectory = csvFile.Text.Substring(0, slashLocation);

            if (dlg.ShowDialog() == true)
            {
                File.WriteAllText(dlg.FileName, GetDownloadErrors());
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
