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
    /// Interaction logic for ExcelHelp.xaml
    /// </summary>
    public partial class ExcelHelp : Page
    {
        public ExcelHelp()
        {
            InitializeComponent();
        }

        private void Go_To_MultiTabbedData(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MultiTabbedData.xaml", UriKind.Relative));
        }

        private void Go_To_MultiTabbedTest(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MultiTabbedTest.xaml", UriKind.Relative));
        }
    }
}
