using READER_0._1.Model.Excel;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using READER_0._1.Model.Word;

namespace READER_0._1.Model
{
    public class File : ModelPropertyChanged
    {
        private string path;
        public string Path 
        { 
            get
            {
                return path;
            }
            protected set 
            { 
                path = value;
                OnPropertyChanged(nameof(Path));
            }
        }
        private string name;
        public string Name
        {
            get
            {
                return name;                
            }
            protected set 
            { 
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private string format;
        public string Format 
        { 
            get 
            { 
                return format;
            }
            protected set 
            {
                format = value;
                OnPropertyChanged(nameof(Format));
            }
        }
        private string tempCopyPath;
        public string TempCopyPath 
        {
            get 
            {
                return tempCopyPath;
            }
            protected set 
            {
                tempCopyPath = value;
                OnPropertyChanged(nameof(TempCopyPath));
            }
        }
        private bool readed;
        public bool Readed
        {
            get
            {
                return readed;
            }
            protected set
            {
                readed = value;
                OnPropertyChanged(nameof(Readed));                             
            }
        }

        private bool corrupted = false;
        public bool Corrupted
        {
            get
            {
                return corrupted;
            }
            protected set
            {
                corrupted = value;
                OnPropertyChanged(nameof(Corrupted));                
            }
        }

        public Guid Id { get; }
        public File(string path, string name, string format)
        {
            Path = path;
            Name = name;
            Format = format;
            Id = Guid.NewGuid();
        }

        public File()
        {
            Path = "";
            Name = "";
            Format = null;
            Id = Guid.NewGuid();
        }
        public void SetReaded(bool value)
        {
            Readed = value;
        }
        public void SetCorrupted(bool value) 
        {
            Corrupted = value;
        }
        public void SetTempCopyPath(string tempCopyPath) 
        {
            TempCopyPath = tempCopyPath;
        }

        public ExcelFile ToExcelFile()
        {
            if (this.Format == ".xlsx" ||
                this.Format == ".xls")
            {
                ExcelFile excelFile = new ExcelFile(Path, Name, this.Format);
                return excelFile;
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
        /// <summary>
        /// Метод сравнивает фактическое равенство файлов, а не их равенство в программе, то есть сравнивает их пути
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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

