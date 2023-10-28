using READER_0._1.Model.Excel.Settings;
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
using System.Windows.Shapes;

namespace READER_0._1.View.Elements
{
    /// <summary>
    /// Логика взаимодействия для ExcelSettingsDialog.xaml
    /// </summary>
    public partial class ExcelSettingsDialog : Window
    {
        public static readonly DependencyProperty ExcelSettingsReadProperty = DependencyProperty.Register(nameof(ExcelSettingsRead), typeof(ExcelSettingsRead), typeof(ExcelSettingsDialog));
        public ExcelSettingsRead ExcelSettingsRead
        {
            get { return (ExcelSettingsRead)GetValue(ExcelSettingsReadProperty); }
            set { SetValue(ExcelSettingsReadProperty, value); }
        }
        public ExcelSettingsDialog()
        {
            ExcelSettingsRead = new ExcelSettingsRead();            
            InitializeComponent();
        }       
    }
}
