using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.ViewModel
{
    public class ConverterListInListForView
    {
        private string stringValue;
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }

        private ConverterListInListForView(string ConvertedString)
        {
            StringValue = ConvertedString;
        }
        public ConverterListInListForView()
        {
            
        }

        public ConverterListInListForView ConvertValue(string convertedString)
        {
            return new ConverterListInListForView(convertedString);
        }
        public List<ConverterListInListForView> ConvertList(List<string> ConvertedList)
        {
            List<ConverterListInListForView> listResult = new List<ConverterListInListForView>();
            foreach (string item in ConvertedList)
            {
                listResult.Add(new ConverterListInListForView(item));
            }
            return listResult;
        }       
    }
}
