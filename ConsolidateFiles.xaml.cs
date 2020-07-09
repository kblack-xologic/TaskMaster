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
using WinForms = System.Windows.Forms;

namespace TaskMaster
{
    /// <summary>
    /// Interaction logic for ConsolidateFiles.xaml
    /// </summary>
    public partial class ConsolidateFiles : Page
    {
        public ConsolidateFiles()
        {
            InitializeComponent();
        }

        private void Select_Parent_Dir(object sender, RoutedEventArgs e)
        {
            WinForms.FolderBrowserDialog folderDialog = new WinForms.FolderBrowserDialog();
            WinForms.DialogResult result = folderDialog.ShowDialog();

            if (result == WinForms.DialogResult.OK)
            {
                string sPath = folderDialog.SelectedPath;
                parentDir.Text = sPath;
                parentDir.ToolTip = sPath;
            }
        }

        private void Select_Final_Dir(object sender, RoutedEventArgs e)
        {
            WinForms.FolderBrowserDialog folderDialog = new WinForms.FolderBrowserDialog();
            WinForms.DialogResult result = folderDialog.ShowDialog();

            if (result == WinForms.DialogResult.OK)
            {
                string sPath = folderDialog.SelectedPath;
                finalDir.Text = sPath;
                finalDir.ToolTip = sPath;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            this.Title = button.Content.ToString();
        }

        private async void Start_Consolidating(object sender, RoutedEventArgs e)
        {
            consolidateReport.Text = "";
            consolidateStatus.Text = "In Progress... Do not exit the program";
            string finaldir = finalDir.Text;
            string parentdir = parentDir.Text;
            bool fileExists = true;
            string fileError = "";
            int count = 0;
            if (Copy.IsChecked == true)
            {
                int n = await Task.Run(() =>
                {
                    DirectoryInfo dirInfo;
                    try
                    {
                        foreach (var file in Directory.GetFiles(finaldir))
                        { }
                        dirInfo = new DirectoryInfo(finaldir);
                    }
                    catch
                    {
                        fileExists = false;
                        fileError = "The final directory does not exist - please select another directory or create this one";
                        return 0;
                    }
                    try
                    {
                        List<String> allFiles = Directory.GetFiles(parentdir, "*.*", SearchOption.AllDirectories).ToList();

                        foreach (string file in allFiles)
                        {
                            FileInfo mFile = new FileInfo(file);
                            if (new FileInfo(dirInfo + @"\" + mFile.Name).Exists == false)
                            {
                                mFile.CopyTo(dirInfo + @"\" + mFile.Name);
                                count++;
                            }
                        }
                    }
                    catch
                    {
                        fileExists = false;
                        fileError = "The parent directory does not exist - please select another directory";
                        return 0;
                    }
                    return 0;
                });

                if (fileExists)
                {
                    string consolidateCount = count.ToString();
                    consolidateStatus.Text = "Finished!!!";
                    consolidateReport.Text = consolidateCount + " files have been COPIED to the specified directory - Files can still be found in their original locations";
                }
                else
                {
                    consolidateStatus.Text = fileError;
                }
            }
            else
            {
                int n = await Task.Run(() =>
                {
                    DirectoryInfo dirInfo;
                    try
                    {
                        foreach (var file in Directory.GetFiles(finaldir))
                        { }
                        dirInfo = new DirectoryInfo(finaldir);
                    }
                    catch
                    {
                        fileExists = false;
                        fileError = "The final directory does not exist - please select another directory or create this one";
                        return 0;
                    }
                    try
                    {
                        List<String> allFiles = Directory.GetFiles(parentdir, "*.*", SearchOption.AllDirectories).ToList();

                        foreach (string file in allFiles)
                        {
                            FileInfo mFile = new FileInfo(file);
                            if (new FileInfo(dirInfo + @"\" + mFile.Name).Exists == false)
                            {
                                mFile.MoveTo(dirInfo + @"\" + mFile.Name);
                                count++;
                            }
                        }
                    }
                    catch
                    {
                        fileExists = false;
                        fileError = "The parent directory does not exist - please select another directory";
                        return 0;
                    }
                    return 0;
                });
                if (fileExists)
                {
                    string consolidateCount = count.ToString();
                    consolidateStatus.Text = "Finished!!!";
                    consolidateReport.Text = consolidateCount + " files have been MOVED to the specified directory - Files have been removed from their original locations";
                }
                else
                {
                    consolidateStatus.Text = fileError;
                }
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
