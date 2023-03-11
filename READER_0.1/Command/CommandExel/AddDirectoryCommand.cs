using Microsoft.Win32;
using READER_0._1.Model;
using READER_0._1.Model.Exel;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using WinForms = System.Windows.Forms;
using System.Runtime.InteropServices;

namespace READER_0._1.Command.CommandExel
{
    class AddDirectoryCommand : CommandBase
    {
        private readonly ExelViewModel exelViewModel;
        public AddDirectoryCommand(ExelViewModel exelViewModel)
        {
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
                string extension;
                for (int i = 0; i < allFoundFiles.Length; i++)
                {
                    extension = Path.GetExtension(allFoundFiles[i]).Replace(".", "");
                    filesToDirectory.Add(new Model.File(allFoundFiles[i], Path.GetFileNameWithoutExtension(allFoundFiles[i]), (Formats)Enum.Parse(typeof(Formats), extension, true)));
                }
                Model.Directory directory = new Model.Directory(directoryPath, directoryName, filesToDirectory);
                if (exelViewModel.SelectedExelFile != null && exelViewModel.SelectedPage != null) // убрать это если нужно создать самодостаточную папку
                {
                    exelViewModel.AddDirectory(directory, exelViewModel.SelectedExelFile);                  
                }
               
            }
        }              
    }
}
