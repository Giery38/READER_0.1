using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model.Word
{
    public class Paragraph
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
        public Paragraph()
        {
            StatementTitle = new List<string>();
        }
        public Paragraph(string title)
        {
            Title = title;
            StatementTitle = new List<string>();
        }
    }
}
