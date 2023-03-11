using READER_0._1.Model.Exel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace READER_0._1.Model
{
    public class Directory
    {
        public string Path { get; private set; }
        public string Name { get; private set; }
        public List<File> Files { get; private set; }
        public Directory(string path, string name, List<File> files)
        {
            Path = path;
            Name = name;
            Files = files;
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
        public List<Model.File> SearchFileToName(List<string> SearchData, Formats FormatsFileSearch)
        {
            List<string> filesPathInDirectory = System.IO.Directory.GetFiles(Path, "*." + FormatsFileSearch.ToString(), SearchOption.TopDirectoryOnly).ToList<string>();
            List<string> filesInDirectoryName = new List<string>();
            List<Model.File> filesInDirectory = new List<Model.File>();
            List<Model.File> filesEquals = new List<Model.File>();           
            for (int i = 0; i < filesPathInDirectory.Count; i++)
            {
                filesInDirectoryName.Add(System.IO.Path.GetFileNameWithoutExtension(filesPathInDirectory[i]));                
                Model.File file = new Model.File(filesPathInDirectory[i], filesInDirectoryName[i], FormatsFileSearch);
                if (file.Path != "")
                {
                    filesInDirectory.Add(file);
                }                
            }
            // List<string> ColumnsData = ExelFile.ExelPage[IndexPage].GetColumnsDataNoDplicates(nameColumn).ConvertAll(x => Convert.ToString(x)).OfType<string>().ToList();// переделать
            List<string> ColumnsData = SearchData;
            for (int i = 0; i < ColumnsData.Count; i++)
            {
                Model.File addedFileEquals = filesInDirectory.Find(item => item.FileName == ColumnsData[i]);               
                if (addedFileEquals != null)
                {
                    filesEquals.Add(addedFileEquals);
                }              
            }
            return filesEquals;
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
