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
            return FilesInDirectory.Values.SelectMany(x => x).ToList();
        }
        public void AddFilesInDirectory(Directory directory, List<Model.File> Files)
        {
            FilesInDirectory.TryAdd(directory, Files);
        }
    }
}
