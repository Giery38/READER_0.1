using READER_0._1.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using static READER_0._1.Model.Settings.Exel.ExelSettingsRead;

namespace READER_0._1.Model.Exel
{
    public class ExelWindowFileBase
    {
        public List<ExelFile> ExelFiles { get; private set; }
        public List<Directory> Directories { get; private set; }
        public Dictionary<ExelFile, List<Directory>> DirectoriesBelongExelFile { get; private set; }
        public Dictionary<string, List<ExelFile>> FoldersWithFiles { get; private set; }
        public Dictionary<ExelFilePage, List<Model.File>> ExelFilesСontentInDirectoriesEquals { get; private set; }
        public Dictionary<ExelFilePage, List<Model.File>> ExelFilesСontentInDirectoriesNoEquals { get; private set; }
        public List<SearchFilesResult> SearchFilesResults { get; private set; }
        //
        private List<Thread> threadsReadFiles;
        //
        private readonly Settings.Exel.ExelSettingsRead exelSettingsRead;
        static public string TempFolderPath { get; private set; }

        public ExelWindowFileBase(string tempFolderPath, Settings.Exel.ExelSettingsRead exelSettingsRead)
        {
            ExelFiles = new List<ExelFile>();
            Directories = new List<Directory>();
            DirectoriesBelongExelFile = new Dictionary<ExelFile, List<Directory>>();
            FoldersWithFiles = new Dictionary<string, List<ExelFile>>
            {
                { "Файлы", new List<ExelFile>() }
            };
            SearchFilesResults = new List<SearchFilesResult>();
            ExelFilesСontentInDirectoriesEquals = new Dictionary<ExelFilePage, List<Model.File>>();
            ExelFilesСontentInDirectoriesNoEquals = new Dictionary<ExelFilePage, List<Model.File>>();
            threadsReadFiles = new List<Thread>();
            //
            this.exelSettingsRead = exelSettingsRead;
            TempFolderPath = tempFolderPath;
        }       

        public void AddSearchFilesResult(SearchFilesResult searchFilesResult)
        {
            SearchFilesResults.Add(searchFilesResult);
        }

        public void AddFiles(List<ExelFile> AddedFiles)
        {
            if (AddedFiles.Count != 0)
            {                
                AddedFiles = AddedFiles.Except(ExelFiles).ToList();                
                ExelFiles.AddRange(AddedFiles);                
                Thread readExelFiles = new Thread(() => ReadExelFiles(AddedFiles));
                readExelFiles.Start();
            }
        }
        public void AddFiles(List<ExelFile> AddedFiles, string FolderName)
        {
            if (AddedFiles.Count != 0)
            {
                AddedFiles = AddedFiles.Except(ExelFiles).ToList();
                ExelFiles.AddRange(AddedFiles);
                FoldersWithFiles[FolderName].AddRange(AddedFiles);
                Thread readExelFiles = new Thread(() => ReadExelFiles(AddedFiles));
                readExelFiles.Start();
            }
        }
        public void RemoveFiles(List<ExelFile> RemovedFiles)
        {
            if (true)
            {
                ExelFiles = ExelFiles.Except(RemovedFiles).ToList();
            }
        }        
        public void AddDirectory(List<Directory> AddedDirectory) // если папка являетьсья самодостаточным элементом 
        {
            if (AddedDirectory.Count != 0)
            {               
                Directories.AddRange(AddedDirectory);
            }
        }
        public void AddDirectory(Directory AddedDirectory)
        {
            if (AddedDirectory != null)
            {
                if (Directories.Find(item => item == AddedDirectory) == null)
                {
                    Directories.Add(AddedDirectory);
                }
            }
        }
        public void AddDirectory(Directory AddedDirectory, ExelFile BindingFile)
        {
            if (AddedDirectory != null)
            {
                List<Directory> directories = new List<Directory>();
                directories.Add(AddedDirectory);
                directories.Distinct();
                Directories.Add(AddedDirectory);
                Directories.Distinct();
                DirectoriesBelongExelFile.TryAdd(BindingFile, directories);
            }
        }
        public void RemoveDirectory(List<Directory> RemovedDirectory)
        {
            if (RemovedDirectory.Count != 0)
            {
                Directories = Directories.Except(RemovedDirectory).ToList();
            }
        }

        public void AddFolder(string nameFolder)
        {
            FoldersWithFiles.TryAdd(nameFolder, new List<ExelFile>());
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
               
        private void ReadExelFiles(List<ExelFile> exelFileReaed)
        {
            foreach (ExelFile exelFile in exelFileReaed)
            {
                Thread readExelFile = new Thread(() => ReadExelFile(exelFile));
                readExelFile.IsBackground = true;
                readExelFile.Name = "Чтение Excel файла " + exelFile.FileName;
                threadsReadFiles.Add(readExelFile);             
                readExelFile.Start();
                readExelFile.Join();
            }    
            
        }
        public void ChangeFolerName(string NewName, int Index)
        {
            List<string> gg = FoldersWithFiles.Keys.ToList();
            List<ExelFile> gga = FoldersWithFiles[gg[Index]];
            FoldersWithFiles.Remove(gg[Index]);
            FoldersWithFiles.Add(NewName, gga);
        }
        private void ReadExelFile(ExelFile exelFile)
        {                       
            int id = ExelFiles.FindIndex(file => file.FileName == exelFile.FileName);
            ExelFileReader exelFileReader = new ExelFileReader(ExelFiles[id], TempFolderPath, exelSettingsRead);
            ExelFiles[id].AddPage(exelFileReader.Read());
            ExelFiles[id].SetReaded(true);
        }
        private enum Operation
        {
            Add,
            Remove
        }
    }   
}
