using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace READER_0._1.Model.Settings.Word
{
    [Serializable]
    public class WordSettingsRead
    {         
        private List<SearchParagraph> searchParagraphs;
        public List<SearchParagraph> SearchParagraphs
        {
            get
            {
                return searchParagraphs;
            }
            set
            {
                searchParagraphs = value;
            }
        }
        private SearchString baseSearchString;
        public SearchString BaseSearchString
        {
            get
            {
                return baseSearchString;
            }
            set
            {
                baseSearchString = value;
            }
        }
        public WordSettingsRead()
        {
            SearchParagraphs = new List<SearchParagraph>();
        }
    }
}
