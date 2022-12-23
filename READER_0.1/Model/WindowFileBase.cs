using READER_0._1.Model.Exel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace READER_0._1.Model
{
    public class WindowFileBase
    {
        protected event EventHandler<FilesReachedEventArgs> AddFilesEvent;
        protected event EventHandler<FilesReachedEventArgs> RemoveFilesEvent;
        protected event EventHandler<FilesReachedEventArgs> AddDirectoryEvent;
        protected event EventHandler<FilesReachedEventArgs> RemoveDirectoryEvent;
        public List<File> Files { get; private set; }
        public List<Directory> Directories { get; private set; }
        public readonly ExelWindowFileBase exelWindowFileBase;
        public WindowFileBase()
        {
            Files = new List<File>();
            Directories = new List<Directory>();
            exelWindowFileBase = new ExelWindowFileBase();
            AddFilesEvent += exelWindowFileBase.AddFiles;
            RemoveFilesEvent += exelWindowFileBase.RemoveFiles;
            AddDirectoryEvent += exelWindowFileBase.AddDirectories;
        }
        public void AddFiles(List<File> AddedFiles)
        {
            if (AddedFiles.Count != 0)
            {
                AddedFiles = RemoveDublicateInLists(Files, AddedFiles, PatamertMessage.ShowMessage);
                Files.AddRange(AddedFiles);  
                AddFilesEvent?.Invoke(this, new FilesReachedEventArgs(AddedFiles));
            }            
        }
        public void RemoveFiles(List<File> RemovedFiles)
        {
            if (true)
            {
                Files.Except(RemovedFiles);
                RemoveFilesEvent?.Invoke(this, new FilesReachedEventArgs(RemovedFiles));
            }            
        }
        public void AddDirectory(List<Directory> AddedDirectory, File BindingFile) // Если нужно привязать папку к файлу
        {
            if (AddedDirectory.Count != 0)
            {
                AddedDirectory = RemoveDublicateInLists(Directories, AddedDirectory, PatamertMessage.DontShowMessage);
                Directories.AddRange(AddedDirectory);                
                AddDirectoryEvent?.Invoke(this, new FilesReachedEventArgs(AddedDirectory, BindingFile));
            }
        }
        public void AddDirectory(List<Directory> AddedDirectory) // если папка являетьсья самодостаточным элементом 
        {
            if (AddedDirectory.Count != 0)
            {
                AddedDirectory = RemoveDublicateInLists(Directories, AddedDirectory, PatamertMessage.ShowMessage);
                Directories.AddRange(AddedDirectory);
                AddDirectoryEvent?.Invoke(this, new FilesReachedEventArgs(AddedDirectory));
            }
        }
        public void AddDirectory(Directory AddedDirectory, File BindingFile)
        {
            if (AddedDirectory != null)
            {
                List<Directory> directories = new List<Directory>();
                directories.Add(AddedDirectory);
                directories.Distinct();
                Directories.Add(AddedDirectory);
                Directories.Distinct();
                AddDirectoryEvent?.Invoke(this, new FilesReachedEventArgs(directories, BindingFile));
            }
        }
        public void AddDirectory(Directory AddedDirectory)
        {
            if (AddedDirectory != null)
            {
                List<Directory> directories = new List<Directory>();
                directories.Add(AddedDirectory);
                directories = RemoveDublicateInLists(Directories, directories, PatamertMessage.ShowMessage);
                Directories.Add(AddedDirectory);
                AddDirectoryEvent?.Invoke(this, new FilesReachedEventArgs(directories));
            }
        }
        public void RemoveDirectory(List<Directory> RemovedDirectory)
        {
            if (RemovedDirectory.Count != 0)
            {
                Directories.Except(RemovedDirectory);
                AddDirectoryEvent?.Invoke(this, new FilesReachedEventArgs(RemovedDirectory));
            }
        }        
        private List<File> RemoveDublicateInLists(List<File> MainFiles, List<File> ChangeFiles, PatamertMessage patamertMessage)
        {
            bool haveDublicate = false;
            if (MainFiles.Count == 0)
            {
                return ChangeFiles;
            }          
            for (int i = 0; i < MainFiles.Count; i++)
            {
                for (int j = 0; j < ChangeFiles.Count; j++)
                {
                    if (MainFiles[i].FileName == ChangeFiles[j].FileName)
                    {
                        ChangeFiles.Remove(ChangeFiles[j]);
                        haveDublicate = true;
                    }
                }
            }
            if (haveDublicate == true && patamertMessage == PatamertMessage.ShowMessage)
            {
                MessageBox.Show("Некоторые файлы не были добавлены, так как уже присутствуют в списке.");
            }            
            return ChangeFiles;
        }
        private List<Directory> RemoveDublicateInLists(List<Directory> MainDirectories, List<Directory> ChangeDirectories, PatamertMessage patamertMessage)
        {
            bool haveDublicate = false;
            if (MainDirectories.Count == 0)
            {
                return ChangeDirectories;
            }
            for (int i = 0; i < MainDirectories.Count; i++)
            {
                for (int j = 0; j < ChangeDirectories.Count; j++)
                {
                    if (MainDirectories[i].Name == ChangeDirectories[j].Name)
                    {
                        ChangeDirectories.Remove(ChangeDirectories[j]);
                        haveDublicate = true;
                    }
                }
            }
            if (haveDublicate == true && patamertMessage == PatamertMessage.ShowMessage)
            {
                MessageBox.Show("Некоторые файлы не были добавлены, так как уже присутствуют в списке.");
            }
            return ChangeDirectories;
        }

        private enum PatamertMessage
        {
            ShowMessage,
            DontShowMessage
        }

        public Formats FormatStrngToEnum(string format)
        {
            switch (format)
            {
                case ".xls":
                    return Formats.xls;
                case ".xlsx":
                    return Formats.xlsx;
                case ".pdf":
                    return Formats.pdf;
                default:
                    return Formats.error;
            }
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
