using READER_0._1.Tools;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace READER_0._1.Model.Settings.Exel
{
    [Serializable]
    public class ExelSettingsRead
    {      
        private bool multiWorksheet;
        public bool MultiWorksheet
        {
            get 
            {
                return multiWorksheet;
            }
            set
            {
                multiWorksheet = value;
            }
        }
        private string searchableColumn;
        public string SearchableColumn 
        {
            get
            {
                return searchableColumn;
            }
            set
            {
                searchableColumn = value;
            }
        }
        private SerializableDictionary<string, List<string>> searchingColumnName;
        public SerializableDictionary<string, List<string>> SearchingColumnName
        {
            get
            {
                return searchingColumnName;
            }
            set
            {
                searchingColumnName = value;
            }
        }
      

        public ExelSettingsRead()
        {
            SearchingColumnName = new SerializableDictionary<string, List<string>>();
            MultiWorksheet = true;
        }        
    }
}
