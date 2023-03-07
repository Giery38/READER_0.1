using READER_0._1.Model;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Command
{
    class NavigateCommand : CommandBase
    {
        private readonly Navigation.Navigation navigation;
        private readonly ViewModelBase nextViewModel;
        public NavigateCommand(ViewModelBase nextViewModel,  Navigation.Navigation navigation)
        {
            this.navigation = navigation;         
            this.nextViewModel = nextViewModel;
        }
        public override void Execute(object parameter)
        {
            navigation.CurrentViewModel = nextViewModel;
        }
        public override bool CanExecute(object parameter)
         {
            if (navigation.CurrentViewModel == nextViewModel)
            {
                return false;
            }
            else
            {
                return true;
            }            
        }
    }
}
