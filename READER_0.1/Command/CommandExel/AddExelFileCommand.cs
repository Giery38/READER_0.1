using Microsoft.Win32;
using READER_0._1.Model;
using READER_0._1.Model.Exel;
using READER_0._1.View.Elements;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using File = READER_0._1.Model.File;

namespace READER_0._1.Command.CommandExel
{
    public class AddExelFileCommand : CommandBase
    {        
        private readonly ExelViewModel exelViewModel;
        public AddExelFileCommand(ExelViewModel exelViewModel)
        {
            this.exelViewModel = exelViewModel;
           
        }       
        private int GetRowSum(int[,] arr, int row)
        {
            int rowSum = 0;
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                rowSum += arr[row, j];               
            }
            return rowSum;
        }       
        public override void Execute(object parameter)
        {           
            List<ExelFile> files = new List<ExelFile>();
            File file;
            OpenFileDialog openFileDialog = new OpenFileDialog
            {                
                RestoreDirectory = false,
                Multiselect = true,
                Title = "Выберите файлы",
                Filter = "Excel Files|*.xls;*.xlsx"                
            };           
            bool? respons = openFileDialog.ShowDialog();
            if (respons == true)
            {
                string[] filePath = openFileDialog.FileNames;
                string extension;
                for (int i = 0; i < filePath.Length; i++)
                {
                    extension = Path.GetExtension(filePath[i]);
                    file = new File(filePath[i], Path.GetFileNameWithoutExtension(filePath[i]), extension);
                    if (file.Format == ".xls" || file.Format == ".xlsx")
                    {
                        files.Add(file.ToExelFile());
                    }                    
                }                
            }
            exelViewModel.AddExelFile(files, (string)parameter);
        }       
    }
}
