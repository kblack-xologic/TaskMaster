using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
    public partial class ImageConverter : Page
    {
        private string errors;

        public void SetErrors(string error)
        {
            this.errors += error;
        }

        public string GetErrors()
        {
            return this.errors;
        }

        public ImageConverter()
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
                ICdirSelect.Text = sPath;
                ICdirSelect.ToolTip = sPath;
            }
        }

        private async void Start_Converting(object sender, RoutedEventArgs e)
        {
            ICReport.Text = "";
            ICStatus.Text = "In Progress... Do not exit the program";
            ExportErrors.Visibility = Visibility.Hidden;

            string filetarget = ICdirSelect.Text;
            bool fileExists = true;
            string fileErrors = "";

            int o = await Task.Run(() =>
            {
                DirectoryInfo dirInfo;
                try
                {
                    foreach (var file in Directory.GetFiles(filetarget))
                    { }
                    dirInfo = new DirectoryInfo(filetarget);
                }
                catch
                {
                    fileExists = false;
                    fileErrors = "The directory does not exist - please select another directory";
                    return 0;
                }
                return 0;
            });

            if (!fileExists)
            {
                ICStatus.Text = fileErrors;
                return;
            }

            int count = 0;
            int successcount = 0;
            string successcountstring = "";
            int failcount = 0;
            string failcountstring = "";
            string convertedDir = @"\Converted_" + GetTimestamp(DateTime.Now);
            convertedDir = filetarget + convertedDir;
            Directory.CreateDirectory(convertedDir);
            string[] allFiles = Directory.GetFiles(filetarget);
            string[] validExtensions = new string[] { ".bmp", ".emf", ".exif", ".gif", ".guid", ".icon", ".jpg", ".jpeg", ".memorybmp", ".png", ".tiff", ".wmf" };

            string conversionType = ExtensionChoice.Text;
            string fileExtension = ".jpg";
            ImageFormat fileFormat = ImageFormat.Jpeg;
            switch (conversionType)
            {
                case "bmp":
                    fileFormat = ImageFormat.Bmp;
                    fileExtension = ".bmp";
                    break;

                case "gif":
                    fileFormat = ImageFormat.Gif;
                    fileExtension = ".gif";
                    break;

                case "jpg":
                    break;

                case "png":
                    fileFormat = ImageFormat.Png;
                    fileExtension = ".png";
                    break;

                case "tiff":
                    fileFormat = ImageFormat.Tiff;
                    fileExtension = ".tiff";
                    break;
            }
            
            
            foreach (string file in allFiles)
            {
                int n = await Task.Run(() =>
                {
                    string extension = System.IO.Path.GetExtension(file).ToLower();
                    if (validExtensions.Contains(System.IO.Path.GetExtension(file)))
                    {
                        count++;
                        string currentFile = file;
                        
                        try
                        {
                            Bitmap myBitmap = new Bitmap(currentFile);

                            ImageCodecInfo myImageCodecInfo = GetEncoder(fileFormat);
                            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                            EncoderParameters myEncoderParameters = new EncoderParameters(1);

                            myEncoderParameters.Param[0] = myEncoderParameter;
                            currentFile = currentFile.Replace(currentFile.Substring(currentFile.LastIndexOf(@".")), fileExtension);
                            string new_file = currentFile.Replace(filetarget, convertedDir);
                            myBitmap.Save(new_file, myImageCodecInfo, myEncoderParameters);

                            successcount++;
                        }
                        catch
                        {
                            string fileError = System.IO.Path.GetFileName(file) + Environment.NewLine;
                            SetErrors(fileError);
                            failcount++;
                        }
                        
                    }
                    return 0;
                });
                successcountstring = successcount.ToString();
                failcountstring = failcount.ToString();
                ICReport.Text = successcountstring + " of " + count + " files were successfully converted to " + fileExtension + ".\r\n" + failcountstring + " files have failed to convert.";
            }

            ICStatus.Text = "Finished!!!";
            successcountstring = successcount.ToString();
            failcountstring = failcount.ToString();
            ICReport.Text = successcount + " of " + count + " files were successfully converted to " + fileExtension + ".\r\n" + failcount + " files could not be converted. Click 'Export Errors' to view them.";

            if (failcount > 0)
            {
                ExportErrors.Visibility = Visibility.Visible;
            }
           }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyMMddHHmmss");
        }

        private void Save_Errors(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            int slashLocation = ICdirSelect.Text.LastIndexOf(@"\");

            dlg.Filter = "CSV(*.csv)|*.csv";
            dlg.FileName = GetTimestamp(DateTime.Now) + "_errors";
            dlg.InitialDirectory = ICdirSelect.Text;

            if (dlg.ShowDialog() == true)
            {
                File.WriteAllText(dlg.FileName, GetErrors());
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
