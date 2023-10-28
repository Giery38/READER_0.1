using READER_0._1.Model.Excel;
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
    public class RemoveExcelFileCommand : CommandBase
    {
        private readonly ExcelViewModel excelViewModel;

        public RemoveExcelFileCommand(ExcelViewModel excelViewModel)
        {
            this.excelViewModel = excelViewModel;
        }

        public override void Execute(object parameter)
        {
            (ExcelFile excelFile, Guid folderId) remove = ((ExcelFile, Guid))parameter;       
            FolderView folderView = excelViewModel.FoldersView.FirstOrDefault(item => item.Id == remove.folderId);
            if (folderView != null)
            {
                excelViewModel.RemoveExcelFile(remove.excelFile, folderView);
            }           
        }
    }
}
