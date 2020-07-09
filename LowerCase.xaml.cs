using System;
using System.Collections.Generic;
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
using System.IO;

namespace TaskMaster
{
    /// <summary>
    /// Interaction logic for LowerCase.xaml
    /// </summary>
    public partial class LowerCase : Page
    {
        public LowerCase()
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
                LCDir.Text = sPath;
                LCDir.ToolTip = sPath;
            }
        }

        private async void Start_LowerCase(object sender, RoutedEventArgs e)
        {
            string targetFile = LCDir.Text;
            LCReport.Text = "";
            bool fileExists = true;

            LowerCaseStatus.Text = "In Progress... Do not exit the program";
            int count = 0;
            int n = await Task.Run(() =>
            {
                try
                {
                    foreach (var file in Directory.GetFiles(targetFile))
                    {
                        File.Move(file, file.ToLowerInvariant());
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
                string fileCount = count.ToString();
                LowerCaseStatus.Text = "Finished!!!";
                LCReport.Text = fileCount + " files/directories were lowercased";
            }
            else
            {
                LowerCaseStatus.Text = "The selected directory does not exist - please select another directory";
                return;
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
