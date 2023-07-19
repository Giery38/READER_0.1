using READER_0._1.Model.Exel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using static READER_0._1.Model.Exel.Settings.ExelSettingsSearchFiles;

namespace READER_0._1.Model
{
    public class Directory
    {
        public string Path { get; private set; }
        public string Name { get; private set; }
        public List<File> Files { get; private set; }

        private List<File> filseModifiedName;
        private List<File> filseWithoutModifiedName;
        public Directory(string path, string name, List<File> files)
        {
            Path = path;
            Name = name;
            Files = files;
            filseModifiedName = new List<File>();
            filseWithoutModifiedName = new List<File>();
        }
        public Directory(string name)
        {
            Name = name;
            Files = new List<File>();
        }
        public void AddFile(List<File> addedFiles)
        {
            Files.AddRange(addedFiles);
            
        }
        public void AddFile(List<ExelFile> addedFiles)
        {
            Files.AddRange(addedFiles);

        }
        public void AddFile(ExelFile addedFile)
        {
            Files.Add(addedFile);

        }
        public void SetName(string name)
        {
            Name = name;
        }
        public void SetFilseModifiedName(ConfigurationName configurationName)
        {
            for (int file = 0; file < Files.Count; file++)
            {
                if (configurationName.CheckModifieds(Files[file].Name) == true)
                {
                    filseModifiedName.Add(Files[file]);
                }
                else
                {
                    filseWithoutModifiedName.Add(Files[file]);
                }
            }
            filseModifiedName.Sort((x, y) => x.Name.CompareTo(y.Name));
            filseWithoutModifiedName.Sort((x, y) => x.Name.CompareTo(y.Name));
        }

        public List<Model.File> SearchFileToName(List<string> searchData, string formatsFileSearch, List<ConfigurationName> configurationName)
        {
            List<string> tempSearchData = new List<string>(searchData);
            List<Model.File> result = new List<Model.File>();
            for (int i = 0; i < configurationName.Count; i++)
            {
                result.AddRange(SearchFileToName(tempSearchData, formatsFileSearch, configurationName[i]));
            }
            return result;
        }

        private List<Model.File> SearchFileToName(List<string> searchData, string formatsFileSearch, ConfigurationName configurationName)
        {            
            List<Model.File> result = new List<Model.File>();
            if (false)//filseModifiedName.Count > 0 || filseWithoutModifiedName.Count > 0
            {
#pragma warning disable CS0162 // Обнаружен недостижимый код
                result = QuickSearch(searchData, formatsFileSearch, configurationName);
#pragma warning restore CS0162 // Обнаружен недостижимый код
            }
            else
            {
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
                        result.Add(file);
                    }
                }
            }
            return result;
        }
        private List<Model.File> QuickSearch(List<string> searchData, string formatsFileSearch, ConfigurationName configurationName)
        {
            List<Model.File> result = new List<Model.File>();
            Model.File file;
            for (int i = 0; i < searchData.Count; i++)
            {
                if (configurationName.CheckModifieds(searchData[i]) == true)
                {
                    file = BinarySearch(filseModifiedName, searchData[i]);                                       
                }
                else
                {
                    file = BinarySearch(filseWithoutModifiedName, searchData[i]);
                }
                if (file != null && file.Format == formatsFileSearch)
                {
                    searchData.RemoveAt(i);
                    result.Add(file);
                }
            }
            return result;
        }
        private Model.File BinarySearch(List<Model.File> files, string name)
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
