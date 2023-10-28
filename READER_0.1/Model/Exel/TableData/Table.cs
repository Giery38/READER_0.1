using DocumentFormat.OpenXml.Office.CustomUI;
using READER_0._1.Model.Exel.Table;
using READER_0._1.Model.Settings.Word;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;

namespace READER_0._1.Model.Excel.TableData
{
    public class Table
    {
        public Dictionary<string, int> TableColumns { get; private set; }              
        public List<Row> Rows {get; private set; }
        public List<MergeCell> MergeCells { get; private set; } 
        public Range RangeBody { get; private set; }

        public Table(List<string> NameColumns)
        {
            TableColumns = InitializeTableColumns(NameColumns);
            Rows = new List<Row>();          
            MergeCells = new List<MergeCell>();
            RangeBody = new Range();
        }
        public Table()
        {
            TableColumns = new Dictionary<string, int>();           
            MergeCells = new List<MergeCell>();
            Rows = new List<Row>();
        }      
        public DataTable ToDataTable()
        {
            DataTable dataTable = new DataTable();
            foreach (string columnName in TableColumns.Keys)
            {
                dataTable.Columns.Add(columnName, typeof(string));
            }
            
            for (int row = 0; row < Rows.Count; row++)
            {
                DataRow dataRow = dataTable.NewRow();                
                for (int column = 0; column < dataTable.Columns.Count; column++)
                {
                    dataRow[dataTable.Columns[column].ColumnName] = Rows[row].RowData.Find(item => item.Position == column)?.Data;
                }                
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }
        public List<Row> SearchRowsByColumn(string nameColumn, List<object> searrchingList)
        {
            List<Row> rows = new List<Row>();
            if (TableColumns.TryGetValue(nameColumn, out int indexColumn) == false)
            {
                return rows;
            }
            string searchingValue = "";
            for (int i = 0; i < Rows.Count; i++)
            {
                if (Rows[i].RowData[indexColumn].Data != null)
                {
                    searchingValue = Rows[i].RowData[indexColumn].Data.ToString();
                }
                if (searrchingList.Any(x => x.ToString() == searchingValue) == true)
                {
                    rows.Add(Rows[i]);
                }
            }
            return rows;
        }     
        public Table RemoveDuplicatesByColumn(string columnName)
        {
            Table result = new Table(this.TableColumns.Keys.ToList());
            List<Row> rows = new List<Row>();
            TableColumns.TryGetValue(columnName, out int columnNumber);
            foreach (Row row in Rows)
            {
                var matchingRows = rows.FindAll(item => item.RowData[columnNumber]?.ToString() == row.RowData[columnNumber]?.ToString());
                if (matchingRows.Count == 0)
                {
                    rows.Add(row);
                }               
            }
            result.AddRow(rows);
            return result;
        }
        public void AddRow(List<Row> addedRows)
        {
            Rows.AddRange(addedRows);                    
        } 
        public void AddRow(Row addedRow)
        {            
            Rows.Add(addedRow);           
        }       
        public void AddRow(List<SearchWord> searchWords)
        {           
            foreach (SearchWord searchWord in searchWords)            
            {
                if (TableColumns.Keys.Contains(searchWord.Name) == false)
                {
                    if (TableColumns.Values.Count != 0)
                    {
                        TableColumns.TryAdd(searchWord.Name, TableColumns.Values.Max() + 1);
                    }
                    else
                    {
                        TableColumns.TryAdd(searchWord.Name, 0);
                    }
                }
            }          
            Row row = new Row(Rows.Count);
            foreach (SearchWord searchWord in searchWords)
            {            
                row.AddRowData(searchWord.Data, TableColumns[searchWord.Name]);          
            }
            row.RowData.Sort((a, b) => a.Position.CompareTo(b.Position));
            if (row.RowData.Count < TableColumns.Count)
            {
                for (int i = 0; i < TableColumns.Count; i++)
                {
                    if (row.RowData.Find(item => item.Position == i) == null)
                    {
                        row.RowData.Insert(i, new Cell(null, i));
                    }
                }
            }
            FillingRow(row);
            Rows.Add(row);            
        }       
        private void FillingRow(Row row)
        {
            row.RowData.Sort((a, b) => a.Position.CompareTo(b.Position));
            if (row.RowData.Count < TableColumns.Count)
            {
                for (int i = 0; i < TableColumns.Count; i++)
                {
                    if (row.RowData.Find(item => item.Position == i) == null)
                    {
                        row.RowData.Insert(i, new Cell(null, i));
                    }
                }
            }
        }

        public List<object> GetColumn(string nameColumn)
        {
            List<object> columnData = new List<object>();
            int columnNumber = 0;
            if (TableColumns.TryGetValue(nameColumn, out columnNumber) == true)
            {
                for (int row = 0; row < Rows.Count; row++)
                {
                    columnData.Add(Rows[row].RowData.FirstOrDefault(item => item.Position == columnNumber)?.Data);
                }              
            }
            return columnData;
        }
        public void AddMergeCells(List<MergeCell> mergeCells)
        {
            MergeCells.AddRange(mergeCells);
        }

        public void SetRangeBody(Range rangeBody)
        {
            RangeBody = rangeBody;
        }
        public List<object> GetColumnNoDuplicates(string nameColumn)
        {
            List<object> columnData = GetColumn(nameColumn);
            List<object> columnDataNoDuplicates = new HashSet<object>(columnData).ToList();           
            return columnDataNoDuplicates;
        }
        public List<object> GetColumnDuplicates(string nameColumn)
        {
            List<object> columnData = GetColumn(nameColumn);
            List<object> columnDataDuplicate = columnData.GroupBy(s => s).SelectMany(grp => grp.Skip(1)).ToList();
            return columnDataDuplicate;
        }       
                              
       
        public void RemoveNullRow() //удаление по rownumber сдулать
        {            
            for (int i = 0; i < Rows.Count; i++)
            {
                if (Rows[i].ItsNullRow() == true)
                {
                    Rows[i] = null;
                }
            }
            Rows.RemoveAll(item => item == null);
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
        public void AddDataToColumn(List<object> columnData, string nameColumn)
        {                       
            TableColumns.TryGetValue(nameColumn, out int columnNumber);
            int lastRow = 0;
            if (Rows.Count > 0)
            {
                Cell cell = Rows.Last().RowData.Find(item => item.Position == columnNumber);
                if (cell != null)
                {
                    lastRow = cell.Position + 1;
                }
            }            
            for (int i = lastRow; i < columnData.Count; i++)
            {
                Row row = Rows.Find(item => item.Number == i);                
                if (row == null)
                {
                    row = new Row(i);
                    Rows.Add(row);
                }
                row.AddRowData(columnData[i], columnNumber);
            }           
            
        }
        public void AddColumn(string nameColumn)
        {
            TableColumns.Add(nameColumn, TableColumns.Count);
        }              
    }
}
