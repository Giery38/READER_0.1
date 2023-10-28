using READER_0._1.Model.Excel;
using READER_0._1.Model.Word;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace READER_0._1.Model
{
    public class WindowFileBase
    {       
        public List<File> Files { get; private set; }
        public List<Directory> Directories { get; private set; }
        static public string TempFolderPath { get; private set; }
        public readonly Settings.Settings settings;
        public readonly ExcelWindowFileBase excelWindowFileBase;
        public readonly WordWindowFileBase wordWindowFileBase;
        public WindowFileBase(string tempFolderPath, Settings.Settings settings)
        {            
            Files = new List<File>();
            Directories = new List<Directory>();           
            TempFolderPath = tempFolderPath;
            this.settings = settings;
            excelWindowFileBase = new ExcelWindowFileBase(TempFolderPath, settings.ExcelSettings);
            wordWindowFileBase = new WordWindowFileBase(TempFolderPath, settings.WordSettings);            
        }                              
    }
    public class FilesReachedEventArgs : EventArgs
    {
        public List<File> ChendgeFiles { get; set; }
        public List<Directory> ChendgeDirectory { get; set; }
        public File File { get; set; }
        public FilesReachedEventArgs(List<File> chandgeFiles)
        {
            ChendgeFiles = chandgeFiles;
        }
        public FilesReachedEventArgs(List<Directory> chendgeDirectory)
        {
            ChendgeDirectory = chendgeDirectory;
        }
        public FilesReachedEventArgs(List<Directory> chendgeDirectory, File file)
        {
            ChendgeDirectory = chendgeDirectory;
            File = file;
        }       
    }

}
