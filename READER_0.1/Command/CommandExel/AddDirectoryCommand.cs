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
using READER_0._1.Model.Exel;

namespace READER_0._1.Command.CommandExel
{
    public class AddDirectoryCommand : CommandBase
    {
        private readonly ExelViewModel exelViewModel;
        public AddDirectoryCommand(ExelViewModel exelViewModel)
        {
            this.exelViewModel = exelViewModel;
            exelViewModel.PropertyChanged += ExelViewModel_PropertyChanged;
        }

        private void ExelViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(exelViewModel.SelectedPage))
            {
                OnCanExecutedChanged();
            }           
        }       
        public override bool CanExecute(object parameter)
        {
            if (exelViewModel.SelectedPage == null)
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
                    ExelFile binddingExelFile = exelViewModel.SelectedExelFile;                    
                    foreach (string directoryPath in selectedFolders)
                    {
                        foreach (string extension in exelViewModel.windowFileBase.exelWindowFileBase.ExelSettings.ExelSettingsSearchFiles.FormatsSearch)
                        {
                            FindFiles(directoryPath, extension, binddingExelFile);
                        }
                    }
                });
            }           
        }
        private void FindFiles(string directoryPath, string extension, ExelFile exelFile)
        {
            string directoryName = Path.GetFileName(directoryPath);
            string[] allFoundFiles = System.IO.Directory.GetFiles(directoryPath,"*" + extension, SearchOption.TopDirectoryOnly);
            List<Model.File> filesToDirectory = new List<Model.File>();
            for (int i = 0; i < allFoundFiles.Length; i++)
            {
                filesToDirectory.Add(new Model.File(allFoundFiles[i], Path.GetFileNameWithoutExtension(allFoundFiles[i]), extension));
            }
            Model.Directory directory = new Model.Directory(directoryPath, directoryName, filesToDirectory);
            if (exelViewModel.SelectedExelFile != null && exelViewModel.SelectedPage != null) // убрать это если нужно создать самодостаточную папку
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    exelViewModel.AddDirectory(directory, exelFile);
                });                
            }
        }
    }
}
