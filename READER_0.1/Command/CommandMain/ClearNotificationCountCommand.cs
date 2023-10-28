using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Command.CommandMain
{
    class ClearNotificationCountCommand : CommandBase
    {
        private readonly MainViewModel mainViewModel;
        public ClearNotificationCountCommand(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
        }

    public override void Execute(object parameter)
        {
            if (mainViewModel.NotificationCountValue != "0")
            {
                mainViewModel.ClearNotificationCount();
            }
        }
    }
}
