using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model.Exel.Settings
{
    [Serializable]
    public class ExelSettings
    {
       private ExelSettingsRead exelSettingsRead;
       public ExelSettingsRead ExelSettingsRead
        {
            get 
            { 
                return exelSettingsRead; 
            }
            set
            {
                exelSettingsRead = value;
            }
        }
        private ExelSettingsSearchFiles exelSettingsSearchFiles;
        public ExelSettingsSearchFiles ExelSettingsSearchFiles
        {
            get 
            {
                return exelSettingsSearchFiles;
            }
            set
            {
                exelSettingsSearchFiles = value;            
            }
        }      
        public ExelSettings()
        {
            ExelSettingsRead = new ExelSettingsRead();
            ExelSettingsSearchFiles = new ExelSettingsSearchFiles();
        }
    }
}
