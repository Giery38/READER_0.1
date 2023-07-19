using DocumentFormat.OpenXml.Drawing.Diagrams;
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
      
        private List<TypeSearchStrings> typesSearchStrings;
        public List<TypeSearchStrings> TypesSearchStrings
        {
            get
            {
                return typesSearchStrings;
            }
            set
            {
                typesSearchStrings = value;
            }
        }
        public SearchParagraph()
        {
            TypesSearchStrings = new List<TypeSearchStrings>();
            MainSearchStrings = new List<SearchString>();
        }
        public List<SearchString> GetSearchStrings(string keyWord)
        {
            List<SearchString> value = new List<SearchString>();
            foreach (TypeSearchStrings item in TypesSearchStrings)
            {
                value.AddRange(item.SearchStrings);
            }
            var tt = GetStrings(keyWord, value);
            if (tt == null)
            {

            }
            return GetStrings(keyWord, value);
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
        public TypeSearchStrings GetTypeSearchString(SearchString searchString)
        {
            foreach (TypeSearchStrings type in TypesSearchStrings)
            {
                if (type.SearchStrings.Find(item => item == searchString) != null)
                {
                    return type;
                }
            }
            return null;
        }
        public class TypeSearchStrings 
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
            private bool active;
            public bool Active
            {
                get
                {
                    return active;
                }
                set
                {                    
                    active = value;
                    foreach (SearchString item in SearchStrings)
                    {
                        item.Active = value;
                    }
                }
            }
            private (string nameSearchWord,string value) replacement;
            public (string nameSearchWord, string value) Replacement
            {
                get
                {
                    return replacement;
                }
                set
                {
                    replacement = value;
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
        }
    }
}
