using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static READER_0._1.Model.Excel.Settings.ExcelSettingsSearchFiles;

namespace READER_0._1.Model
{
    public class ModifiedDirectory : Directory
    {
        public List<File> FilesModifiedName { get; private set; }
        public List<File> FilesWithoutModifiedName { get; private set; }
        public ModifiedDirectory(string path, string name, List<File> files) : base(path, name, files)
        {
            FilesModifiedName = new List<File>();
            FilesWithoutModifiedName = new List<File>();
        }
        public void SetFilesModifiedName(ConfigurationName configurationName)
        {
            for (int file = 0; file < Files.Count; file++)
            {
                if (configurationName.CheckModifieds(Files[file].Name) == true)
                {
                    FilesModifiedName.Add(Files[file]);
                }
                else
                {
                    FilesWithoutModifiedName.Add(Files[file]);
                }
            }
            FilesModifiedName.Sort((x, y) => x.Name.CompareTo(y.Name));
            FilesWithoutModifiedName.Sort((x, y) => x.Name.CompareTo(y.Name));
        }
        private List<Model.File> QuickSearch(List<string> searchData, string formatsFileSearch, ConfigurationName configurationName)
        {
            List<Model.File> result = new List<Model.File>();
            Model.File file;
            for (int i = 0; i < searchData.Count; i++)
            {
                if (configurationName.CheckModifieds(searchData[i]) == true)
                {
                    file = BinarySearch(FilesModifiedName, searchData[i]);
                }
                else
                {
                    file = BinarySearch(FilesWithoutModifiedName, searchData[i]);
                }
                if (file != null && file.Format == formatsFileSearch)
                {
                    searchData.RemoveAt(i);
                    result.Add(file);
                }
            }
            return result;
        }

    }
}
