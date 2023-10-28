using READER_0._1.Model.Word.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model.Settings.Word
{
    [Serializable]
    public class SearchWord
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        private object data;
        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }
        private DataTypes type;
        public DataTypes Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;               
            }
        }
        public SearchWord(string name, object data, DataTypes dataType)
        {
            Name = name;
            Data = data;
            Type = dataType;
        }
        public SearchWord(SearchWord searchWord)
        {
            Name = new string(searchWord.Name);
            Data = new string(searchWord.Data?.ToString());
        }
        public SearchWord()
        {

        }
    }
}
