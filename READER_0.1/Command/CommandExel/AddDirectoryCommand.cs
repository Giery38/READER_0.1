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
using System.Reflection;
using READER_0._1.View.Elements;
using Microsoft.WindowsAPICodePack.Dialogs;
using READER_0._1.Tools;
using System.Windows;
using System.Windows.Interop;

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
            СustomizedCommonOpenFileDialog сustomizedCommonOpenFileDialog = new СustomizedCommonOpenFileDialog();
            сustomizedCommonOpenFileDialog.Multiselect = true;
            сustomizedCommonOpenFileDialog.FolderPicker = FolderPickerOption.Custom;
            сustomizedCommonOpenFileDialog.OkButtonLabel = "Выбор папки";
            сustomizedCommonOpenFileDialog.FileNameLabel = "Папка:";
            WindowInteropHelper helper = new WindowInteropHelper(Application.Current.MainWindow);
            IntPtr hWnd = helper.Handle;
            if (сustomizedCommonOpenFileDialog.ShowDialog(hWnd) == CommonFileDialogResult.Ok)
            {
                List<string> selectedFolders = сustomizedCommonOpenFileDialog.FilePaths;
                foreach (string directoryPath in selectedFolders)
                {
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
}
