using System;
using System.Collections;

namespace READER_0._1.Model.Exel
{
    public class Range 
    {
        public Tuple<int,int> Start { get; private set; }
        public Tuple<int, int> End { get; private set; }

        public Range(Tuple<int, int> start, Tuple<int, int> end)
        {
            Start = start;
            End = end;
        }
        public Range()
        {
            Start = new Tuple<int, int>(0, 0);
            End = new Tuple<int, int>(0, 0);
        }
    }
}