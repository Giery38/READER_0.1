using READER_0._1.Model.Excel.Settings;
using READER_0._1.Tools;
using READER_0._1.Tools.ThreadManagers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using static READER_0._1.Model.Excel.Settings.ExcelSettingsRead;

namespace READER_0._1.Model.Excel
{
    public class ExcelWindowFileBase
    {     
        public Dictionary<ExcelFile, List<ModifiedDirectory>> DirectoriesBelongExcelFile { get; private set; }
        public List<Directory> FoldersWithFiles { get; private set; }        
        public ExcelReaderManager ExcelReaderManager { get; private set; }       
        public Dictionary<Page, List<Model.File>> ExcelFilesСontentInDirectoriesEquals { get; private set; }
        public Dictionary<Page, List<Model.File>> ExcelFilesСontentInDirectoriesNoEquals { get; private set; }
        public List<SearchFilesResult> SearchFilesResults { get; private set;}
        //
        public ExcelSettings ExcelSettings { get; private set; }
        public string TempFolderPath { get; private set; }

        private QueueManager excelRemoveManager;

        public ExcelWindowFileBase(string tempFolderPath, ExcelSettings excelSettings)
        {
            DirectoriesBelongExcelFile = new Dictionary<ExcelFile, List<ModifiedDirectory>>();
            FoldersWithFiles = new List<Directory>();          
            //
            ExcelReaderManager = new ExcelReaderManager(tempFolderPath);
            excelRemoveManager = new QueueManager("ExcelRemoveManager");

            SearchFilesResults = new List<SearchFilesResult>();
            ExcelFilesСontentInDirectoriesEquals = new Dictionary<Page, List<Model.File>>();
            ExcelFilesСontentInDirectoriesNoEquals = new Dictionary<Page, List<Model.File>>();          
            //
            this.ExcelSettings = excelSettings;
            TempFolderPath = tempFolderPath;
            //      
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
        public bool TryRemoveDirectoryInSearchFilesResult(Directory RemovedDirectory, ExcelFile BindingFile)
        {
            SearchFilesResult searchFilesResult = SearchFilesResults.FirstOrDefault(item => item.ExcelFile == BindingFile && item.FilesInDirectory.Keys.FirstOrDefault(item => item == RemovedDirectory) == RemovedDirectory);
            if (searchFilesResult == null)
            {
                return false;
            }
            searchFilesResult.FilesInDirectory.Remove(RemovedDirectory);
            return true;
        }
        public void AddFile(List<ExcelFile> addedFiles, Guid folderId)
        {
            Directory folderWithFiles = FoldersWithFiles.Find(item => item.Id == folderId);
            foreach (ExcelFile file in addedFiles)
            {               
                folderWithFiles.AddFile(file);
            }           
        }
        public void AddFile(ExcelFile addedFile, Guid folderId) 
        {
            Directory folderWithFiles = FoldersWithFiles.Find(item => item.Id == folderId);
            folderWithFiles.AddFile(addedFile);
        }        
        public void RemoveFile(ExcelFile removedFile, Guid folderId) 
        {
            excelRemoveManager.AddFunc(new Action<ExcelFile, Guid>(RemoveFileBody),
                   folderId, "Удаление Excel файла" + "&&" + removedFile.Path, new object[] { removedFile, folderId });
        }
        private void RemoveFileBody(ExcelFile RemovedFile, Guid folderId)
        {            
            Directory folderWithFiles = FoldersWithFiles.Find(item => item.Id == folderId);
            if (folderWithFiles == null)
            {
                return;
            }
            ExcelReaderManager.RemoveExcelFileReader(RemovedFile);
            folderWithFiles.Files.Remove(RemovedFile);
            SearchFilesResults.RemoveAll(item => item.ExcelFile == RemovedFile);
            ExcelFile directoryBelongExcelFileKey = DirectoriesBelongExcelFile.Keys.FirstOrDefault(item => item == RemovedFile);
            if (directoryBelongExcelFileKey != null)
            {
                DirectoriesBelongExcelFile.Remove(directoryBelongExcelFileKey);
            }
            try
            {
                System.IO.File.Delete(RemovedFile.TempCopyPath);
            }
            catch (Exception)
            {

            }
        }                
        public void RemoveFolder(Guid folderId)
        {
            Directory folderWithFiles = FoldersWithFiles.Find(folder => folder.Id == folderId);
            FoldersWithFiles.Remove(folderWithFiles);
            for (int i = 0; i < folderWithFiles.Files.Count; i++)
            {
                if (folderWithFiles.Files[i] != null)
                {
                    RemoveFile(folderWithFiles.Files[i] as ExcelFile, folderWithFiles.Id);
                }               
            }
        }
        public bool TryAddDirectory(ModifiedDirectory AddedDirectory, ExcelFile BindingFile)
        {
            if (AddedDirectory == null || BindingFile == null)
            {
                return false;
            }
            if (DirectoriesBelongExcelFile.Keys.FirstOrDefault(item => item.Path == BindingFile.Path) != null)
            {
                if (DirectoriesBelongExcelFile[BindingFile].Find(item => item.Path == AddedDirectory.Path) == null)
                {
                    DirectoriesBelongExcelFile[BindingFile].Add(AddedDirectory);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                DirectoriesBelongExcelFile.TryAdd(BindingFile, new List<ModifiedDirectory>() { AddedDirectory });
                return true;
            }        
        }
        public bool TryRemoveDirectory(ModifiedDirectory RemovedDirectory, ExcelFile BindingFile) //продолжать фиксить все на айди
        {
            if (RemovedDirectory == null || BindingFile == null)
            {
                return false;
            }
            if (DirectoriesBelongExcelFile.Keys.FirstOrDefault(item => item.Path == BindingFile.Path) != null)
            {
                if (DirectoriesBelongExcelFile[BindingFile].Find(item => item.Path == RemovedDirectory.Path) != null)
                {
                    DirectoriesBelongExcelFile[BindingFile].Remove(RemovedDirectory);
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
        public void AddFolder(Directory directory)
        {
            FoldersWithFiles.Add(directory);
        }
        public void AddСontentInDirectoriesEquals(Page keyPage, List<Model.File> AddedList)
        {                        
            bool availabilitySelectedPageContentEquals = ExcelFilesСontentInDirectoriesEquals.TryAdd(keyPage, AddedList);
            if (availabilitySelectedPageContentEquals == false)
            {               
                AddedList = RemoveDublicateInLists(ExcelFilesСontentInDirectoriesEquals[keyPage], AddedList);
                ExcelFilesСontentInDirectoriesEquals[keyPage].AddRange(AddedList);
            }            
        }
                
        public void AddСontentInDirectoriesNoEquals(Page keyPage, List<Model.File> AddedList)
        {            
            bool availabilitySelectedPageContentNoEquals = ExcelFilesСontentInDirectoriesNoEquals.TryAdd(keyPage, AddedList);
            if (availabilitySelectedPageContentNoEquals == false)
            {
                AddedList = RemoveDublicateInLists(ExcelFilesСontentInDirectoriesNoEquals[keyPage], AddedList);
                AddedList = RemoveDublicateInLists(ExcelFilesСontentInDirectoriesEquals[keyPage], AddedList);
                ExcelFilesСontentInDirectoriesNoEquals[keyPage].AddRange(AddedList);
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
        public void ReadExcelFile(ExcelFile excelFile, ExcelSettingsRead excelSettingsRead)
        {           
            ExcelReaderManager.Read(excelFile, excelSettingsRead);           
        }         
    }   
}
