using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model.Exel
{
    public class ExelFilePageTableRow
    {       
        public List<object> RowData { get; private set; }
        public int Number { get; private set; }
        public ExelFilePageTableRow(List<object> rowData, int number)
        {
            RowData = rowData;
            Number = number;
        }       
        public void AddRowData(object AddedObject)
        {
            RowData.Add(AddedObject);
        }

        public object SearchCell(object cellData)
        {
            for (int i = 0; i < RowData.Count; i++)
            {
                if (RowData[i] == cellData)
                {
                    return RowData[i];
                }
            }
            return null;
        }

        public bool ItsNullRow()
        {
            int nullCount = 0;
            for (int i = 0; i < RowData.Count; i++)
            {
                if (RowData[i] == null)
                {                    
                    nullCount++;
                }                
            }
            if (nullCount == RowData.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
