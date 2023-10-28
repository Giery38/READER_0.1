using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Command.CommandExcel
{
    public class ChangeInfoPageCommand : CommandBase
    {
        private readonly ExcelViewModel excelViewModel;
        public ChangeInfoPageCommand(ExcelViewModel excelViewModel)
        {
            this.excelViewModel = excelViewModel;
        }
        public override void Execute(object parameter)
        {
            excelViewModel.CurrentInfoPage = (string)parameter;
        }
    }
}
