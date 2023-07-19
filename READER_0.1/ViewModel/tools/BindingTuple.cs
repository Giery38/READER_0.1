using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.ViewModel.tools
{
    public class BindingTuple : ViewModeToolsBase
    {       
        public BindingTuple(object item1, object item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
        private object item1;
        public object Item1
        {
            get
            {
                return item1;
            }
            set
            {
                item1 = value;
                OnPropertyChanged(nameof(Item1));
            }
        }
        private object item2;
        public object Item2
        {
            get
            {
                return item2;
            }
            set
            {
                item2 = value;
                OnPropertyChanged(nameof(Item2));
            }
        }
    }
}
