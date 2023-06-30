using READER_0._1.Model.Exel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model.Word
{
    public class WordFile : File
    {
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
        private List<ExelFilePageTable> tables;
        public List<ExelFilePageTable> Tables
        {
            get
            {
                return tables;
            }
            set
            {
                tables = value;
                OnPropertyChanged(nameof(Tables));
            }
        }
        public WordFile(string path, string name, string format) : base(path, name, format)
        {           
            Readed = false;
        }
        public void SetReaded(bool value)
        {
            Readed = value;
        }
    }
}
