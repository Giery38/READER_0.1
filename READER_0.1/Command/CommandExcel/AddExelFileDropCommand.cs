using READER_0._1.Model;
using READER_0._1.Model.Excel;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using File = READER_0._1.Model.File;

namespace READER_0._1.Command.CommandExcel
{
    public class AddExcelFileDropCommand : CommandBase
    {        
        private readonly ExcelViewModel excelViewModel;
        public AddExcelFileDropCommand(ExcelViewModel excelViewModel) 
        {
            this.excelViewModel = excelViewModel;
        }
        public override void Execute(object parameter)
        {
            (Guid folderId, string[] filePaths) addedFiles = ((Guid, string[]))parameter;            
            List<ExcelFile> files = new List<ExcelFile>();
            File file;
            string extension;
            for (int i = 0; i < addedFiles.filePaths.Length; i++)
            {
                extension = Path.GetExtension(addedFiles.filePaths[i]).Replace(".", "");
                file = new File(addedFiles.filePaths[i], Path.GetFileNameWithoutExtension(addedFiles.filePaths[i]), extension);
                if (file.Format == ".xls" || file.Format == ".xlsx")
                {
                    files.Add(file.ToExcelFile());
                }
            }
            excelViewModel.AddExcelFile(files, addedFiles.folderId);
        }
    }
}
