using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model.Excel.Settings
{
    [Serializable]
    public class ExcelSettings
    {
       private ExcelSettingsRead excelSettingsRead;
       public ExcelSettingsRead ExcelSettingsRead
        {
            get 
            { 
                return excelSettingsRead; 
            }
            set
            {
                excelSettingsRead = value;
            }
        }
        private ExcelSettingsSearchFiles excelSettingsSearchFiles;
        public ExcelSettingsSearchFiles ExcelSettingsSearchFiles
        {
            get 
            {
                return excelSettingsSearchFiles;
            }
            set
            {
                excelSettingsSearchFiles = value;            
            }
        }      
        public ExcelSettings()
        {
            ExcelSettingsRead = new ExcelSettingsRead();
            ExcelSettingsSearchFiles = new ExcelSettingsSearchFiles();
        }
    }
}
