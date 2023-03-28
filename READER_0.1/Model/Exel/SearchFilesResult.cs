using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model.Exel
{
    public class SearchFilesResult
    {
        public ExelFile ExelFile { get; private set; }
        public Dictionary<Directory, List<Model.File>> FilesInDirectory { get; private set; }        
        public string NameColumn { get; private set; }      

        public SearchFilesResult()
        {           
            FilesInDirectory = new Dictionary<Directory, List<Model.File>>();
        }
        public void SetExelFile(ExelFile exelFile)
        {
            ExelFile = exelFile;
        }
        public void SetNameColumn(string nameColumn)
        {
            NameColumn = nameColumn;
        }  
        public List<Model.File> GetAllFiles()
        {
            return FilesInDirectory.Values.SelectMany(x => x).Distinct().ToList();
        }
        public void AddFilesInDirectory(Directory directory, List<Model.File> Files)
        {
            FilesInDirectory.TryAdd(directory, Files);
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            SearchFilesResult other = (SearchFilesResult)obj;
            return ExelFile.Equals(other.ExelFile) &&
                   FilesInDirectory.Equals(other.FilesInDirectory) &&
                   NameColumn.Equals(other.NameColumn);
        }
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + ExelFile.GetHashCode();
            hash = hash * 31 + FilesInDirectory.GetHashCode();
            hash = hash * 31 + NameColumn.GetHashCode();
            return hash;
        }
    }
}
