using READER_0._1.Model.Exel;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace READER_0._1.Model
{
    public class File
    {
        public string Path { get;}
        public string FileName { get;}
        public Formats Format { get; }
        public File(string path, string fileName, Formats format)
        {
            Path = path;
            FileName = fileName;
            Format = format;
        }

        public File()
        {
            Path = "";
            FileName = "";
            Format = Formats.error;
        }

        public void CopyeTo(string DestinationPath)
        {
            DestinationPath += "//" + System.IO.Path.GetFileName(Path);
            System.IO.File.Copy(Path, DestinationPath);
        }

        public ExelFile ToExelFile()
        {
            if (this.Format == Formats.xlsx ||
                this.Format == Formats.xls)
            {
                ExelFile exelFile = new ExelFile(Path, FileName, Formats.xlsx);
                return exelFile;
            }
            else
            {
                throw new Exception("Не подходящий формат файла");
            }

        }
    }
}
public enum Formats
{
    xls,
    xlsx,
    doc,
    docx,
    pdf,
    error
}

