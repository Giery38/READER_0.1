using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace READER_0._1.Converters
{
    public class TupleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return null;
            }
            int index = 0;
            Int32.TryParse(parameter as string, out index);
            if (index > 1 )
            {
                index = 1;
            }
            if (index < 0 )
            {
                index = 0;
            }
            ITuple tuple = value as ITuple;            
            return tuple[index].ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
