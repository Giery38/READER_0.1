using READER_0._1.Model;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Command.CommandExel
{
    class SizeChangeCommand : CommandBase
    {
        private readonly WindowFileBase windowFileBase;
        private readonly ViewModel.ExelViewModel exelViewModel;
        public SizeChangeCommand(ExelViewModel exelViewModel, WindowFileBase windowFileBase)
        {
            this.windowFileBase = windowFileBase;
            this.exelViewModel = exelViewModel;
            
        }
        public override void Execute(object parameter)
        {
           
        }
    }
}
