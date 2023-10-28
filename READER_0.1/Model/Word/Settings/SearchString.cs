using READER_0._1.Model.Word.Settings;
using READER_0._1.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model.Settings.Word
{
    [Serializable]
    public class SearchString
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
            }
        }
        private List<string> searchStringWords;
        public List<string> SearchStringWords            
        {
            get
            {
                return searchStringWords;
            }
            set
            {
                searchStringWords = value;
            }
        }
        private List<string> keyWords;
        public List<string> KeyWords
        {
            get
            {
                return keyWords;
            }
            set
            {
                keyWords = value;
            }
        }
        private List<SearchWord> searchWords;
        public List<SearchWord> SearchWords
        {
            get
            {
                return searchWords;
            }
            set
            {
                searchWords = value;
            }
        }
       private SerializableDictionary<string, int> positionKeyWords;
       public SerializableDictionary<string, int> PositionKeyWords
        {
            get
            {
                return positionKeyWords;
            }
            set
            {
                positionKeyWords = value;
            }
        }
        private SerializableDictionary<string, int> staticPositionSearchWords;
        public SerializableDictionary<string, int> StaticPositionSearchWords
        {
            get
            {
                return staticPositionSearchWords;
            }
            set
            {
                staticPositionSearchWords = value;
            }
        }
        //ищет слово по его типу
        private List<DynamicSearchWord> dynamicSearchWords;
        public List<DynamicSearchWord> DynamicSearchWords
        {
            get
            {
                return dynamicSearchWords;
            }
            set
            {
                dynamicSearchWords = value;
            }
        }
        private List<DynamicSearchWord> dynamicSearchRanges;
        public List<DynamicSearchWord> DynamicSearchRanges
        {
            get
            {
                return dynamicSearchRanges;
            }
            set
            {
                dynamicSearchRanges = value;
            }
        }
        private SerializableDictionary<int, List<int>> associationsWords;
        public SerializableDictionary<int, List<int>> AssociationsWords
        {
            get
            {
                return associationsWords;
            }
            set
            {
                associationsWords = value;
            }
        }
        private List<(string nameSearchWord, string value)> replacements;
        public List<(string nameSearchWord, string value)> Replacements
        {
            get
            {
                return replacements;
            }
            set
            {
                replacements = value;
            }
        }
        private List<(string baseNameSearchWord, SearchWord value)> additions;
        public List<(string baseNameSearchWord, SearchWord value)> Additions
        {
            get
            {
                return additions;
            }
            set
            {
                additions = value;
            }
        }
        public SearchString()
        {
            SearchStringWords = new List<string>();
            SearchWords = new List<SearchWord>();
            KeyWords = new List<string>();
            PositionKeyWords = new SerializableDictionary<string, int>();
            StaticPositionSearchWords = new SerializableDictionary<string, int>();
            AssociationsWords = new SerializableDictionary<int, List<int>>();
            Replacements = new List<(string nameSearchWord, string value)>();
            Additions = new List<(string baseNameSearchWord, SearchWord value)>();
            DynamicSearchRanges = new List<DynamicSearchWord>();
            DynamicSearchWords = new List<DynamicSearchWord>();
        }
        public SearchString(SearchString searchString)
        {
            SearchStringWords = new List<string>(searchString.SearchStringWords);
            SearchWords = new List<SearchWord>(searchString.SearchWords.Select(item => new SearchWord(item)));
            KeyWords = new List<string>(searchString.KeyWords);
            PositionKeyWords = new SerializableDictionary<string, int>(searchString.PositionKeyWords);
            StaticPositionSearchWords = new SerializableDictionary<string, int>(searchString.StaticPositionSearchWords);
            AssociationsWords = new SerializableDictionary<int, List<int>>(searchString.AssociationsWords);
        }
        public void SetSearchStringWords(string searchString)
        {
            SearchStringWords = searchString.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();            
        }  

        public void CopyName(SearchString copySearchString)
        {
            if (copySearchString.SearchWords.Count == SearchWords.Count)
            {
                for (int i = 0; i < copySearchString.SearchWords.Count; i++)
                {
                    SearchWords[i].Name = copySearchString.SearchWords[i].Name;
                }
            }
        }
        
        public void SetPositionKeyWordsMask()
        {            
            for (int i = 0; i < SearchStringWords.Count; i++)
            {
                if (KeyWords.Contains(SearchStringWords[i]) == true)
                {
                    PositionKeyWords.TryAdd(SearchStringWords[i], i);
                }
            }            
        }
        public void SetPositionSearchWordsMask()
        {
            for (int i = 0; i < SearchStringWords.Count; i++)
            {
                if (SearchWords.Find(item => item.Data == SearchStringWords[i]) != null)
                {
                    StaticPositionSearchWords.TryAdd(SearchStringWords[i], i);
                }
            }
        }
        /// <summary>
        /// Name и Type будет как у первого элемента в списке 
        /// </summary>
        /// <param name="searchWords"></param>
        public static SearchWord CollapseSearchWords(List<SearchWord> searchWords)
        {
            SearchWord firstWord = searchWords.First();
            for (int i = 1; i < searchWords.Count; i++)
            {                
                firstWord.Data = firstWord.Data + " " + searchWords[i].Data;
            }           
            return firstWord;
        }
        public List<int> GetRelativeMaskSearchWords(string keyWord)
        {
            PositionKeyWords.TryGetValue(keyWord, out int indexKeyWord);
            int position = 0;
            List<int> mask = new List<int>();
            foreach (int index in StaticPositionSearchWords.Values)
            {
                position = 0;
                switch (index.CompareTo(indexKeyWord))
                {
                    case 1:
                        position = indexKeyWord - index;
                        position = -position;
                        break;
                    case -1:
                        position = indexKeyWord - index - 1;
                        position = -position;
                        break;
                }
                mask.Add(position);
            }
            return mask;
        }

        public List<int> GetRelativeMaskKeyWords(string keyWord)
        {
            PositionKeyWords.TryGetValue(keyWord, out int indexKeyWord);
            int position = 0;
            List<int> mask = new List<int>();
            foreach (int index in PositionKeyWords.Values)
            {
                position = 0;
                switch (index.CompareTo(indexKeyWord))
                {
                    case 1:
                        position = indexKeyWord - index;
                        position = -position;
                        break;
                    case -1:
                        position = indexKeyWord - index;
                        position = -position;
                        break;                   
                }
                mask.Add(position);
            }
            return mask;
        }
    }
}
