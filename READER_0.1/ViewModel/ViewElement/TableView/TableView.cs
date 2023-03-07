using READER_0._1.Model.Exel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.ViewModel.ViewElement.TableView
{
    public class TableView : ViewModelBase 
    {
        public ObservableCollection<TableColumnView> Columns { get; private set; }
        public ObservableCollection<TableRowView> Rows { get; private set; }
        public ObservableCollection<string> NameColumns { get; private set; }
        public string Tag { get; private set; }
        public TableView(ObservableCollection<TableColumnView> columns)
        {
            Columns = columns;
            NameColumns =  new ObservableCollection<string>(GetAllNames(columns));
        }
        public TableView(ExelFilePageTable exelFilePageTable)
        {
            /*
            if (exelFilePageTable.Rows.Count == 0)
            {
                Rows = null;
                NameColumns = null;
            }
            Tag = exelFilePageTable.GetHashCode().ToString();
            NameColumns = new ObservableCollection<string>(exelFilePageTable.TableColumns.Keys.ToList());
            Rows = new ObservableCollection<TableRowView>();
            List<object> firstRowData = new List<object>(NameColumns);
            firstRowData.Insert(0, "Номер строки");
            TableRowView firstRow = new TableRowView(firstRowData, 0);
            Rows.Add(firstRow);
            for (int i = 0; i < exelFilePageTable.Rows.Count; i++)
            {
                Rows.Add(new TableRowView(exelFilePageTable.Rows[i], Tag));
            }
            */
            Rows = new ObservableCollection<TableRowView>();
            Columns = new ObservableCollection<TableColumnView>();
            foreach (var item in exelFilePageTable.TableColumns.Keys)
            {
                Columns.Add(new TableColumnView(exelFilePageTable.GetColumn(item),item));                
            }
        }
        public List<TableCellView> Search(object search)
        {
            List<TableCellView> foundCells = new List<TableCellView>();
            var rows = Rows.ToList();
            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                for (int j = 0; j < row.RowCells.Count; j++)
                {
                    var cell = row.RowCells[j];
                    if (cell.CellData.Equals(search))
                    {
                        foundCells.Add(cell);
                    }
                }
            }
            return foundCells;
        }
        public TableCellView GetLongestCellInColumn(int columnNomber)
        {
            int size = 0;
            TableCellView tableCellLongest = new TableCellView("0");
            for (int i = 0; i < Rows.Count; i++)
            {
                if (Rows[i].RowCells[columnNomber].CellData == null)
                {
                    size = 1;
                }
                else
                {
                    size = Rows[i].RowCells[columnNomber].CellData.ToString().Length;
                }
                if (size > tableCellLongest.CellData.ToString().Length)
                {
                    tableCellLongest = Rows[i].RowCells[columnNomber];
                }
            }
            return tableCellLongest;
        }

        public void SetWidthInColumn(int columnNomber, double Width)
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                Rows[i].RowCells[columnNomber].Width = Width;
            }
        }

        private List<string> GetAllNames(ObservableCollection<TableColumnView> columns)
        {
            List<string> names = new List<string>();
            foreach (TableColumnView column in columns)
            {
                names.Add(column.Name);
            }
            return names;
        }

    }
}
