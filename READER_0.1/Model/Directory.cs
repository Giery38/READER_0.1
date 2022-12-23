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

        public List<Model.File> SearchFileToName(ExelFile ExelFile, int IndexPage,string nameColumn, Formats FormatsFileSearch, SearchParametr searchParametr)
        {
            List<string> filesPathInDirectory = System.IO.Directory.GetFiles(Path, "*." + FormatsFileSearch.ToString(), SearchOption.TopDirectoryOnly).ToList<string>();
            List<string> filesInDirectoryName = new List<string>();
            List<Model.File> filesInDirectory = new List<Model.File>();
            List<Model.File> filesEquals = new List<Model.File>();
            List<Model.File> filesNoEquals = new List<Model.File>();
            for (int i = 0; i < filesPathInDirectory.Count; i++)
            {
                filesInDirectoryName.Add(System.IO.Path.GetFileNameWithoutExtension(filesPathInDirectory[i]));                
                Model.File file = new Model.File(filesPathInDirectory[i], filesInDirectoryName[i], FormatsFileSearch);
                if (file.Path != "")
                {
                    filesInDirectory.Add(file);
                }                
            }
            List<string> ColumnsData = ListObjectConvertListString(ExelFile.ExelPage[IndexPage].GetColumnsDataNoDplicates(nameColumn));
            for (int i = 0; i < ColumnsData.Count; i++)
            {
                Model.File addedFileEquals = filesInDirectory.Find(item => item.FileName == ColumnsData[i]);               
                if (addedFileEquals != null)
                {
                    filesEquals.Add(addedFileEquals);
                }
                else
                {
                    Model.File addedFileNoEquals = new Model.File("", ColumnsData[i], FormatsFileSearch);
                    filesNoEquals.Add(addedFileNoEquals);
                }
            }
            if (searchParametr == SearchParametr.Equals)
            {
                return filesEquals;
            }
            else
            {
                return filesNoEquals;
            }
        }
        private List<string> ListObjectConvertListString(List<object> ListObject)
        {
            List<string> listString = new List<string>();
            for (int i = 0; i < ListObject.Count; i++)
            {
                if (ListObject[i] != null)
                {
                    listString.Add(ListObject[i].ToString());
                }                
            }
            return listString;
        }
        public enum SearchParametr
        {
            Equals,
            NoEquals
        }
    }
}
