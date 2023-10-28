using READER_0._1.Model;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Command.CommandExcel
{
    public class SizeChangeCommand : CommandBase
    {
        private readonly WindowFileBase windowFileBase;
        private readonly ViewModel.ExcelViewModel excelViewModel;
        public SizeChangeCommand(ExcelViewModel excelViewModel, WindowFileBase windowFileBase)
        {
            this.windowFileBase = windowFileBase;
            this.excelViewModel = excelViewModel;
            
        }
        public override void Execute(object parameter)
        {
           
        }
    }
}
