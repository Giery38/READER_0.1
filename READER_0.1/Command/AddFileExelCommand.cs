using Microsoft.Win32;
using READER_0._1.Model;
using READER_0._1.Model.Exel;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using File = READER_0._1.Model.File;

namespace READER_0._1.Command
{
    class AddFileExelCommand : CommandBase
    {
        private readonly WindowFileBase windowFileBase;
        private readonly ExelViewModel exelViewModel;
        private event EventHandler ChangeFileList;

        public AddFileExelCommand(ExelViewModel exelViewModel, WindowFileBase windowFileBase)
        {
            this.windowFileBase = windowFileBase;
            this.exelViewModel = exelViewModel;
            ChangeFileList += exelViewModel.UpdateFiles;
        }
        public override void Execute(object parameter)
        {            
            List<File> files = new List<File>();
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
                    files.Add(file);
                }                
            }            
            windowFileBase.AddFiles(files);
            ChangeFileList?.Invoke(this, new EventArgs());     
            
        }        
    }
}
