using READER_0._1.Model.Settings.Word;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using static READER_0._1.Model.Settings.Exel.ExelSettingsRead;

namespace READER_0._1.Model.Exel
{
    public class ExelFilePageTable
    {
        public Dictionary<string, int> TableColumns { get; private set; }        
        public Dictionary<Tuple<int,int>, object> TableCells { get; private set; } // item 1 = row item2 = column //переделать
        public List<ExelFilePageTableRow> Rows { get; private set; }
        public List<MergeCell> MergeCells { get; private set; } 
        public Range RangeBody { get; private set; }

        public ExelFilePageTable(List<string> NameColumns)
        {
            TableColumns = InitializeTableColumns(NameColumns);
            Rows = new List<ExelFilePageTableRow>();
            TableCells = new Dictionary<Tuple<int, int>, object>();
            MergeCells = new List<MergeCell>();
            RangeBody = new Range();
        }
        public ExelFilePageTable()
        {
            TableColumns = new Dictionary<string, int>();
            TableCells = new Dictionary<Tuple<int, int>, object>();
            MergeCells = new List<MergeCell>();
            Rows = new List<ExelFilePageTableRow>();
        }
        public DataTable ToDataTabel()
        {
            DataTable dataTable = new DataTable();
            foreach (int columnNumber in TableColumns.Values)
            {
                dataTable.Columns.Add(columnNumber.ToString(), typeof(string));
            }

            for (int row = 0; row < Rows.Count; row++)
            {
                DataRow dataRow = dataTable.NewRow();
                for (int column = 0; column < TableColumns.Count; column++)
                {
                    if (TableCells[new Tuple<int, int>(row, column)] != null)
                    {
                        dataRow[column.ToString()] = TableCells[new Tuple<int, int>(row, column)].ToString();
                    }
                    else
                    {
                        dataRow[column.ToString()] = null;
                    }
                }
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }
        public List<ExelFilePageTableRow> SearchRowsByColumn(string nameColumn, List<object> searrchingList)
        {
            List<ExelFilePageTableRow> rows = new List<ExelFilePageTableRow>();
            if (TableColumns.TryGetValue(nameColumn, out int indexColumn) == false)
            {
                return rows;
            }
            string searchingValue = "";
            for (int i = 0; i < Rows.Count; i++)
            {
                if (Rows[i].RowData[indexColumn] != null)
                {
                    searchingValue = Rows[i].RowData[indexColumn].ToString();
                }
                if (searrchingList.Any(x => x.ToString() == searchingValue) == true)
                {
                    rows.Add(Rows[i]);
                }
            }
            return rows;
        }

        public static List<SearchWord> ShuffleAndInsert(List<SearchWord> list, int count) // test
        {
            var rnd = new Random();

            // меняем местами заданное количество элементов списка
            for (int i = 0; i < count; i++)
            {
                int index1 = rnd.Next(list.Count);
                int index2 = rnd.Next(list.Count);
                var temp = list[index1];
                list[index1] = list[index2];
                list[index2] = temp;
            }

            // генерируем случайное количество новых элементов и вставляем их в список
            int newItemsCount = rnd.Next(1, 6);
            for (int i = 0; i < newItemsCount; i++)
            {
                var newItem = new SearchWord { Data = $"New Data {i}", Name = $"New Name {i}" };
                int insertIndex = rnd.Next(list.Count + 1);
                list.Insert(insertIndex, newItem);
            }

            return list;
        }

        public void AddRow(List<ExelFilePageTableRow> addedRows)
        {
            Rows.AddRange(addedRows);
        }

        public void AddRow(List<SearchWord> searchWords)
        {
            //searchWords = ShuffleAndInsert(searchWords, 4);
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
            var lastRow = Rows.Count;
            foreach (SearchWord searchWord in searchWords)
            {
                TableCells.TryAdd(new Tuple<int, int>(lastRow, TableColumns[searchWord.Name]), searchWord.Data);
            }          
            Rows.Add(ConvertRowInExelFilePageTableRow(lastRow));
            LillingMissingCells();   
        }

        private void LillingMissingCells()
        {
            if (Rows.Count == 0)
            {
                return;
            }
            int max = Rows.OrderByDescending(row => row.RowData.Count).FirstOrDefault().RowData.Count;
            int min = Rows.OrderBy(row => row.RowData.Count).FirstOrDefault().RowData.Count;
            int modifiedRow = -1;
            if (min != max)
            {
                int maxColumn = TableCells.Keys.Max(k => k.Item2);
                int maxRow = TableCells.Keys.Max(k => k.Item1) + 1;
                // Проходимся по каждой строке
                for (int i = 0; i < Rows.Count; i++)
                {
                    modifiedRow = -1;
                    // Проходимся по каждому столбцу для данной строки
                    for (int j = 0; j < TableColumns.Count; j++)
                    {
                        // Если ячейка не заполнена, добавляем ее в словарь со значением null
                        Tuple<int, int> cell = new Tuple<int, int>(i, j);
                        if (!TableCells.ContainsKey(cell))
                        {
                            TableCells.Add(cell, null);
                            modifiedRow = i;
                        }
                    }
                    if (modifiedRow >= 0)
                    {
                        Rows[i] = ConvertRowInExelFilePageTableRow(i);
                    }
                }
            }
        }


        private ExelFilePageTableRow ConvertRowInExelFilePageTableRow(int rowNumber)
        {
            List<object> rowData = new List<object>();
            Tuple<int, int> position;
            for (int i = 0; i < TableColumns.Count; i++)
            {
                position = new Tuple<int, int>(rowNumber, i);
                if (TableCells.Keys.Contains(position) == false)
                {
                    TableCells.TryAdd(position, null);
                }
                rowData.Add(TableCells[new Tuple<int, int>(rowNumber, i)]);
            }            
            return new ExelFilePageTableRow(rowData, rowNumber);
        }

        public List<object> GetColumn(string nameColumn)
        {
            List<object> columnData = new List<object>();
            int columnNumber = 0;
            if (TableColumns.TryGetValue(nameColumn, out columnNumber) == true)
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

        public void SetRangeBody(Range rangeBody)
        {
            RangeBody = rangeBody;
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

        public int GetRowCount()
        {
            return TableCells.Keys.Count / TableColumns.Count;
        }

        public void AddDataInColumn(List<object> Data, string Column)
        {
            int columnNumer;
            TableColumns.TryGetValue(Column, out columnNumer);
            int lastRow = GetLastRowInColumn(Column);
            for (int i = 0; i < Data.Count; i++)
            {
                TableCells.Add(new Tuple<int, int>(i + lastRow, columnNumer), Data[i]);
            }
        }

        private int GetLastRowInColumn(string Column)
        {
            int columnNumer;
            TableColumns.TryGetValue(Column, out columnNumer);
            int row = 0;
            object tempObject = new object();
            while (true)
            {
                if (TableCells.TryGetValue(new Tuple<int, int>(row, columnNumer), out tempObject) == false)
                {
                    break;
                }
                row++;
            }
            return row;
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
        public void AddColumn(List<object> columnData, string nameColumn)
        {
            
            int columnNomber = 0;
            TableColumns.TryGetValue(nameColumn, out columnNomber);
            for (int i = 0; i < columnData.Count; i++)
            {
                TableCells.TryAdd(new Tuple<int, int>(i, columnNomber), columnData[i]);
            }
            
            if (Rows.Count == 0)
            {
                CreateRow(columnData);
            }
            else
            {
                AddAllRowData(columnData);
            }
        }
        private void CreateRow(List<object> columnData)
        {            
            for (int i = 0; i < columnData.Count; i++)
            {
                List<object> tempList = new List<object>();
                tempList.Add(columnData[i]);
                Rows.Add(new ExelFilePageTableRow(tempList, i));
            }
        }
        private void AddAllRowData(List<object> columnData)
        {
            for (int i = 0; i < columnData.Count; i++)
            {
                Rows[i].AddRowData(columnData[i]);
            }
        }
    }
}
