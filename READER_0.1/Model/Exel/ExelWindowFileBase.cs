using READER_0._1.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using static READER_0._1.Model.Settings.ExelSettingsRead;

namespace READER_0._1.Model.Exel
{
    public class ExelWindowFileBase
    {
        public List<ExelFile> ExelFiles { get; private set; }
        public List<Directory> Directories { get; private set; }
        public Dictionary<ExelFile, List<Directory>> ExelFilesСontentInDirectories { get; private set; }
        public Dictionary<ExelFilePage, List<Model.File>> ExelFilesСontentInDirectoriesEquals { get; private set; }
        public Dictionary<ExelFilePage, List<Model.File>> ExelFilesСontentInDirectoriesNoEquals { get; private set; }
        private List<Thread> threadsReadFiles;

        public ExelWindowFileBase()
        {
            ExelFiles = new List<ExelFile>();
            Directories = new List<Directory>();
            ExelFilesСontentInDirectories = new Dictionary<ExelFile, List<Directory>>();
            ExelFilesСontentInDirectoriesEquals = new Dictionary<ExelFilePage, List<Model.File>>();
            ExelFilesСontentInDirectoriesNoEquals = new Dictionary<ExelFilePage, List<Model.File>>();
            threadsReadFiles = new List<Thread>();
        }
        public void AddFiles(object sender, FilesReachedEventArgs AddedFiles)
        {
            ChangeExelFileList(sender, AddedFiles.ChendgeFiles, Operation.Add);           
        }
        public void RemoveFiles(object sender, FilesReachedEventArgs RemovedFiles)
        {
            ChangeExelFileList(sender, RemovedFiles.ChendgeFiles, Operation.Remove);
        }
        public void AddDirectories(object sender, FilesReachedEventArgs AddedDirectories)
        {
            ChangeDirectoriesList(sender, AddedDirectories.ChendgeDirectory, Operation.Add, AddedDirectories.File);
        }
        public void RemoveDirectories(object sender, FilesReachedEventArgs RemovedDirectories)
        {
            ChangeDirectoriesList(sender, RemovedDirectories.ChendgeDirectory, Operation.Remove, RemovedDirectories.File);
        }
        
        public void AddСontentInDirectoriesEquals(ExelFilePage keyPage, List<Model.File> AddedList)
        {                        
            bool availabilitySelectedPageContentEquals = ExelFilesСontentInDirectoriesEquals.TryAdd(keyPage, AddedList);
            if (availabilitySelectedPageContentEquals == false)
            {
                AddedList = RemoveDublicateInLists(ExelFilesСontentInDirectoriesEquals[keyPage], AddedList);
                ExelFilesСontentInDirectoriesEquals[keyPage].AddRange(AddedList);
            }            
        }
                
        public void AddСontentInDirectoriesNoEquals(ExelFilePage keyPage, List<Model.File> AddedList)
        {            
            bool availabilitySelectedPageContentNoEquals = ExelFilesСontentInDirectoriesNoEquals.TryAdd(keyPage, AddedList);
            if (availabilitySelectedPageContentNoEquals == false)
            {
                AddedList = RemoveDublicateInLists(ExelFilesСontentInDirectoriesNoEquals[keyPage], AddedList);
                AddedList = RemoveDublicateInLists(ExelFilesСontentInDirectoriesEquals[keyPage], AddedList);
                ExelFilesСontentInDirectoriesNoEquals[keyPage].AddRange(AddedList);
            }            
        }

