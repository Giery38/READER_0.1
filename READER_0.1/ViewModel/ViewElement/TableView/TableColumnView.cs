using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.ViewModel.ViewElement.TableView
{
    public class TableColumnView : ViewModelBase
    {        
        public ObservableCollection<object> ColumnData { get; private set; }

        public string Name { get; private set; }
        public TableColumnView(List<object> columnData, string name)
        {
            ColumnData = new ObservableCollection<object>(columnData);
            Name = name;
        }
    }
}
