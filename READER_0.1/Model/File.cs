using READER_0._1.Model.Exel;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using READER_0._1.Model.Word;

namespace READER_0._1.Model
{
    public class File : INotifyPropertyChanged
    {
        public string Path { get; protected set; }
        public string FileName { get; protected set; }
        public Formats Format { get; protected set; }
        public string TempCopyPath { get; protected set; }
        private bool corrupted = false;
        public bool Corrupted
        {
            get
            {
                return corrupted;
            }
            set
            {
                corrupted = value;
                OnPropertyChanged(nameof(Corrupted));
            }
        }
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CopyeTo(string DestinationPath)
        {
            DestinationPath += "//" + System.IO.Path.GetFileName(Path);
            System.IO.File.Copy(Path, DestinationPath, true);
        }

        public void SetTempCopyPath(string tempCopyPath) 
        {
            TempCopyPath = tempCopyPath;
        }

        public ExelFile ToExelFile()
        {
            if (this.Format == Formats.xlsx ||
                this.Format == Formats.xls)
            {
                ExelFile exelFile = new ExelFile(Path, FileName, this.Format);
                return exelFile;
            }
            else
            {
                throw new Exception("Не подходящий формат файла");
            }

        }
        public WordFile ToWordFile()
        {
            if (this.Format == Formats.doc ||
                this.Format == Formats.docx)
            {
                WordFile wordFile = new WordFile(Path, FileName, this.Format);
                return wordFile;
            }
            else
            {
                throw new Exception("Не подходящий формат файла");
            }

        }
        public override bool Equals(object obj)
        {
            try
            {
                return Path.Equals(((File)obj).Path);
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
public enum Formats
{
    xls,
    xlsx,
    doc,
    docx,
    pdf,
    error
}

