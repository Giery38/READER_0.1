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

namespace READER_0._1.View.Elements
{
    /// <summary>
    /// Логика взаимодействия для CustomCheakBox.xaml
    /// </summary>
    public partial class CustomCheakBox : UserControl
    {
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(CustomCheakBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set
            {
                SetValue(IsCheckedProperty, value);               
            }
        }
        public static readonly DependencyProperty IsFocusableProperty = DependencyProperty.Register(nameof(IsFocusable), typeof(bool), typeof(CustomCheakBox), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsFocusable
        {
            get { return (bool)GetValue(IsFocusableProperty); }
            set
            {
                SetValue(IsFocusableProperty, value);
            }
        }
        public CustomCheakBox()
        {
            InitializeComponent();
        }

        private void Circle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsChecked = !IsChecked;
            if (IsChecked == true)
            {
                IsFocusable = false;
            }
        }                 
        private void Circle_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsChecked == false)
            {
                IsFocusable = true;
            }
        }

        private void Circle_MouseLeave(object sender, MouseEventArgs e)
        {
            IsFocusable = false;
        }
    }
}
