using READER_0._1.Model.Settings.Word;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace READER_0._1.Converters
{
    public class SearchWordsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string targetValue = parameter as string;
            if (value is SearchString searchString && targetValue != null)
            {
                // Найдите элемент списка SearchWords с заданным значением поля SearchWords
                SearchWord targetItem = searchString.SearchWords.FirstOrDefault(item => item.Name == targetValue);
                return targetItem?.Data;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
