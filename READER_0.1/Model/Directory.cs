using READER_0._1.Model.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using static READER_0._1.Model.Excel.Settings.ExcelSettingsSearchFiles;

namespace READER_0._1.Model
{
    public class Directory
    {
        public string Path { get; private set; }
        public string Name { get; private set; }
        public List<File> Files { get; private set; }

        public Guid Id { get; }
        public Directory(string path, string name, List<File> files) 
        {
            Path = path;
            Name = name;
            Files = files;    
            Id = Guid.NewGuid();
        }
        public Directory(string name)
        {
            Name = name;
            Files = new List<File>();
            Id = Guid.NewGuid();
        }
        public void AddFile(List<File> addedFiles)
        {
            Files.AddRange(addedFiles);
            
        }
        public void AddFile(List<ExcelFile> addedFiles)
        {
            Files.AddRange(addedFiles);

        }
        public void AddFile(ExcelFile addedFile)
        {
            Files.Add(addedFile);

        }
        public void SetName(string name)
        {
            Name = name;
        }       
        public List<Model.File> SearchFileToName(List<string> searchData, string formatsFileSearch, List<ConfigurationName> configurationName) //переписать тут 
        {
            List<string> tempSearchData = new List<string>(searchData);
            List<Model.File> result = new List<Model.File>();
            for (int i = 0; i < configurationName.Count; i++)
            {
                result.AddRange(SearchFileToName(tempSearchData, formatsFileSearch, configurationName[i]));
            }
            return result;
        }

        private List<Model.File> SearchFileToName(List<string> searchData, string formatsFileSearch, ConfigurationName configurationName)// и тут
        {            
            List<Model.File> result = new List<Model.File>();
            for (int i = 0; i < searchData.Count; i++)
            {
                Model.File file = Files.Find(item => item.Name == searchData[i]);
                if (file == null)
                {
                    string configurationNameString = configurationName.SetOrRemoveConfiguration(searchData[i]);
                    file = Files.Find(item => item.Name == searchData[i] || item.Name == configurationNameString);
                }

                if (file != null && file.Format == formatsFileSearch)
                {
                    searchData.RemoveAt(i);
                    i--;
                    result.Add(file);
                }
            }
            return result;
        }       
        protected Model.File BinarySearch(List<Model.File> files, string name)
        {
            int left = 0;
            int right = files.Count - 1;
            while (left <= right)
            {
                int middle = (left + right) / 2;
                int compareResult = files[middle].Name.CompareTo(name);
                if (compareResult == 0)
                {
                    return files[middle];
                }
                else if (compareResult < 0)
                {                  
                    left = middle + 1;
                }
                else
                {               
                    right = middle - 1;
                }
            }
            return null;
        }     
        internal static bool Exists(string tempFolderPath)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Метод сравнивает фактическое равенство файлов, а не их равенство в программе, то есть сравнивает их пути
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            try
            {
                if (Path != null)
                {
                    return Path.Equals(((Directory)obj).Path);
                }
                return Name.Equals(((Directory)obj).Name);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }
    }
}
