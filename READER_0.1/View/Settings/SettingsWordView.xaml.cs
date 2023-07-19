using READER_0._1.View.Elements;
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

namespace READER_0._1.View.Settings
{
    /// <summary>
    /// Логика взаимодействия для SettingsWordView.xaml
    /// </summary>
    public partial class SettingsWordView : UserControl
    {
        public SettingsWordView()
        {
            InitializeComponent();
        }

        private void InputSearchStringsTextButton_Click(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(InputSearchStringsTextBox, 1);
            Panel.SetZIndex(SearchStringsList, 2);
        }

        private void CustomButton_Click(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(InputSearchStringsTextBox, 2);
            Panel.SetZIndex(SearchStringsList, 1);
        }
    }
}
