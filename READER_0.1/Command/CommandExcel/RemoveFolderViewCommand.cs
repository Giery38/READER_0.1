using READER_0._1.ViewModel;
using READER_0._1.ViewModel.ViewElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace READER_0._1.Command.CommandExcel
{
    public class RemoveFolderViewCommand : CommandBase
    {        
        private readonly ExcelViewModel excelViewModel;
        public RemoveFolderViewCommand(ExcelViewModel excelViewModel)
        {
            this.excelViewModel = excelViewModel;
        }
        public override void Execute(object parameter)
        {
            FolderView folderView = excelViewModel.FoldersView.FirstOrDefault(item => item.Name == parameter as string);
            excelViewModel.RemoveFolderView(folderView);
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
