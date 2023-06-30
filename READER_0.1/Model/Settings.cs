using READER_0._1.Model.Exel.Settings;
using READER_0._1.Model.Settings.Word;
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
        private ExelSettings exelSettings;
        public ExelSettings ExelSettings
        {
            get
            {
                return exelSettings;
            }
            set
            {
                exelSettings = value;                
            }
        }
        private WordSettingsRead wordSettingsRead;
        public WordSettingsRead WordSettingsRead
        {
            get
            {
                return wordSettingsRead;
            }
            set
            {
                wordSettingsRead = value;                
            }
        }
        [XmlIgnore]
        private string configFilePath;        
        public Settings()
        {
            WordSettingsRead = new WordSettingsRead();            
            ExelSettings = new ExelSettings();
            
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
