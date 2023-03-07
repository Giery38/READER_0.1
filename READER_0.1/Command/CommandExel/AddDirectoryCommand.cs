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
        private readonly WindowFileBase windowFileBase;
        private readonly ExelViewModel exelViewModel;
        protected event Action ChangeDirectoryList;
        public AddDirectoryCommand(ExelViewModel exelViewModel, WindowFileBase windowFileBase)
        {
            this.windowFileBase = windowFileBase;
            this.exelViewModel = exelViewModel;
            ChangeDirectoryList += exelViewModel.UpdateDirectory;            
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
                if (exelViewModel.SelectedExelFile != null && exelViewModel.SelectedPage != null) // убрать это если нужно создать самодостаточную папку
                {
                    windowFileBase.exelWindowFileBase.AddDirectory(directory, exelViewModel.SelectedExelFile);
                }
                ChangeDirectoryList?.Invoke();
            }
        }
        /*
        private void AddDirectoryToBindingFile(Model.Directory directory) 
        {
            windowFileBase.exelWindowFileBase.AddDirectory(directory, exelViewModel.SelectedExelFile);
            int indexPage = GetIndexPage(exelViewModel.SelectedExelFile, exelViewModel.SelectedPage);
            List<Model.File> EqualsFile = directory.SearchFileToName(exelViewModel.SelectedExelFile, indexPage, exelViewModel.SelectedColumnName.StringValue, Formats.pdf, Model.Directory.SearchParametr.Equals); 
            List<Model.File> NoEqualsFile = directory.SearchFileToName(exelViewModel.SelectedExelFile, indexPage, exelViewModel.SelectedColumnName.StringValue, Formats.pdf, Model.Directory.SearchParametr.NoEquals);
            windowFileBase.exelWindowFileBase.AddСontentInDirectoriesEquals(exelViewModel.SelectedPage, EqualsFile);
            windowFileBase.exelWindowFileBase.AddСontentInDirectoriesNoEquals(exelViewModel.SelectedPage, NoEqualsFile);
        }*/
        private int GetIndexPage(ExelFile exelFile, ExelFilePage exelFilePage)
        {
            int index = 0;
            if (exelFile.ExelPage.Count > 0)
            {
                index = exelFile.ExelPage.FindIndex(item => item.WorksheetName == exelFilePage.WorksheetName);
            }
            return index;
        }
    }
}
