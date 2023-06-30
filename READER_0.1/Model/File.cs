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
        public string Name { get; protected set; }
        public string Format { get; protected set; }
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
        public File(string path, string name, string format)
        {
            Path = path;
            Name = name;
            Format = format;
        }

        public File()
        {
            Path = "";
            Name = "";
            Format = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }        

        public void SetTempCopyPath(string tempCopyPath) 
        {
            TempCopyPath = tempCopyPath;
        }

        public ExelFile ToExelFile()
        {
            if (this.Format == ".xlsx" ||
                this.Format == ".xls")
            {
                ExelFile exelFile = new ExelFile(Path, Name, this.Format);
                return exelFile;
            }
            else
            {
                throw new Exception("Не подходящий формат файла");
            }

        }
        public WordFile ToWordFile()
        {
            if (this.Format == ".doc" ||
                this.Format == ".docx")
            {
                WordFile wordFile = new WordFile(Path, Name, this.Format);
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

