using READER_0._1.Model;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using WinForms = System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows;

namespace READER_0._1.Command.CommandExel
{
    class CopyExelFileCommand : CommandBase
    {
        private readonly WindowFileBase windowFileBase;
        private readonly ExelViewModel exelViewModel;
        public CopyExelFileCommand(ExelViewModel exelViewModel, WindowFileBase windowFileBase)
        {
            this.windowFileBase = windowFileBase;
            this.exelViewModel = exelViewModel;
        }
        public override void Execute(object parameter)
        {
            WinForms.FolderBrowserDialog folderBrowserDialog = new WinForms.FolderBrowserDialog();
            WinForms.DialogResult dialogResult = folderBrowserDialog.ShowDialog();
            if (dialogResult == WinForms.DialogResult.OK)
            {
                string directoryPath = folderBrowserDialog.SelectedPath;
                string directoryName = Path.GetFileName(directoryPath);
                string[] allFoundFiles = System.IO.Directory.GetFiles(directoryPath, "*." + nameof(Formats.pdf), SearchOption.TopDirectoryOnly);
                List<Model.File> filesToDirectory = new List<Model.File>();
                for (int i = 0; i < allFoundFiles.Length; i++)
                {
                    filesToDirectory.Add(new Model.File(allFoundFiles[i], Path.GetFileNameWithoutExtension(allFoundFiles[i]), windowFileBase.FormatStrngToEnum(".pdf")));
                }
                Model.Directory directory = new Model.Directory(directoryPath, directoryName, filesToDirectory);
                CopyFiles(directory);
                MessageBox.Show("Все файлы копированы.");
            }
        }

        private void CopyFiles(Model.Directory DestinationDirectory)
        {
            List<Model.File> copiedFiles = new List<Model.File>();
            windowFileBase.exelWindowFileBase.ExelFilesСontentInDirectoriesEquals.TryGetValue(exelViewModel.SelectedPage, out copiedFiles);
            for (int i = 0; i < copiedFiles.Count; i++)
            {
                copiedFiles[i].CopyeTo(DestinationDirectory.Path);
            }
        }
    }
}
