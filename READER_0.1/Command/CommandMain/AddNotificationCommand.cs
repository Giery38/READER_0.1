using DevExpress.XtraReports.Native.Parameters;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace READER_0._1.Command.CommandMain
{
    public class AddNotificationCommand : CommandBase
    {
        private readonly MainViewModel mainViewModel;
        public AddNotificationCommand(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
        }

        public override void Execute(object parameter)
        {
            if (parameter is bool)
            {              
                mainViewModel.AddNotification((bool)parameter);                
            }
        }
    }
}
