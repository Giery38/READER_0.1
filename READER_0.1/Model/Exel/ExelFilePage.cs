using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static READER_0._1.Model.Settings.ExelSettingsRead;
using Excel = Microsoft.Office.Interop.Excel;

namespace READER_0._1.Model.Exel
{
    public class ExelFilePage
    {
        public Excel.Worksheet Worksheet { get; private set; }
        public string WorksheetName { get; private set; }
        public List<ExelFilePageTable> Tabeles { get; private set; }        
        public bool SearchingColumn { get; private set; }
        public List<string> ColumnsNameAllTabels { get; private set; }
        public ExelFilePage(Worksheet worksheet)
        {
            Worksheet = worksheet;
            WorksheetName = worksheet.Name;
            Tabeles = new List<ExelFilePageTable>();           
            SearchingColumn = false;
            ColumnsNameAllTabels = new List<string>();
        }

        public void AddTabel(List<ExelFilePageTable> AddedTabels)
        {
            Tabeles.AddRange(AddedTabels);
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

        private List<object> SearchDuplicate(List<object> List)
        {
            List<object> listResult = List.GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(y => y.Key)
            .ToList();
            return listResult;
        }
    }
}
