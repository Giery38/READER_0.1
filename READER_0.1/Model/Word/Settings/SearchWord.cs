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
        private string data;
        public string Data
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
        public SearchWord(string name, string data)
        {
            Name = name;
            Data = data;
        }
        public SearchWord(SearchWord searchWord)
        {
            Name = new string(searchWord.Name);
            Data = new string(searchWord.Data);
        }
        public SearchWord()
        {

        }
        public void CollapseSearchWord(List<SearchWord> searchWords)
        {
            foreach (SearchWord word in searchWords)
            {
                string result = string.Join(" ", word.Data);
                Data = Data + " " + result;
            }
        }
    }
}
