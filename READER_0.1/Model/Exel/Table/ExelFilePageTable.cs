using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static READER_0._1.Model.Settings.ExelSettingsRead;

namespace READER_0._1.Model.Exel
{
    public class ExelFilePageTable
    {
        public Dictionary<string, int> TableColumns { get; private set; }        
        public Dictionary<Tuple<int,int>, object> TableCells { get; private set; } // item 1 = row item2 = column //переделать
        public List<MergeCell> MergeCells { get; private set; } 

        public ExelFilePageTable(List<string> NameColumns)
        {
            TableColumns = InitializeTableColumns(NameColumns);
            TableCells = new Dictionary<Tuple<int, int>, object>();
            MergeCells = new List<MergeCell>();
        }
        public ExelFilePageTable()
        {
            TableColumns = new Dictionary<string, int>();
            TableCells = new Dictionary<Tuple<int, int>, object>();
            MergeCells = new List<MergeCell>();
        }

        public List<object> GetColumn(string nameColumn)
        {
            List<object> columnData = new List<object>();
            int columnNumber = 0;
            if (TableColumns.TryGetValue(nameColumn, out columnNumber) == true);
            {
                int lastRow = 0;
                if (TableCells.Count > 0 && TableColumns.Count > 0)
                {
                    lastRow = TableCells.Count / TableColumns.Count;
                }
                else
                {
                    return null;
                }
                for (int i = 0; i < lastRow; i++)
                {
                    object tempObject = new object();
                    TableCells.TryGetValue(new Tuple<int, int>(i, columnNumber),out tempObject);
                    columnData.Add(tempObject);
                }                
            }
            return columnData;
        }
        public void AddMergeCells(List<MergeCell> mergeCells)
        {
            MergeCells.AddRange(mergeCells);
        }


        public List<object> GetColumnNoDplicates(string nameColumn)
        {
            List<object> columnData = GetColumn(nameColumn);
            List<object> columnDataNoDplicates = columnData.Distinct().ToList();
            return columnDataNoDplicates;
        }
        public List<object> GetColumnDplicates(string nameColumn)
        {
            List<object> columnData = GetColumn(nameColumn);
            List<object> columnDataDplicate = columnData.GroupBy(s => s).SelectMany(grp => grp.Skip(1)).ToList();
            return columnDataDplicate;
        }       

        public List<object> GetRow(int numberRow)
        {
            List<object> rowData = new List<object>();
            for (int i = 0; i < TableColumns.Count; i++)
            {
                object tempObject = new object();
                TableCells.TryGetValue(new Tuple<int, int>(numberRow,i),out tempObject);
                rowData.Add(tempObject);
            }
            return rowData;
        }

        public void RemoveRow(int rowNumber)
        {
            for (int i = 0; i < TableColumns.Count; i++)
            {
                TableCells.Remove(new Tuple<int, int>(rowNumber,i));
            }
        }

        public void RemoveNullRow()
        {
            int lastRow = 0;
            if (TableCells.Count > 0 && TableColumns.Count > 0)
            {
                lastRow = TableCells.Count / TableColumns.Count;
            }
            //List<int> nullRow = new List<int>();
            for (int row = 0; row < lastRow; row++)
            {
                int nullRowItemCount = 0;
                for (int column = 0; column < TableColumns.Count; column++)
                {                    
                    object tempCell = new object();
                    TableCells.TryGetValue(new Tuple<int, int>(row, column), out tempCell);
                    if(tempCell == null || tempCell.ToString() == "")
                    {
                        nullRowItemCount++;
                    }
                }
                if (nullRowItemCount == TableColumns.Count)
                {
                    RemoveRow(row);
                }
            }
        }

        private Dictionary<string, int> InitializeTableColumns(List<string> NameColumns)
        {
            Dictionary<string, int> tableColumns = new Dictionary<string, int>();
            for (int i = 0; i < NameColumns.Count; i++)
            {
                tableColumns.TryAdd(NameColumns[i],i);
            }
            return tableColumns;
        }
        public void AddColumn(List<object> columnData, string nameColumn)
        {
            int columnNomber = 0;
            TableColumns.TryGetValue(nameColumn, out columnNomber);
            for (int i = 0; i < columnData.Count; i++)
            {
                TableCells.TryAdd(new Tuple<int, int>(i, columnNomber), columnData[i]);
            }
        }
    }
}