        private List<string> RemoveDublicateInLists(List<string> MainList, List<string> ChekingList)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < ChekingList.Count; i++)
            {
                if (MainList.Find(item => item == ChekingList[i]) == null)
                {
                    result.Add(ChekingList[i]);
                }
            }
            return result;
        }
        private List<Model.File> RemoveDublicateInLists(List<Model.File> MainList, List<Model.File> ChekingList)
        {
            List<Model.File> result = new List<Model.File>();
            for (int i = 0; i < ChekingList.Count; i++)
            {
                if (MainList.Find(item => item.FileName == ChekingList[i].FileName) == null)
                {
                    result.Add(ChekingList[i]);
                }
            }
            return result;
        }
        private void ChangeDirectoriesList(object sender, List<Directory> Directories, Operation operation, File file)
        {
            if (sender is WindowFileBase)
            {
                for (int i = 0; i < Directories.Count; i++)
                {
                    if (operation == Operation.Remove)
                    {
                        this.Directories.Remove(Directories[i]);
                    }
                    else if (operation == Operation.Add)
                    {
                        this.Directories.Add(Directories[i]);                        
                    }
                }
                ExelFile exelFileBinnding = file.ToExelFile();
                exelFileBinnding = ExelFiles.Find(item => item.Path == exelFileBinnding.Path);
                bool added = ExelFilesСontentInDirectories.TryAdd(exelFileBinnding, Directories);
                if (added == false)
                {
                    for (int i = 0; i < Directories.Count; i++)
                    {
                        if (ExelFilesСontentInDirectories[exelFileBinnding].Find(item => item.Path == Directories[i].Path) == null)
                        {
                            ExelFilesСontentInDirectories[exelFileBinnding].AddRange(Directories);
                        }
                        else
                        {
                            MessageBox.Show("Папка уже есть в списке.");
                        }
                    }                                       
                }
                this.Directories.Distinct();
            }
        }
        
        private void ChangeExelFileList(object sender, List<File> Files, Operation operation)
        {
            if (sender is WindowFileBase)
            {
                List<ExelFile> exelFileReaed = new List<ExelFile>();
                for (int i = 0; i < Files.Count; i++)
                {
                    if (Files[i].Format == Formats.xls ||
                        Files[i].Format == Formats.xlsx)
                    {                        
                        if (operation == Operation.Remove)
                        {
                            ExelFiles.Remove(Files[i].ToExelFile());
                        }
                        else if(operation == Operation.Add)
                        {
                            ExelFile exelFile = Files[i].ToExelFile();
                            ExelFiles.Add(exelFile);
                            exelFileReaed.Add(exelFile);
                            /*
                            Thread readExelFile = new Thread(() => ReadExelFile(exelFile));
                            readExelFile.Name = "Чтение файла " + exelFile.FileName;
                            if (ThreadHelper.SerchThreadLive(threadsReadFiles).Count == threadsReadFiles.Count)
                            {
                                threadsReadFiles.Clear();
                            }
                            threadsReadFiles.Add(readExelFile);
                            readExelFile.Start();
                            */
                        }
                    }
                }
                if (exelFileReaed.Count > 0)
                {
                    Thread readExelFiles = new Thread(() => ReadExelFiles(exelFileReaed));
                    readExelFiles.Start();
                }
                ExelFiles.Distinct();               
            }        
        }
        private void ReadExelFiles(List<ExelFile> exelFileReaed)
        {
            foreach (ExelFile exelFile in exelFileReaed)
            {
                Thread readExelFile = new Thread(() => ReadExelFile(exelFile));
                readExelFile.Name = "Чтение файла " + exelFile.FileName;
                threadsReadFiles.Add(readExelFile);
                var rr = ThreadHelper.SerchThreadLive(threadsReadFiles).Count;
                while (ThreadHelper.SerchThreadLive(threadsReadFiles).Count > 3)
                {
                    Thread.Sleep(3000);
                }
                readExelFile.Start();
            }            
        }

        private void ReadExelFile(ExelFile exelFile)
        {                       
            int id = ExelFiles.FindIndex(file => file.FileName == exelFile.FileName);
            ExelFileReader exelFileReader = new ExelFileReader(ExelFiles[id]);
            ExelFiles[id].AddPage(exelFileReader.ReadWorksheetsExel(true));
            ExelFiles[id].SetReaded(true);
        }
        private enum Operation
        {
            Add,
            Remove
        }
    }   
}
