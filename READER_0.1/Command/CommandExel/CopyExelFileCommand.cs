using READER_0._1.Model;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using WinForms = System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows;
using READER_0._1.Model.Exel;

namespace READER_0._1.Command.CommandExel
{
    public class CopyExelFileCommand : CommandBase
    {
        private readonly ExelViewModel exelViewModel;
        public CopyExelFileCommand(ExelViewModel exelViewModel)
        {
            this.exelViewModel = exelViewModel;
            exelViewModel.PropertyChanged += ExelViewModel_PropertyChanged;
        }
        private void ExelViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(exelViewModel.ExelFilesСontentInDirectoriesEquals))
            {
                OnCanExecutedChanged();
            }
        }
        public override bool CanExecute(object parameter)
        {
            if (exelViewModel.ExelFilesСontentInDirectoriesEquals.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override void Execute(object parameter)
        {           
            WinForms.FolderBrowserDialog folderBrowserDialog = new WinForms.FolderBrowserDialog();
            WinForms.DialogResult dialogResult = folderBrowserDialog.ShowDialog();
            if (dialogResult == WinForms.DialogResult.OK)
            {
                SearchFilesResult searchFilesResult = exelViewModel.windowFileBase.exelWindowFileBase.SearchFilesResults
   .Find(item => item.ExelFile == exelViewModel.SelectedExelFile && item.NameColumn == exelViewModel.SelectedColumnName);
                string directoryPath = folderBrowserDialog.SelectedPath;
                foreach (List<Model.File> files in searchFilesResult.FilesInDirectory.Values)
                {
                    foreach (Model.File file in files)
                    {
                        System.IO.File.Copy(file.Path, Path.Combine(directoryPath, file.Name + file.Format), true);
                    }
                }                
                MessageBox.Show("Все файлы копированы.");
            }
        }       
    }
}
