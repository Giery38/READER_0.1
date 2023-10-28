using READER_0._1.View.Tools;
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

namespace READER_0._1.View
{
    /// <summary>
    /// Логика взаимодействия для WordView.xaml
    /// </summary>
    public partial class WordView : UserControl
    {
        public WordView()
        {
            InitializeComponent();
            Loaded += WordView_Loaded;
        }

        private void WordView_Loaded(object sender, RoutedEventArgs e)
        {
                       
        }

        private void CustomButton_MouseLeave(object sender, MouseEventArgs e)
        {
            
        }
    }
}
