using READER_0._1.Model.Exel.Settings;
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
using static READER_0._1.Model.Exel.Settings.ExelSettingsRead;

namespace READER_0._1.Model.Exel
{
    public class ExelWindowFileBase
    {     
        public Dictionary<ExelFile, List<Directory>> DirectoriesBelongExelFile { get; private set; }
        public List<Directory> FoldersWithFiles { get; private set; }        
        public ExelReaderManager ExelReaderManager { get; private set; }
        public Dictionary<ExelFilePage, List<Model.File>> ExelFilesСontentInDirectoriesEquals { get; private set; }
        public Dictionary<ExelFilePage, List<Model.File>> ExelFilesСontentInDirectoriesNoEquals { get; private set; }
        public List<SearchFilesResult> SearchFilesResults { get; private set; }
        //
        public List<Thread> ThreadsReadFiles { get; private set; } 
        //
        public ExelSettings ExelSettings { get; private set; }
        static public string TempFolderPath { get; private set; }

        public ExelWindowFileBase(string tempFolderPath, ExelSettings exelSettings)
        {
            DirectoriesBelongExelFile = new Dictionary<ExelFile, List<Directory>>();
            FoldersWithFiles = new List<Directory>();           
            ExelReaderManager = new ExelReaderManager(tempFolderPath);
            SearchFilesResults = new List<SearchFilesResult>();
            ExelFilesСontentInDirectoriesEquals = new Dictionary<ExelFilePage, List<Model.File>>();
            ExelFilesСontentInDirectoriesNoEquals = new Dictionary<ExelFilePage, List<Model.File>>();
            ThreadsReadFiles = new List<Thread>();
            //
            this.ExelSettings = exelSettings;
            TempFolderPath = tempFolderPath;
            //                       
        }       
        public void AddThreadsReadFile(Thread thread)
        {
            ThreadsReadFiles.Add(thread);
        }
        
        public bool TryAddSearchFilesResult(SearchFilesResult searchFilesResult)
        {
            if (SearchFilesResults.Contains(searchFilesResult) == false)
            {
                SearchFilesResults.Add(searchFilesResult);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool TryRemoveDirectoryInSearchFilesResult(Directory RemovedDirectory, ExelFile BindingFile)
        {
            SearchFilesResult searchFilesResult = SearchFilesResults.FirstOrDefault(item => item.ExelFile == BindingFile && item.FilesInDirectory.Keys.FirstOrDefault(item => item == RemovedDirectory) == RemovedDirectory);
            if (searchFilesResult == null)
            {
                return false;
            }
            searchFilesResult.FilesInDirectory.Remove(RemovedDirectory);
            return true;
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
            Directory folderWithFiles = FoldersWithFiles.Find(item => item.Name == FolderName);
            if (folderWithFiles == null)
            {
                return;
            }
            ExelReaderManager.RemoveExelFileReader(RemovedFile);
            folderWithFiles.Files.Remove(RemovedFile);
            SearchFilesResults.RemoveAll(item => item.ExelFile == RemovedFile);
            ExelFile directoryBelongExelFileKey = DirectoriesBelongExelFile.Keys.FirstOrDefault(item => item == RemovedFile);
            if (directoryBelongExelFileKey != null)
            {
                DirectoriesBelongExelFile.Remove(directoryBelongExelFileKey);
            }
            try
            {
                System.IO.File.Delete(RemovedFile.TempCopyPath);
            }
            catch (Exception)
            {
                
            }            
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
        public bool TryAddDirectory(Directory AddedDirectory, ExelFile BindingFile)
        {
            if (AddedDirectory == null || BindingFile == null)
            {
                return false;
            }
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
        public bool TryRemoveDirectory(Directory RemovedDirectory, ExelFile BindingFile)
        {
            if (RemovedDirectory == null || BindingFile == null)
            {
                return false;
            }
            if (DirectoriesBelongExelFile.Keys.FirstOrDefault(item => item.Path == BindingFile.Path) != null)
            {
                if (DirectoriesBelongExelFile[BindingFile].Find(item => item.Path == RemovedDirectory.Path) != null)
                {
                    DirectoriesBelongExelFile[BindingFile].Remove(RemovedDirectory);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
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
                if (MainList.Find(item => item.Name == ChekingList[i].Name) == null)
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
        public bool TryReadExelFile(ExelFile exelFile, ExelSettingsRead exelSettingsRead)
        {
            bool result = false;
            Thread readExelFile = new Thread(() =>
            {
                result = ExelReaderManager.TryReadExelFile(exelFile, exelSettingsRead);
            });            
            readExelFile.Start();
            readExelFile.Join();
            return result;
        }         
    }   
}
