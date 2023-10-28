using READER_0._1.Model.Excel;
using READER_0._1.Model.Excel.TableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model.Word
{
    public class WordFile : File
    {      
        private List<Table> tables;
        public List<Table> Tables
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
