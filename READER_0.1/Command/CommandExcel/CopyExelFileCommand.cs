using READER_0._1.Model;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using WinForms = System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows;
using READER_0._1.Model.Excel;

namespace READER_0._1.Command.CommandExcel
{
    public class CopyExcelFileCommand : CommandBase
    {
        private readonly ExcelViewModel excelViewModel;
        public CopyExcelFileCommand(ExcelViewModel excelViewModel)
        {
            this.excelViewModel = excelViewModel;
            excelViewModel.PropertyChanged += ExcelViewModel_PropertyChanged;
        }
        private void ExcelViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(excelViewModel.ExcelFilesСontentInDirectoriesEquals))
            {
                OnCanExecutedChanged();
            }
        }
        public override bool CanExecute(object parameter)
        {
            if (excelViewModel.ExcelFilesСontentInDirectoriesEquals.Count > 0)
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
                SearchFilesResult searchFilesResult = excelViewModel.windowFileBase.excelWindowFileBase.SearchFilesResults
   .Find(item => item.ExcelFile == excelViewModel.SelectedExcelFile && item.NameColumn == excelViewModel.SelectedColumnName);
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
