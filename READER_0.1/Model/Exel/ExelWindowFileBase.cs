using READER_0._1.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using static READER_0._1.Model.Settings.Exel.ExelSettingsRead;

namespace READER_0._1.Model.Exel
{
    public class ExelWindowFileBase
    {     
        public List<Directory> Directories { get; private set; }
        public Dictionary<ExelFile, List<Directory>> DirectoriesBelongExelFile { get; private set; }
        public List<Directory> FoldersWithFiles { get; private set; }        
        public ExelReaderManager ExelReaderManager { get; private set; }
        public Dictionary<ExelFilePage, List<Model.File>> ExelFilesСontentInDirectoriesEquals { get; private set; }
        public Dictionary<ExelFilePage, List<Model.File>> ExelFilesСontentInDirectoriesNoEquals { get; private set; }
        public List<SearchFilesResult> SearchFilesResults { get; private set; }
        //
        public List<Thread> ThreadsReadFiles { get; private set; } //
        //
        private readonly Settings.Exel.ExelSettingsRead exelSettingsRead;
        static public string TempFolderPath { get; private set; }

        public ExelWindowFileBase(string tempFolderPath, Settings.Exel.ExelSettingsRead exelSettingsRead)
        {
            Directories = new List<Directory>();
            DirectoriesBelongExelFile = new Dictionary<ExelFile, List<Directory>>();
            FoldersWithFiles = new List<Directory>()
            {
                { new Directory("Файлы")}
            };
            ExelReaderManager = new ExelReaderManager(tempFolderPath, exelSettingsRead);
            SearchFilesResults = new List<SearchFilesResult>();
            ExelFilesСontentInDirectoriesEquals = new Dictionary<ExelFilePage, List<Model.File>>();
            ExelFilesСontentInDirectoriesNoEquals = new Dictionary<ExelFilePage, List<Model.File>>();
            ThreadsReadFiles = new List<Thread>();
            //
            this.exelSettingsRead = exelSettingsRead;
            TempFolderPath = tempFolderPath;
            //                       
        }       
        public void AddThreadsReadFile(Thread thread)
        {
            ThreadsReadFiles.Add(thread);
        }
        
        public void AddSearchFilesResult(SearchFilesResult searchFilesResult)
        {
            SearchFilesResults.Add(searchFilesResult);
        }
              
        public void AddFile(List<ExelFile> AddedFiles, string FolderName)
        {
            if (AddedFiles.Count == 0)
            {
                return;
            }
            foreach (ExelFile file in AddedFiles)
            {
                Directory folderWithFiles = FoldersWithFiles.Find(item => item.Name == FolderName);
                folderWithFiles.AddFile(file);
            }           
        }
        public void AddFile(ExelFile AddedFile, string FolderName)
        {
            Directory folderWithFiles = FoldersWithFiles.Find(item => item.Name == FolderName);
            folderWithFiles.AddFile(AddedFile);
        }
        public void RemoveFile(ExelFile RemovedFile, string FolderName)
        {
            Thread readFileThread = ExelReaderManager.FindThreadsReadFile(RemovedFile.Path);
            if (readFileThread != null)
            {
                if (readFileThread.IsAlive == true)
                {
                    readFileThread.Interrupt();
                    readFileThread.Join();
                }
                else
                {
                    ThreadsReadFiles.Remove(readFileThread);
                }
            }                   
            Directory folderWithFiles = FoldersWithFiles.Find(item => item.Name == FolderName);
            folderWithFiles.Files.Remove(RemovedFile);
            SearchFilesResults.RemoveAll(item => item.ExelFile == RemovedFile);
        }
        public void RemoveFolderWithFiles(string folderName)
        {
            Directory folderWithFiles = FoldersWithFiles.Find(item => item.Name == folderName);
            foreach (ExelFile file in folderWithFiles.Files.ToList())
            {
                RemoveFile(file, folderName);
            }
            FoldersWithFiles.Remove(folderWithFiles);
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
        public bool TryAddDirectory(Directory AddedDirectory, ExelFile BindingFile)
        {
            if (AddedDirectory == null)
            {
                return false;
            }
            Directories.Add(AddedDirectory);
            Directories.Distinct();
            if (DirectoriesBelongExelFile.Keys.FirstOrDefault(item => item.Path == BindingFile.Path) != null)
            {
                if (DirectoriesBelongExelFile[BindingFile].Find(item => item.Path == AddedDirectory.Path) == null)
                {
                    DirectoriesBelongExelFile[BindingFile].Add(AddedDirectory);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                DirectoriesBelongExelFile.TryAdd(BindingFile, new List<Directory>() { AddedDirectory });
                return true;
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
            FoldersWithFiles.Add(new Directory(nameFolder));
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
        public void SetFolerName(string oldName, string newName)
        {
            Directory folderWithFiles = FoldersWithFiles.Find(item => item.Name == oldName);
            if (FoldersWithFiles.Find(item => item.Name == newName) == null)
            {
                folderWithFiles.SetName(newName);
            }            
        }
        public bool TryReadExelFile(ExelFile exelFile)
        {
            bool result = false;
            Thread readExelFile = new Thread(() =>
            {
                result = ExelReaderManager.TryReadExelFile(exelFile);
            });            
            readExelFile.Start();
            readExelFile.Join();
            return result;
        }         
    }   
}
