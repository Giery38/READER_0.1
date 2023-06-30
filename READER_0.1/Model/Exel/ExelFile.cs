using System;
using System.Collections.Generic;
using System.Text;
using READER_0._1.Model.Settings;
using System.Linq;

namespace READER_0._1.Model.Exel
{
    public class ExelFile : File
    {
        public  List<ExelFilePage> ExelPages { get; private set; }

        private bool readed; 
        public bool Readed
        {
            get
            {
                return readed;
            }
            private set
            {
                readed = value;
                OnPropertyChanged(nameof(Readed));
            }
        }

        public ExelFile(string path, string name, string format) : base(path, name, format)
        {
            ExelPages = new List<ExelFilePage>();
            Readed = false;
        }
        public ExelFile()
        {
            ExelPages = new List<ExelFilePage>();
        }
        public ExelFile(ExelFile exelFile)
        {
            Name = exelFile.Name;
            Path = exelFile.Path;
            Readed = exelFile.Readed;
            Format = exelFile.Format;
            ExelPages = exelFile.ExelPages;            
        }
        public void SetReaded(bool value)
        {
            Readed = value;
        }

        public void AddPage(ExelFilePage exelFilePage)
        {
            if (exelFilePage != null)
            {
                ExelPages.Add(exelFilePage);
            }           
        }
        public void AddPage(List<ExelFilePage> exelFilePage)
        {
            if (exelFilePage != null)
            {
                ExelPages.AddRange(exelFilePage);
            }           
        }
    }
}

