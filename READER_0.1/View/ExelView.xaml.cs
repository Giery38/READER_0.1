using System;
using System.Collections.Generic;
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

        private void ListFiles_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            DropFileCommand?.Execute(files);
        }
    }
}
