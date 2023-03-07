using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace READER_0._1.View
{
    /// <summary>
    /// Логика взаимодействия для ExelView.xaml
    /// </summary>
    public partial class ExelView : UserControl
    {
        public ExelView()
        {
            InitializeComponent();
        }       

        public static readonly DependencyProperty DropFileCommandProperty =
            DependencyProperty.RegisterAttached("DropFileCommand", typeof(ICommand), typeof(ExelView), new PropertyMetadata(null));
        public static readonly DependencyProperty ScrollTableCommandProperty =
           DependencyProperty.RegisterAttached("ScrollTableCommand", typeof(ICommand), typeof(ExelView), new PropertyMetadata(null));
        public static readonly DependencyProperty SizeChangeCommandProperty =
           DependencyProperty.RegisterAttached("SizeChangeCommand", typeof(ICommand), typeof(ExelView), new PropertyMetadata(null));
        public ICommand DropFileCommand
        {
            get
            {
                return (ICommand)GetValue(DropFileCommandProperty);
            }
            set
            {
                SetValue(DropFileCommandProperty, value);
            }
        }
        public ICommand ScrollTableCommand
        {
            get
            {
                return (ICommand)GetValue(ScrollTableCommandProperty);
            }
            set
            {
                SetValue(ScrollTableCommandProperty, value);
            }
        }
        public ICommand SizeChangeCommand
        {
            get
            {
                return (ICommand)GetValue(SizeChangeCommandProperty);
            }
            set
            {
                SetValue(SizeChangeCommandProperty, value);
            }
        }

        private void FolderFiles_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            /*
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            ListView listViewSender = (ListView)sender;
            Tuple<string, string[]> addedFiles = new Tuple<string, string[]>(listViewSender.Tag.ToString(), files); // первое тег, второе список файлов
            DropFileCommand?.Execute(addedFiles);
            */
            FileStream stream = File.Open(files[0], FileMode.Open, FileAccess.Read);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataTable dataTable = new DataTable();

            // Добавляем столбцы в DataTable
            dataTable.Columns.Add("Column1", typeof(string));
            dataTable.Columns.Add("Column2", typeof(int));
            dataTable.Columns.Add("Column3", typeof(double));

            // Создаем списки данных
            List<string> column1Data = new List<string>() { "Value1", "Value2", "Value3" };
            List<int> column2Data = new List<int>() { 1, 2, 3 };
            List<double> column3Data = new List<double>() { 1.1, 2.2, 3.3 };

            // Заполняем DataTable данными из списков
            for (int i = 0; i < column1Data.Count; i++)
            {
                DataRow row = dataTable.NewRow();
                row["Column1"] = column1Data[i];
                row["Column2"] = column2Data[i];
                row["Column3"] = column3Data[i];
                dataTable.Rows.Add(row);
            }
                        
            DataSet result = excelReader.AsDataSet();
            excelReader.Close();
            stream.Close();
            DataGrid myDataGrid = this.FindName("ghghghgh") as DataGrid;
            var tt = result.Tables[0].DefaultView;
            //myDataGrid.ItemsSource = result.Tables[0].DefaultView;
            myDataGrid.ItemsSource = dataTable.DefaultView;
        }
        private void Table_Scroll(object sender, ScrollChangedEventArgs e)
        {
            double verticalPosition = e.VerticalOffset;                
        }
        private void SizeTable_Change(object sender, SizeChangedEventArgs e)
        {                 
            SizeChangeCommand?.Execute(sender);
            ListView ss = new ListView();        
            
        }


    }
    public static class ScrollViewerBinding
    {
        /// <summary>
        /// VerticalOffset attached property
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset", typeof(double),
            typeof(ScrollViewerBinding), new FrameworkPropertyMetadata(double.NaN,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnVerticalOffsetPropertyChanged));

        /// <summary>
        /// Just a flag that the binding has been applied.
        /// </summary>
        private static readonly DependencyProperty VerticalScrollBindingProperty =
            DependencyProperty.RegisterAttached("VerticalScrollBinding", typeof(bool?), typeof(ScrollViewerBinding));

        public static double GetVerticalOffset(DependencyObject depObj)
        {
            return (double)depObj.GetValue(VerticalOffsetProperty);
        }

        public static void SetVerticalOffset(DependencyObject depObj, double value)
        {
            depObj.SetValue(VerticalOffsetProperty, value);
        }

        private static void OnVerticalOffsetPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = d as ScrollViewer;
            if (scrollViewer == null)
                return;

            BindVerticalOffset(scrollViewer);
            scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        public static void BindVerticalOffset(ScrollViewer scrollViewer)
        {
            if (scrollViewer.GetValue(VerticalScrollBindingProperty) != null)
                return;

            scrollViewer.SetValue(VerticalScrollBindingProperty, true);
            scrollViewer.ScrollChanged += (s, se) =>
            {
                if (se.VerticalChange == 0)
                    return;
                SetVerticalOffset(scrollViewer, se.VerticalOffset);
            };
        }
    }

    public static class ActualSizeBinding // https://stackoverflow.com/questions/63860555/canvas-actualwidth-and-actualheight-passed-into-viewmodel-in-mvvm-way
    {
        
        public static readonly DependencyProperty ActiveProperty = DependencyProperty.RegisterAttached(
            "Active",
            typeof(bool),
            typeof(ActualSizeBinding),
            new FrameworkPropertyMetadata(OnActualSizePropertyChanged));

        public static bool GetActive(FrameworkElement frameworkElement)
        {
            return (bool)frameworkElement.GetValue(ActiveProperty);
        }

        public static void SetActive(FrameworkElement frameworkElement, bool active)
        {
            frameworkElement.SetValue(ActiveProperty, active);
        }    

        public static readonly DependencyProperty BoundActualWidthProperty = DependencyProperty.RegisterAttached(
            "BoundActualWidth",
            typeof(double),
            typeof(ActualSizeBinding));

        public static double GetBoundActualWidth(FrameworkElement frameworkElement)
        {
            return (double)frameworkElement.GetValue(BoundActualWidthProperty);
        }

        public static void SetBoundActualWidth(FrameworkElement frameworkElement, double width)
        {
            frameworkElement.SetValue(BoundActualWidthProperty, width);
        }

        public static readonly DependencyProperty BoundActualHeightProperty = DependencyProperty.RegisterAttached(
            "BoundActualHeight",
            typeof(double),
            typeof(ActualSizeBinding));

        public static double GetBoundActualHeight(FrameworkElement frameworkElement)
        {
            return (double)frameworkElement.GetValue(BoundActualHeightProperty);
        }

        public static void SetBoundActualHeight(FrameworkElement frameworkElement, double height)
        {
            frameworkElement.SetValue(BoundActualHeightProperty, height);
        }
      
        private static void OnActualSizePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (!(dependencyObject is FrameworkElement frameworkElement))
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                frameworkElement.SizeChanged += OnFrameworkElementSizeChanged;
                UpdateObservedSizesForFrameworkElement(frameworkElement);
            }
            else
            {
                frameworkElement.SizeChanged -= OnFrameworkElementSizeChanged;
            }         
        }

        private static void OnFrameworkElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is FrameworkElement frameworkElement)
            {
                UpdateObservedSizesForFrameworkElement(frameworkElement);
            }
        }

        private static void UpdateObservedSizesForFrameworkElement(FrameworkElement frameworkElement)
        {
            frameworkElement.SetCurrentValue(BoundActualWidthProperty, frameworkElement.ActualWidth);
            frameworkElement.SetCurrentValue(BoundActualHeightProperty, frameworkElement.ActualHeight);
        }
    }
      
}
