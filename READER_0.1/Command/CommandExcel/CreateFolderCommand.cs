using READER_0._1.Model;
using READER_0._1.ViewModel;
using READER_0._1.ViewModel.ViewElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Command.CommandExcel
{
    public class CreateFolderCommand : CommandBase
    {
        private readonly ViewModel.ExcelViewModel excelViewModel;
        public CreateFolderCommand(ExcelViewModel excelViewModel)
        {
            this.excelViewModel = excelViewModel;
        }
        public override void Execute(object parameter)
        {            
            string name = "Новая папка";           
            excelViewModel.AddFolderView(name);            
        }
    }
}
