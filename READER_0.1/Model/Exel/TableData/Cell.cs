using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model.Exel.Table
{
    public class Cell
    {
        public object Data { get; set; }
        public int Position { get; }
        public Cell(object data, int position)
        {
            Data = data;
            Position = position;
        }    
    }
}
