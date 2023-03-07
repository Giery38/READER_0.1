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
        public WordFile(string path, string fileName, Formats format) : base(path, fileName, format)
        {           
            Readed = false;
        }
        public void SetReaded(bool value)
        {
            Readed = value;
        }
    }
}
