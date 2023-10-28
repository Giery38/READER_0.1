using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using READER_0._1.Tools;
using Microsoft.Win32;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Windows.Interop;
using READER_0._1.Model.Excel;

namespace READER_0._1.Command.CommandExcel
{
    public class AddDirectoryCommand : CommandBase
    {
        private readonly ExcelViewModel excelViewModel;
        public AddDirectoryCommand(ExcelViewModel excelViewModel)
        {
            this.excelViewModel = excelViewModel;
            excelViewModel.PropertyChanged += ExcelViewModel_PropertyChanged;
        }

        private void ExcelViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(excelViewModel.SelectedPage))
            {
                OnCanExecutedChanged();
            }           
        }       
        public override bool CanExecute(object parameter)
        {
            if (excelViewModel.SelectedPage == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public override void Execute(object parameter)
        {
            СustomizedCommonOpenFileDialog customizedCommonOpenFileDialog = new СustomizedCommonOpenFileDialog();
            customizedCommonOpenFileDialog.Multiselect = true;
            customizedCommonOpenFileDialog.FolderPicker = FolderPickerOption.Custom;
            customizedCommonOpenFileDialog.OkButtonLabel = "Выбор папки";
            customizedCommonOpenFileDialog.FileNameLabel = "Папка:";
            if (customizedCommonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Task.Run(() =>
                {                    
                    List<string> selectedFolders = customizedCommonOpenFileDialog.FilePaths;
                    ExcelFile bindingExcelFile = excelViewModel.SelectedExcelFile;                    
                    foreach (string directoryPath in selectedFolders)
                    {
                        foreach (string extension in excelViewModel.windowFileBase.excelWindowFileBase.ExcelSettings.ExcelSettingsSearchFiles.FormatsSearch)
                        {
                            FindFiles(directoryPath, extension, bindingExcelFile);
                        }
                    }
                });
            }           
        }
        private void FindFiles(string directoryPath, string extension, ExcelFile excelFile)
        {
            string directoryName = Path.GetFileName(directoryPath);
            string[] allFoundFiles = System.IO.Directory.GetFiles(directoryPath,"*" + extension, SearchOption.TopDirectoryOnly);
            List<Model.File> filesToDirectory = new List<Model.File>();
            for (int i = 0; i < allFoundFiles.Length; i++)
            {
                filesToDirectory.Add(new Model.File(allFoundFiles[i], Path.GetFileNameWithoutExtension(allFoundFiles[i]), extension));
            }
            Model.ModifiedDirectory directory = new Model.ModifiedDirectory(directoryPath, directoryName, filesToDirectory);
            if (excelViewModel.SelectedExcelFile != null && excelViewModel.SelectedPage != null) // убрать это если нужно создать самодостаточную папку
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    excelViewModel.AddDirectory(directory, excelFile);
                });                
            }
        }
    }
}
