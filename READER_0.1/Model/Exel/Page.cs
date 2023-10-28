using DevExpress.DirectX.Common;
using Microsoft.Xaml.Behaviors.Media;
using READER_0._1.Model.Excel.TableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace READER_0._1.Model.Excel
{
    public class Page
    {
        public string WorksheetName { get; private set; }
        public List<Table> Tables { get; private set; }        
        public Page(string worksheetName)
        {
            WorksheetName = worksheetName;
            Tables = new List<Table>();           
        }

        public void AddTable(List<Table> AddedTabels)
        {
            Tables.AddRange(AddedTabels);
        }
        public void AddTable(Table AddedTabel)
        {
            Tables.Add(AddedTabel);
        }
        public List<string> GetColumnsNameAllTabels()
        {
            List<string> columnsName = new List<string>();
            foreach (Table table in Tables)
            {
                columnsName.AddRange(table.TableColumns.Keys);
            }
            return columnsName.Distinct().ToList();
        }

        public List<object> GetColumnsData(string nameColumn)
        {
            List<object> columnsData = new List<object>();
            for (int i = 0; i < Tables.Count; i++)
            {                
                columnsData.AddRange(Tables[i].GetColumn(nameColumn));
            }
            return columnsData;
        }
        public List<object> GetColumnsDataNoDuplicates(string nameColumn)
        {
            List<object> columnsData = new List<object>();
            for (int i = 0; i < Tables.Count; i++)
            {
                columnsData.AddRange(Tables[i].GetColumnNoDuplicates(nameColumn));
            }

            return new HashSet<object>(columnsData).ToList();
        }
        public List<object> GetColumnsDataDuplicates(string nameColumn)
        {
            List<object> columnsData = new List<object>();
            for (int i = 0; i < Tables.Count; i++)
            {
                columnsData.AddRange(Tables[i].GetColumn(nameColumn));
            }
            List<object> columnDataDuplicate = columnsData.GroupBy(s => s).SelectMany(grp => grp.Skip(1)).ToList();
            return columnDataDuplicate;
        }            
        /*
        public Table UnitedTables() 
        {
            var startTime = System.Diagnostics.Stopwatch.StartNew();
            Table table = new Table();
            for (int tabele = 0; tabele < Tables.Count; tabele++)
            {
                Table table = Tables[tabele];
                foreach (string column in table.TableColumns.Keys)
                {
                    if (table.TableColumns.ContainsKey(column) == false)
                    {
                       table.AddColumn(column);
                    }
                }
                List<(int truePosotion, int newPosition)> positions = new List<(int truePosotion, int newPosition)>();
                bool positionsEquals = true;
                foreach (string column in table.TableColumns.Keys)
                {
                    positions.Add((table.TableColumns[column], table.TableColumns[column]));
                    if (positions.Last().newPosition != positions.Last().truePosotion)
                    {
                        positionsEquals = false;                      
                    }
                }                
                if (positionsEquals == true)
                {
                    for (int row = 0; row < table.Rows.Count; row++)
                    {
                        table.AddRow(table.Rows[row]);
                    }
                }                
                else
                {
                    for (int row = 0; row < table.Rows.Count; row++)
                    {
                        Row tempRow = new Row(new List<object>(), table.Rows[row].Number);
                        object[] rowData = new object[table.TableColumns.Count];
                        for (int i = 0; i < table.TableColumns.Count; i++)
                        {
                            if (table.Rows[row].RowData.Count > i)
                            {
                                int newPosition = positions[i].newPosition;
                                rowData[newPosition] = table.Rows[row].RowData[i];
                            }
                        }
                        tempRow.SetRowData(rowData.ToList());
                        table.AddRow(tempRow);
                    }
                }               
            }            
            startTime.Stop();
            return table;
        }
       */
    }
}
