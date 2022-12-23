using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model.Exel
{
    public class MergeCell
    {
        public Tuple<int,int> StartPosition { get; private set; }
        public int Size { get; private set; }
        public MergeCell(Tuple<int, int> startPosition, int size)
        {
            StartPosition = startPosition;
            Size = size;
        }
    }
}
