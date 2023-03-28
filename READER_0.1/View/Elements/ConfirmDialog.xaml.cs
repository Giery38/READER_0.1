using READER_0._1.View.Elements.Command;
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
    /// Логика взаимодействия для ConfirmDialog.xaml
    /// </summary>
    public partial class ConfirmDialog : Window
    {       
        public bool IsConfirmed { get; private set; }
        public string Message { get; private set; }
        public ICommand SetConfirmed { get; private set; }
        public ConfirmDialog(string message)
        {
            InitializeComponent();
            DataContext = this;
            Message = message;
            SetConfirmed = new RelayCommand(Execute);
        }
        private void Execute(object parameter)
        {            
            bool.TryParse(parameter as string,out bool result);
            IsConfirmed = result;
            this.Close();            
        }
    }
}
