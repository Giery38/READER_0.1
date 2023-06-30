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
        private List<SearchString> mainSearchStrings;
        public List<SearchString> MainSearchStrings
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
      
        private List<SearchString> searchStrings;
        public List<SearchString> SearchStrings
        {
            get
            {
                return searchStrings;
            }
            set
            {
                searchStrings = value;
            }
        }
        public SearchParagraph()
        {
            SearchStrings = new List<SearchString>();
            MainSearchStrings = new List<SearchString>();
        }
        public List<SearchString> GetSearchStrings(string keyWord)
        {           
            return GetStrings(keyWord,SearchStrings);
        }
        public List<SearchString> GetMainStrings(string keyWord)
        {
            return GetStrings(keyWord, MainSearchStrings);
        }
        public List<SearchString> GetStrings(string keyWord, List<SearchString> MainList)
        {
            List<SearchString> strings = new List<SearchString>();
            for (int i = 0; i < MainList.Count; i++)
            {
                if (MainList[i].KeyWords.Contains(keyWord) == true)
                {
                    strings.Add(MainList[i]);
                }
            }
            return strings;
        }
    }
}
