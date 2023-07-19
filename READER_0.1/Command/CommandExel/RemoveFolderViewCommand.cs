using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Command.CommandExel
{
    public class RemoveFolderViewCommand : CommandBase
    {        
        private readonly ExelViewModel exelViewModel;
        public RemoveFolderViewCommand(ExelViewModel exelViewModel)
        {
            this.exelViewModel = exelViewModel;
        }
        public override void Execute(object parameter)
        {
            exelViewModel.RemoveFolderView(parameter as string);
        }
        public override bool CanExecute(object parameter)
        {
            if (parameter == null)
            {
                return true;
            }
            if (parameter != null && parameter is bool == true)
            {
                return (bool)parameter;
            }
            return false;
        }        
    }
}
