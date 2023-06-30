using READER_0._1.Model.Exel.Settings;
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
    /// Логика взаимодействия для ExelSettingsDialog.xaml
    /// </summary>
    public partial class ExelSettingsDialog : Window
    {
        public static readonly DependencyProperty ExelSettingsReadProperty = DependencyProperty.Register(nameof(ExelSettingsRead), typeof(ExelSettingsRead), typeof(ExelSettingsDialog));
        public ExelSettingsRead ExelSettingsRead
        {
            get { return (ExelSettingsRead)GetValue(ExelSettingsReadProperty); }
            set { SetValue(ExelSettingsReadProperty, value); }
        }
        public ExelSettingsDialog()
        {
            ExelSettingsRead = new ExelSettingsRead();            
            InitializeComponent();
        }       
    }
}
