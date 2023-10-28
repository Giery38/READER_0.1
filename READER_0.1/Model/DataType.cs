using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model
{
    public static class DataType
    {
        public static DataTypes GetType(object item, out object result)
        {
            result = item;     
            if (item == null || item.ToString() == null)
            {               
                return DataTypes.None;
            } 
            string value = item.ToString();
            if (long.TryParse(value, out long number) == true)
            {
                result = number;
                return DataTypes.Number;
            }
            if (DateTime.TryParse(value, out DateTime resultDataTime) == true)
            {
                return DataTypes.DataTime;
            }
            List<char> chars = value.ToList();
            if (chars.Contains('.') == true|| chars.Contains(',') == true)
            {
                int lastDotIndex = 0;
                for (int i = 0; i < chars.Count; i++)
                {
                    if (chars[i] == '.' || chars[i] == ',')
                    {
                        chars.RemoveAt(i);
                        lastDotIndex = i;
                    }
                }
                if (lastDotIndex > 0)
                {
                    chars.Insert(lastDotIndex, ',');
                }
                string temp = new string(chars.ToArray());
                if (double.TryParse(temp, out double numberDouble))
                {
                    result = numberDouble;
                    return DataTypes.Number;
                }
            }           
            return DataTypes.String;
        }
        public static object ToType(object value, DataTypes type)
        {
            object result = value;
            if (value == null || value.ToString() == null)
            {
                return null;
            }
            switch (type)
            {                
                case DataTypes.String:
                    result.ToString();
                    break;
                case DataTypes.Number:
                    if(long.TryParse(value.ToString(), out long longNumber))
                    {
                        result = longNumber;
                    }
                    else if (double.TryParse(value.ToString(), out double numberDouble))
                    {
                        result = numberDouble;                      
                    }
                    else if(ToDouble(value, out double doubleNumber))
                    {
                        result = doubleNumber;
                    }                  
                    break;                                   
            }
            return result;
        }
        private static bool ToDouble(object value, out double result)
        {
            result = 0;
            if (value == null || value.ToString() == null)
            {
                return false;
            }
            string temp = value.ToString();
            List<char> chars = temp.ToList();
            if (chars.Contains('.') == true || chars.Contains(',') == true)
            {
                int lastDotIndex = 0;
                for (int i = 0; i < chars.Count; i++)
                {
                    if (chars[i] == '.' || chars[i] == ',')
                    {
                        chars.RemoveAt(i);
                        lastDotIndex = i;
                    }
                }
                if (lastDotIndex > 0)
                {
                    chars.Insert(lastDotIndex, ',');
                }
                temp = new string(chars.ToArray());
                if (double.TryParse(temp, out double numberDouble))
                {
                    result = numberDouble;
                    return true;
                }
            }
            return false;
        }
     
        
    }    
   public enum DataTypes
    {
        None,
        String,
        Number,
        DataTime
    }
}
