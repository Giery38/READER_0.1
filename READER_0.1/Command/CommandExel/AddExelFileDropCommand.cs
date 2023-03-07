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
    class AddExelFileDropCommand : CommandBase
    {        
        private readonly WindowFileBase windowFileBase;
        private readonly ExelViewModel exelViewModel;
        protected event Action ChangeFileList;
        public AddExelFileDropCommand(ExelViewModel exelViewModel, WindowFileBase windowFileBase) // добавляет все файлы, переопрелить для каждого окна
        {
            this.windowFileBase = windowFileBase;
            this.exelViewModel = exelViewModel;
            ChangeFileList += exelViewModel.UpdateFiles;
        }
        public override void Execute(object parameter)
        {
            Type typeParameter = parameter.GetType();
            if (typeParameter == typeof(Tuple<string, string[]>))
            {
                Tuple<string, string[]> FolderAndPaths = (Tuple<string, string[]>)parameter;
                List<ExelFile> files = new List<ExelFile>();
                File file;
                string[] filePaths = FolderAndPaths.Item2;
                string folderName = FolderAndPaths.Item1;
                for (int i = 0; i < filePaths.Length; i++)
                {
                    file = new File(filePaths[i], Path.GetFileNameWithoutExtension(filePaths[i]), windowFileBase.FormatStrngToEnum(Path.GetExtension(filePaths[i])));                    ;
                    if (file.Format == Formats.xls || file.Format == Formats.xlsx)
                    {
                        files.Add(file.ToExelFile());
                    }
                }
                windowFileBase.exelWindowFileBase.AddFiles(files, folderName);
                ChangeFileList?.Invoke();
            } 
        }
    }
}
