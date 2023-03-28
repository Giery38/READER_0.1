using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Tools
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
        public bool IsNullRECT()
        {
            if (left == 0 && top == 0 && right == 0 && bottom == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int Width
        {
            get { return right - left; }
        }

        public int Height
        {
            get { return bottom - top; }
        }
        
    }
}
