using READER_0._1.Model.Exel.Table;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model.Excel.TableData
{
    public class Row
    {       
        public List<Cell> RowData { get; private set; }
        public int Number { get; private set; }
        public Row(List<Cell> rowData, int number)
        {
            RowData = rowData;
            Number = number;
        }
        public Row(List<object> rowData, int number)
        {
            for (int i = 0; i < rowData.Count; i++)
            {
                RowData.Add(new Cell(rowData[i], i));
            }            
            Number = number;
        }
        public Row(int number)
        {
            RowData = new List<Cell>();
            Number = number;
        }
        public void AddRowData(object AddedObject, int position)
        {
            RowData.Add(new Cell(AddedObject, position));
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
        public void SetRowData(List<object> rowData)
        {
            for (int i = 0; i < rowData.Count; i++)
            {
                RowData.Add(new Cell(rowData[i], i));
            }            
        }
        
        public override bool Equals(object obj)
        {
          
            if ((obj is Row other) == false)
            {
                return false;
            }

           
            if (RowData.Count != other.RowData.Count)
            {
                return false;
            }

            for (int i = 0; i < RowData.Count; i++)
            {
                if (RowData[i]?.ToString().Equals(other.RowData[i]?.ToString()) == false)
                {
                    return false;
                }
            }

            return true;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                foreach (object item in RowData)
                {
                    hash = hash * 23 + (item != null ? item.GetHashCode() : 0);
                }

                return hash;
            }
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
