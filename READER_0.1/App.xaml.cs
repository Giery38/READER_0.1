using READER_0._1.Model;
using READER_0._1.View;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace READER_0._1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly WindowFileBase windowFileBase;

        public App()
        {
            windowFileBase = new WindowFileBase();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(windowFileBase)
            };
            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}
