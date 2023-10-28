using READER_0._1.Model.Excel.Settings;
using READER_0._1.Model.Settings.Word;
using READER_0._1.Model.Word.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace READER_0._1.Model.Settings
{
    [Serializable]
    public class Settings
    {        
        private  ExcelSettings excelSettings;
        public  ExcelSettings ExcelSettings
        {
            get
            {
                return excelSettings;
            }
            set
            {
                excelSettings = value;                
            }
        }
        private WordSettings wordSettings;
        public WordSettings WordSettings
        {
            get
            {
                return wordSettings;
            }
            set
            {
                wordSettings = value;                
            }
        }
        [XmlIgnore]
        private string configFilePath;        
        public Settings()
        {
            WordSettings = new WordSettings();            
            ExcelSettings = new ExcelSettings();
            
        }        
        public void SetConfigFilePath(string configFilePath)
        {
            this.configFilePath = configFilePath;
           
        }
        public void SaveSettings()
        {
            FileStream stream = new FileStream(configFilePath, FileMode.OpenOrCreate);
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            serializer.Serialize(stream, this);
        }       
    }
}
