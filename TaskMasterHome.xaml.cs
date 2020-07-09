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

namespace TaskMaster
{
    /// <summary>
    /// Interaction logic for TaskMasterHome.xaml
    /// </summary>
    public partial class TaskMasterHome : Page
    {
        public TaskMasterHome()
        {
            InitializeComponent();
        }

        private void Go_To_LowerCase(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/LowerCase.xaml", UriKind.Relative));
        }

        private void Go_To_ListFiles(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ListFiles.xaml", UriKind.Relative));
        }

        private void Go_To_ConsolidateFiles(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ConsolidateFiles.xaml", UriKind.Relative));
        }

        private void Go_To_DownloadURLs(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/DownloadURLs.xaml", UriKind.Relative));
        }


        private void Go_To_ImageConverter(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ImageConverter.xaml", UriKind.Relative));
        }

        private void Go_To_ExcelHelp(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ExcelHelp.xaml", UriKind.Relative));
        }

        private void Go_To_DownloadTest(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/DownloadTest.xaml", UriKind.Relative));
        }
    }
}
