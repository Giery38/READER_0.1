﻿using READER_0._1.Model;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using READER_0._1.Model.Word;
using System.IO;

namespace READER_0._1.Command.CommandWord
{
    class AddWordFileCommand : CommandBase
    {
        private readonly WindowFileBase windowFileBase;
        private readonly WordViewModel wordViewModel;
        protected event Action ChangeFileList;
        public AddWordFileCommand(WordViewModel wordViewModel, WindowFileBase windowFileBase) // добавляет все файлы, переопрелить для каждого окна
        {
            this.windowFileBase = windowFileBase;
            this.wordViewModel = wordViewModel;
            ChangeFileList += wordViewModel.UpdateFiles;
        }
        public override void Execute(object parameter)
        {
            List<WordFile> files = new List<WordFile>();
            Model.File file;
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                RestoreDirectory = false,
                Multiselect = true,
                Title = "Выберите файлы",
                Filter = "Word Files|*.docx;*.doc"
            };
            bool? respons = openFileDialog.ShowDialog();
            if (respons == true)
            {
                string[] filePath = openFileDialog.FileNames;
                for (int i = 0; i < filePath.Length; i++)
                {                   
                    file = new Model.File(filePath[i], Path.GetFileNameWithoutExtension(filePath[i]), windowFileBase.FormatStrngToEnum(Path.GetExtension(filePath[i])));
                    if (file.Format == Formats.docx || file.Format == Formats.doc)
                    {
                        files.Add(file.ToWordFile());
                    }
                }
            }
            windowFileBase.wordWindowFileBase.AddFiles(files);
            ChangeFileList?.Invoke();
        }
    }
}
