using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.ViewModel.tools
{
    public class StringContainer : ViewModeToolsBase
    {
        private List<string> baseList;
        public StringContainer(string value, List<string> baseList)
        {
            this.baseList = baseList;
            Value = value;                        
        }
        private string value;
        public string Value
        {
            get
            {
                return value;                
            }
            set
            {
                int index = -1;
                if (baseList != null)
                {
                    index = baseList.IndexOf(this.value);
                }               
                if (index >= 0)
                {
                    baseList[index] = value;
                }
                this.value = value;                
                OnPropertyChanged(nameof(Value));                
            }
        }
    }
}
