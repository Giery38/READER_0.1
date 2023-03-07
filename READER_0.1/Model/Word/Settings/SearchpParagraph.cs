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
        private SearchString mainSearchString;
        public SearchString MainSearchString
        {
            get
            {
                return mainSearchString;
            }
            set
            {
                mainSearchString = value;
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
        }
        public List<SearchString> GetSearchStrings(string keyWord)
        {
            List<SearchString> searchStrings = new List<SearchString>();
            for (int i = 0; i < SearchStrings.Count; i++)
            {
                if (SearchStrings[i].KeyWords.Contains(keyWord) == true)
                {
                    searchStrings.Add(SearchStrings[i]);
                }             
            }
            return searchStrings;
        }
    }
}
