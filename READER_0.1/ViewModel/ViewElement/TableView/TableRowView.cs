using READER_0._1.Model.Exel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.ViewModel.ViewElement.TableView
{
    public class TableRowView : ViewModelBase
    {
        public ObservableCollection<TableCellView> RowCells { get; private set; }
        public int Number { get; private set; }
        public string Tag { get; private set; }
        public TableRowView(ObservableCollection<TableCellView> rowCells, int number)
        {
            RowCells = rowCells;
            Number = number;
        }
        public TableRowView(List<object> rowData, int number)
        {                        
            RowCells = new ObservableCollection<TableCellView>();
            for (int i = 0; i < rowData.Count; i++)
            {
                RowCells.Add(new TableCellView(rowData[i], i));
            }
            Number = number;
        }

        public TableRowView(ExelFilePageTableRow exelFilePageTableRow, string TagTable)
        {
            RowCells = new ObservableCollection<TableCellView>();            
            Number = exelFilePageTableRow.Number + 1;
            Tag = TagTable +":"+ Number.ToString();
            RowCells.Add(new TableCellView(Number));
            /*
            for (int i = 0; i < exelFilePageTableRow.RowData.Count; i++)
            {
                RowCells.Add(new TableCellView(exelFilePageTableRow.RowData[i], Tag + ":" + (i + 1)));
            }
            */
            for (int i = 0; i < exelFilePageTableRow.RowData.Count; i++)
            {
                RowCells.Add(new TableCellView(exelFilePageTableRow.RowData[i], i));
            }
        }
        public void SetWidthCells(double Width)
        {
            for (int i = 0; i < RowCells.Count; i++)
            {
                RowCells[i].Width = Width;
            }
       }

    }
}
