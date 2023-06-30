using System;
using System.Collections.Generic;
using System.Linq;

namespace READER_0._1.Model.Exel
{
    public class ExelFilePage
    {
        public string WorksheetName { get; private set; }
        public List<ExelFilePageTable> Tabeles { get; private set; }        
        public bool SearchingColumn { get; private set; }
        public List<string> ColumnsNameAllTabels { get; private set; }
        public ExelFilePage(string worksheetName)
        {
            WorksheetName = worksheetName;
            Tabeles = new List<ExelFilePageTable>();           
            SearchingColumn = false;
            ColumnsNameAllTabels = new List<string>();
        }

        public void AddTabel(List<ExelFilePageTable> AddedTabels)
        {
            Tabeles.AddRange(AddedTabels);
        }
        public void AddTabel(ExelFilePageTable AddedTabel)
        {
            Tabeles.Add(AddedTabel);
        }
        public List<string> GetColumnsNameAllTabels()
        {
            List<string> columnsName = new List<string>();
            foreach (ExelFilePageTable table in Tabeles)
            {
                columnsName.AddRange(table.TableColumns.Keys);
            }
            return columnsName.Distinct().ToList();
        }

        public List<object> GetColumnsData(string nameColumn)
        {
            List<object> columnsData = new List<object>();
            for (int i = 0; i < Tabeles.Count; i++)
            {                
                columnsData.AddRange(Tabeles[i].GetColumn(nameColumn));
            }
            return columnsData;
        }
        public List<object> GetColumnsDataNoDplicates(string nameColumn)
        {
            List<object> columnsData = new List<object>();
            for (int i = 0; i < Tabeles.Count; i++)
            {
                columnsData.AddRange(Tabeles[i].GetColumnNoDplicates(nameColumn));
            }
            return columnsData;
        }
        public List<object> GetColumnsDataDplicates(string nameColumn)
        {
            List<object> columnsData = new List<object>();
            for (int i = 0; i < Tabeles.Count; i++)
            {
                columnsData.AddRange(Tabeles[i].GetColumnDplicates(nameColumn));
            }
            return columnsData;
        }

        public void SetSearchingColumn(bool searchingResult)
        {
            SearchingColumn = searchingResult;
        }        

        public List<ExelFilePageTable> UnitedTebles()
        {
            List<ExelFilePageTable> tempTables = new List<ExelFilePageTable>(Tabeles);
            List<List<ExelFilePageTable>> tables = new List<List<ExelFilePageTable>>();
            List<ExelFilePageTable> unitedTebles = new List<ExelFilePageTable>();
            while (tempTables.Count > 0)
            {
                foreach (ExelFilePageTable table in tempTables)
                {
                    List<string> names = table.TableColumns.Keys.ToList();
                    List<ExelFilePageTable> equalsColumnTable = Tabeles.FindAll(item => item.TableColumns.Keys.ToList().SequenceEqual(names) == true);
                    tables.Add(equalsColumnTable);
                    tempTables = tempTables.Except(equalsColumnTable).ToList();
                    break;
                }
            }
            foreach (List<ExelFilePageTable> colappsedTableList in tables)
            {
                unitedTebles.Add(CollapsTables(colappsedTableList));
            }
            return unitedTebles;
        }
        private ExelFilePageTable CollapsTables(List<ExelFilePageTable> tables)
        {
            if (tables.Count > 0)
            {
                ExelFilePageTable collapsedTable = new ExelFilePageTable(tables[0].TableColumns.Keys.ToList());
                foreach (ExelFilePageTable table in tables)
                {
                    foreach (string key in table.TableColumns.Keys.ToList())
                    {
                        collapsedTable.AddDataInColumn(table.GetColumn(key), key);
                    }                    
                }
                return collapsedTable;
            }
            return null;
        }
    }
}
