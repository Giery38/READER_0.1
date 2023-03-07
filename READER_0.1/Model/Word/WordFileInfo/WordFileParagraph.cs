using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model.Word
{
    public class WordFileParagraph
    {
        private string title;
        public string Title 
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            } 
        }
        private List<string> statementTitle;
        public List<string> StatementTitle
        {
            get
            {
                return statementTitle;
            }
            set
            {
                statementTitle = value;
            }
        }
        public WordFileParagraph()
        {
            StatementTitle = new List<string>();
        }
        public WordFileParagraph(string title)
        {
            Title = title;
            StatementTitle = new List<string>();
        }
    }
}
