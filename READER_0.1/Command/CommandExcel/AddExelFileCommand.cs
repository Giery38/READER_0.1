using Microsoft.Win32;
using READER_0._1.Model;
using READER_0._1.Model.Excel;
using READER_0._1.View.Elements;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using File = READER_0._1.Model.File;

namespace READER_0._1.Command.CommandExcel
{
    public class AddExcelFileCommand : CommandBase
    {        
        private readonly ExcelViewModel excelViewModel;
        public AddExcelFileCommand(ExcelViewModel excelViewModel)
        {
            this.excelViewModel = excelViewModel;
           
        }                     
        public override void Execute(object parameter)
        {           
            List<ExcelFile> files = new List<ExcelFile>();
            File file;
            OpenFileDialog openFileDialog = new OpenFileDialog
            {                
                RestoreDirectory = false,
                Multiselect = true,
                Title = "Выберите файлы",
                Filter = "Excel Files|*.xls;*.xlsx"                
            };           
            bool? response = openFileDialog.ShowDialog();
            if (response == true)
            {
                string[] filePath = openFileDialog.FileNames;
                string extension;
                for (int i = 0; i < filePath.Length; i++)
                {
                    extension = Path.GetExtension(filePath[i]);
                    file = new File(filePath[i], Path.GetFileNameWithoutExtension(filePath[i]), extension);
                    if (file.Format == ".xls" || file.Format == ".xlsx")
                    {
                        files.Add(file.ToExcelFile());
                    }                    
                }                
            }
            excelViewModel.AddExcelFile(files, (Guid)parameter);
        }       
    }
}
