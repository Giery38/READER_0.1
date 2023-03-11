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
        private readonly ExelViewModel exelViewModel;
        public AddExelFileDropCommand(ExelViewModel exelViewModel) // добавляет все файлы, переопрелить для каждого окна
        {
            this.exelViewModel = exelViewModel;
        }
        public override void Execute(object parameter)
        {
            (string folderName, string[] filePaths) addedFiles = ((string, string[]))parameter;            
            List<ExelFile> files = new List<ExelFile>();
            File file;
            string extension;
            for (int i = 0; i < addedFiles.filePaths.Length; i++)
            {
                extension = Path.GetExtension(addedFiles.filePaths[i]).Replace(".", "");
                file = new File(addedFiles.filePaths[i], Path.GetFileNameWithoutExtension(addedFiles.filePaths[i]), (Formats)Enum.Parse(typeof(Formats), extension, true));
                if (file.Format == Formats.xls || file.Format == Formats.xlsx)
                {
                    files.Add(file.ToExelFile());
                }
            }
            exelViewModel.AddExelFile(files, addedFiles.folderName);
        }
    }
}
