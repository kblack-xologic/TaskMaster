using Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
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
using System.Windows.Threading;
using Excel = Microsoft.Office.Interop.Excel;

namespace TaskMaster
{
    /// <summary>
    /// Interaction logic for MultiTabbedData.xaml
    /// </summary>
    public partial class MultiTabbedData : System.Windows.Controls.Page
    {
        private static Excel.Workbook MyBook = null;
        private static Excel.Application MyApp = null;
        private static Excel.Worksheet MySheet = null;

        public MultiTabbedData()
        {
            InitializeComponent();
        }

        private void Select_File(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            //dlg.DefaultExt = ".xlsx"; // Default file extension
            //dlg.Filter = "Excel Workbook (*.xlsx)|*.xls"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                excelFile.Text = filename;
                excelFile.ToolTip = filename;
            }
        }

        private void Start_Consolidating(object sender, RoutedEventArgs e)
        {
            string itemNumHeader = ItemNum.Text;


            Dispatcher.Invoke(new System.Action(() =>
            {
                Status.Text = "In Progress... Please do not close the program";
            }), DispatcherPriority.ContextIdle);

            MyApp = new Excel.Application();
            MyApp.Visible = false;
            var WorkBooks = MyApp.Workbooks;
            MyBook = WorkBooks.Open(excelFile.Text);

            int itemCount = 0;
            int headerCount = 0;
            var WorkSheets = MyBook.Worksheets;
            int numSheets = WorkSheets.Count;
            string saveAs = "";

            List<int> LastCol = new List<int>();
            List<int> LastRow = new List<int>();
            List<int> ItemIndexStart = new List<int>();
            List<string> AllHeaders = new List<string>();
            List<int> AllHeadersIndexStart = new List<int>();
            List<int> ItemNumberColPos = new List<int>();
            List<string> AllItemNumbers = new List<string>();
            List<List<List<string>>> myDataSet = new List<List<List<string>>>();

            //Getting Unique Header Values, Item Numbers, and Column and Row Counts
            for (int i = 1; i <= numSheets; i++)
            {
                MySheet = (Excel.Worksheet)MyBook.Sheets[i];
                var MySheetC = MySheet.Cells;
                //var cellValue = "";
                var MySheetSC = MySheetC.SpecialCells(XlCellType.xlCellTypeLastCell, Type.Missing);
                int lastCol = MySheetSC.Column;
                int lastRow = MySheetSC.Row;

                LastCol.Add(lastCol);
                LastRow.Add(lastRow);
                AllHeadersIndexStart.Add(headerCount);
                List<List<string>> myDataSetSheet = new List<List<string>>();

                for (int j = 1; j <= lastCol; j++)
                {
                    var cell = (MySheetC[1, j] as Excel.Range).Value;
                    if (cell == null)
                    {
                        AllHeaders.Add("");
                    }
                    else
                    {
                        AllHeaders.Add(cell.ToString());
                    }
                    List<string> myDataSetColumn = new List<string>();

                    for (int k = 1; k <= lastRow; k++)
                    {
                        var cellValue = (MySheetC[k, j] as Excel.Range).Value;
                        if (cellValue == null)
                        {
                            myDataSetColumn.Add("");
                        }
                        else
                        {
                            myDataSetColumn.Add(cellValue.ToString());
                        }
                    }
                    myDataSetSheet.Add(myDataSetColumn);
                    if (cell == null)
                    {
                    }
                    else
                    {
                        cell = cell.ToString();
                        if (cell == itemNumHeader)
                        {
                            ItemIndexStart.Add(itemCount);
                            ItemNumberColPos.Add(j);
                            for (int m = 2; m <= lastRow; m++)
                            {
                                var itemNumCell = (MySheetC[m, j] as Excel.Range).Value;
                                if (itemNumCell == null)
                                {
                                    AllItemNumbers.Add("");
                                }
                                else
                                {
                                    AllItemNumbers.Add(itemNumCell.ToString());
                                }
                                itemCount++;
                            }
                        }
                    }
                    headerCount++;
                }
                myDataSet.Add(myDataSetSheet);
            }

            List<string> Headers = new List<string>();
            int HeadersCount = Headers.Count;
            bool exists;
            for (int i = 0; i < AllHeaders.Count; i++)
            {
                exists = false;
                for (int j = 0; (j <= i) && (j < Headers.Count); j++)
                {
                    bool testThis = AllHeaders[i] == Headers[j];
                    if (testThis)
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    Headers.Add(AllHeaders[i]);
                }
            }

            LastCol.Insert(0, Headers.Count);

            //Creating new WorkSheet and filling in the unique Headers
            Excel.Worksheet newWorkSheet = WorkSheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            LastRow.Insert(0, itemCount + 1);
            ItemIndexStart.Insert(0, -1);
            AllHeadersIndexStart.Insert(0, -1);
            numSheets++;
            int numHeaders = Headers.Count();

            for (int i = 1; i <= numHeaders; i++)
            {
                newWorkSheet.Cells[1, i] = Headers[i - 1];

                //Filling in all ItemNumbers
                if (Headers[i - 1] == itemNumHeader)
                {
                    ItemNumberColPos.Insert(0, i);
                    for (int j = 2; j <= itemCount + 1; j++)
                    {
                        newWorkSheet.Cells[j, i] = AllItemNumbers[j - 2];
                    }
                }
            }

            //Populating Cells based on Item Numbers and Headers
            //Worksheet containing the Item Number will be determined based on LastRow and AllItemNumber Lists.
            string currentItem = "";
            string currentHeader = "";
            int workSheetRow = 0;
            for (int i = 0; i < itemCount; i++)
            {
                currentItem = AllItemNumbers[i];
                for (int j = numSheets - 1; j >= 0; j--)
                {
                    if (ItemIndexStart[j] <= i)
                    {
                        workSheetRow = i - ItemIndexStart[j] + 2;
                        int colStartIndex = AllHeadersIndexStart[j];
                        for (int k = 1; k <= numHeaders; k++)
                        {
                            if (k == ItemNumberColPos[0])
                            {
                                continue;
                            }
                            currentHeader = Headers[k - 1];
                            for (int m = 1; m <= LastCol[j]; m++)
                            {
                                if (currentHeader == AllHeaders[colStartIndex + m - 1])
                                {
                                    newWorkSheet.Cells[i + 2, k] = myDataSet[j - 1][m - 1][workSheetRow - 1];
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
                if (i % 10 == 0)
                {
                    Dispatcher.Invoke(new System.Action(() =>
                    {
                        Report.Text = i.ToString() + " out of " + itemCount.ToString() + " items have been consolidated.... Please wait";
                    }), DispatcherPriority.ContextIdle);
                }
            }


            saveAs = @"C:\Users\kevin\Desktop\Development\Test\Tabbed Catalogs\" + GetTimestamp(DateTime.Now) + @"TestSave.xlsx";
            MyBook.SaveAs(saveAs);

            MyBook.Close(false, Type.Missing, Type.Missing);
            MyApp.Quit();

            releaseObject(MySheet);
            releaseObject(WorkSheets);
            releaseObject(numSheets);
            releaseObject(newWorkSheet);
            releaseObject(WorkBooks);
            releaseObject(MyBook);
            releaseObject(MyApp);
            MySheet = null;
            WorkSheets = null;
            newWorkSheet = null;
            WorkBooks = null;
            MyBook = null;
            MyApp = null;

            Status.Text = "Finished!!! You may close this program.";

            Report.Text = itemCount.ToString() + " items have been consolidated into one sheet." + Environment.NewLine + "The consolidated items can be found on \'Sheet 1\' of " + saveAs;
        }

        //private void Start_Consolidating(object sender, RoutedEventArgs e)
        //{
        //    string itemNumHeader = ItemNum.Text;


        //    Dispatcher.Invoke(new System.Action(() =>
        //    {
        //        Status.Text = "In Progress... Please do not close the program";
        //    }), DispatcherPriority.ContextIdle);

        //    MyApp = new Excel.Application();
        //    MyApp.Visible = false;
        //    var WorkBooks = MyApp.Workbooks;
        //    MyBook = WorkBooks.Open(excelFile.Text);

        //    int itemCount = 0;
        //    int headerCount = 0;
        //    var WorkSheets = MyBook.Worksheets;
        //    int numSheets = WorkSheets.Count;
        //    string saveAs = "";

        //    List<int> LastCol = new List<int>();
        //    List<int> LastRow = new List<int>();
        //    List<int> ItemIndexStart = new List<int>();
        //    List<string> AllHeaders = new List<string>();
        //    List<int> AllHeadersIndexStart = new List<int>();
        //    List<int> ItemNumberColPos = new List<int>();
        //    List<string> AllItemNumbers = new List<string>();

        //    //Getting Unique Header Values, Item Numbers, and Column and Row Counts
        //    for (int i = 1; i <= numSheets; i++)
        //    {

        //        MySheet = (Excel.Worksheet)MyBook.Sheets[i];
        //        var MySheetC = MySheet.Cells;
        //        var MySheetSC = MySheetC.SpecialCells(XlCellType.xlCellTypeLastCell, Type.Missing);
        //        int lastCol = MySheetSC.Column;
        //        int lastRow = MySheetSC.Row;

        //        LastCol.Add(lastCol);
        //        LastRow.Add(lastRow);
        //        AllHeadersIndexStart.Add(headerCount);

        //        for (int j = 1; j <= lastCol; j++)
        //        {
        //            var cell = (string)(MySheetC[1, j] as Excel.Range).Value;
        //            AllHeaders.Add(cell.ToString());

        //            if (cell == itemNumHeader)
        //            {
        //                ItemIndexStart.Add(itemCount);
        //                ItemNumberColPos.Add(j);
        //                for (int k = 2; k <= lastRow; k++)
        //                {
        //                    var itemNumCell = (string)(MySheetC[k, j] as Excel.Range).Value;
        //                    AllItemNumbers.Add(itemNumCell.ToString());
        //                    itemCount++;
        //                }
        //            }
        //            headerCount++;
        //        }
        //    }

        //    List<string> Headers = new List<string>();
        //    int HeadersCount = Headers.Count;
        //    bool exists;
        //    for (int i = 0; i < AllHeaders.Count; i++)
        //    {
        //        exists = false;
        //        for (int j = 0; (j <= i) && (j < Headers.Count); j++)
        //        {
        //            bool testThis = AllHeaders[i] == Headers[j];
        //            if (testThis)
        //            {
        //                exists = true;
        //                break;
        //            }
        //        }
        //        if (!exists)
        //        {
        //            Headers.Add(AllHeaders[i]);
        //        }
        //    }

        //    LastCol.Insert(0, Headers.Count);

        //    //Creating new WorkSheet and filling in the unique Headers
        //    Excel.Worksheet newWorkSheet = WorkSheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        //    LastRow.Insert(0, itemCount + 1);
        //    ItemIndexStart.Insert(0, -1);
        //    AllHeadersIndexStart.Insert(0, -1);
        //    numSheets++;
        //    int numHeaders = Headers.Count();

        //    for (int i = 1; i <= numHeaders; i++)
        //    {
        //        newWorkSheet.Cells[1, i] = Headers[i - 1];

        //        //Filling in all ItemNumbers
        //        if (Headers[i - 1] == itemNumHeader)
        //        {
        //            ItemNumberColPos.Insert(0, i);
        //            for (int j = 2; j <= itemCount + 1; j++)
        //            {
        //                newWorkSheet.Cells[j, i] = AllItemNumbers[j - 2];
        //            }
        //        }
        //    }

        //    //Populating Cells based on Item Numbers and Headers
        //    //Worksheet containing the Item Number will be determined based on LastRow and AllItemNumber Lists.
        //    string currentItem = "";
        //    string currentHeader = "";
        //    int workSheetRow = 0;
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        currentItem = AllItemNumbers[i];
        //        for (int j = numSheets - 1; j >= 0; j--)
        //        {
        //            if (ItemIndexStart[j] <= i)
        //            {
        //                MySheet = (Excel.Worksheet)MyBook.Sheets[j + 1];
        //                workSheetRow = i - ItemIndexStart[j] + 2;
        //                int colStartIndex = AllHeadersIndexStart[j];
        //                for (int k = 1; k <= numHeaders; k++)
        //                {
        //                    if (k == ItemNumberColPos[0])
        //                    {
        //                        continue;
        //                    }
        //                    currentHeader = Headers[k - 1];
        //                    for (int m = 1; m <= LastCol[j]; m++)
        //                    {
        //                        if (currentHeader == AllHeaders[colStartIndex + m - 1])
        //                        {
        //                            newWorkSheet.Cells[i + 2, k] = MySheet.Cells[workSheetRow, m];
        //                            break;
        //                        }
        //                    }
        //                }
        //                break;
        //            }
        //        }
        //        if (i % 10 == 0)
        //        {
        //            Dispatcher.Invoke(new System.Action(() =>
        //            {
        //                Report.Text = i.ToString() + " out of " + itemCount.ToString() + " items have been consolidated.... Please wait";
        //            }), DispatcherPriority.ContextIdle);
        //        }
        //    }


        //    saveAs = @"C:\Users\kevin\Desktop\Test\Tabbed Catalogs\" + GetTimestamp(DateTime.Now) + @"TestSave.xlsx";
        //    MyBook.SaveAs(saveAs);

        //    MyBook.Close(false, Type.Missing, Type.Missing);
        //    MyApp.Quit();

        //    releaseObject(MySheet);
        //    releaseObject(WorkSheets);
        //    releaseObject(numSheets);
        //    releaseObject(newWorkSheet);
        //    releaseObject(WorkBooks);
        //    releaseObject(MyBook);
        //    releaseObject(MyApp);
        //    MySheet = null;
        //    WorkSheets = null;
        //    newWorkSheet = null;
        //    WorkBooks = null;
        //    MyBook = null;
        //    MyApp = null;

        //    Status.Text = "Finished!!! You may close this program.";

        //    Report.Text = itemCount.ToString() + " items have been consolidated into one sheet." + Environment.NewLine + "The consolidated items can be found on \'Sheet 1\' of " + saveAs;
        //}

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in releasing object :" + ex);
                obj = null;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyMMddHHmmss");
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
