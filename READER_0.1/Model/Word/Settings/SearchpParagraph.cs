using DocumentFormat.OpenXml.Drawing.Diagrams;
using READER_0._1.Model.Word.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model.Settings.Word
{
    [Serializable]
    public class SearchParagraph 
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
        private List<MainSearchString> mainSearchStrings;
        public List<MainSearchString> MainSearchStrings
        {
            get
            {
                return mainSearchStrings;
            }
            set
            {
                mainSearchStrings = value;
            }
        }       
        private List<SearchString> subSearchStrings;
        public List<SearchString> SubSearchStrings
        {
            get
            {
                return subSearchStrings;
            }
            set
            {
                subSearchStrings = value;
            }
        }       
        public SearchParagraph()
        {
            SubSearchStrings = new List<SearchString>();
            MainSearchStrings = new List<MainSearchString>();
        }
        public List<SearchString> GetSubSearchStrings(string keyWord)
        {
            List<SearchString> strings = new List<SearchString>();
            for (int i = 0; i < SubSearchStrings.Count; i++)
            {
                if (SubSearchStrings[i].KeyWords.Contains(keyWord) == true)
                {
                    strings.Add(SubSearchStrings[i]);
                }
            }
            return strings;
        }
        public List<MainSearchString> GetMainStrings(string keyWord)
        {
            List<MainSearchString> strings = new List<MainSearchString>();
            for (int i = 0; i < MainSearchStrings.Count; i++)
            {
                if (MainSearchStrings[i].KeyWords.Contains(keyWord) == true)
                {
                    strings.Add(MainSearchStrings[i]);
                }
            }
            return strings;
        }        
    }
}
