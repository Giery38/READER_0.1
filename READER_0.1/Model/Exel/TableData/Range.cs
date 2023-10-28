using System;
using System.Collections;

namespace READER_0._1.Model.Excel.TableData
{
    public class Range 
    {
        public (int row, int column) Start { get; private set; }
        public (int row, int column) End { get; private set; }

        public Range((int row, int column) start, (int row, int column) end)
        {
            Start = start;
            End = end;
        }
        public Range()
        {
            Start = new (0, 0);
            End = new (0, 0);
        }
    }
}