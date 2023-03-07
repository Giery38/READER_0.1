using Microsoft.Win32;
using READER_0._1.Model;
using READER_0._1.Model.Exel;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using File = READER_0._1.Model.File;

namespace READER_0._1.Command.CommandExel
{
    class AddExelFileCommand : CommandBase
    {        
        private readonly WindowFileBase windowFileBase;
        private readonly ExelViewModel exelViewModel;
        private event Action ChangeFileList;

        public AddExelFileCommand(ExelViewModel exelViewModel, WindowFileBase windowFileBase)
        {
            this.windowFileBase = windowFileBase;
            this.exelViewModel = exelViewModel;
            ChangeFileList += exelViewModel.UpdateFiles;
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
                for (int i = 0; i < filePath.Length; i++)
                {
                    file = new File(filePath[i], Path.GetFileNameWithoutExtension(filePath[i]), windowFileBase.FormatStrngToEnum(Path.GetExtension(filePath[i])));
                    if (file.Format == Formats.xls || file.Format == Formats.xlsx)
                    {
                        files.Add(file.ToExelFile());
                    }                    
                }                
            }
            windowFileBase.exelWindowFileBase.AddFiles(files, (string)parameter);
            ChangeFileList?.Invoke();            
        }        
    }
}
