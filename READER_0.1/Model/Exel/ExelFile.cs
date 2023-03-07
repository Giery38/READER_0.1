using System;
using System.Collections.Generic;
using System.Text;
using READER_0._1.Model.Settings;
using System.Linq;

namespace READER_0._1.Model.Exel
{
    public class ExelFile : File
    {
        public  List<ExelFilePage> ExelPage { get; private set; }

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

        public ExelFile(string path, string fileName, Formats format) : base(path, fileName, format)
        {
            ExelPage = new List<ExelFilePage>();
            Readed = false;
        }
        public ExelFile()
        {
            ExelPage = new List<ExelFilePage>();
        }
        public ExelFile(ExelFile exelFile)
        {
            FileName = exelFile.FileName;
            Path = exelFile.Path;
            Readed = exelFile.Readed;
            Format = exelFile.Format;
            ExelPage = exelFile.ExelPage;            
        }
        public void SetReaded(bool value)
        {
            Readed = value;
        }

        public void AddPage(ExelFilePage exelFilePage)
        {
            if (exelFilePage != null)
            {
                ExelPage.Add(exelFilePage);
            }           
        }
        public void AddPage(List<ExelFilePage> exelFilePage)
        {
            if (exelFilePage != null)
            {
                ExelPage.AddRange(exelFilePage);
            }           
        }
    }
}

