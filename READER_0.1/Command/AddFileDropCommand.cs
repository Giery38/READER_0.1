using READER_0._1.Model;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using File = READER_0._1.Model.File;

namespace READER_0._1.Command
{
    class AddFileDropCommand : CommandBase
    {
        private readonly WindowFileBase windowFileBase;
        private readonly ExelViewModel exelViewModel;
        private event EventHandler ChangeFileList;
        public AddFileDropCommand(ExelViewModel exelViewModel, WindowFileBase windowFileBase) // добавляет все файлы, переопрелить для каждого окна
        {
            this.windowFileBase = windowFileBase;
            this.exelViewModel = exelViewModel;
            ChangeFileList += exelViewModel.UpdateFiles;
        }
        public override void Execute(object parameter)
        {
            Type typeParameter = parameter.GetType();
            if (typeParameter.IsArray == true)
            {
                List<File> files = new List<File>();
                File file;
                string[] filePath = (string[])parameter;
                for (int i = 0; i < filePath.Length; i++)
                {
                    file = new File(filePath[i], Path.GetFileNameWithoutExtension(filePath[i]), windowFileBase.FormatStrngToEnum(Path.GetExtension(filePath[i])));
                    files.Add(file);
                }
                windowFileBase.AddFiles(files);
                ChangeFileList?.Invoke(this, new EventArgs());
            } 
        }
    }
}
