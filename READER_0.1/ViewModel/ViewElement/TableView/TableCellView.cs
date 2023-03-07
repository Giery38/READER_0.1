using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.ViewModel.ViewElement.TableView
{
    public class TableCellView : ViewModelBase
    {
        public object CellData { get; private set; }

        private double height;
        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                OnPropertyChanged(nameof(Height));
            }
        }
        private double width;
        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                OnPropertyChanged(nameof(Width));
            }
        }
        private int id;
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                OnPropertyChanged(nameof(Width));
            }
        }
        public string Tag { get; private set; }
        public TableCellView(object cellData, string tag)
        {
            CellData = cellData;
            Tag = tag;           
        }
        public TableCellView(object cellData)
        {
            CellData = cellData;
        }
        public TableCellView(object cellData, int id)
        {
            CellData = cellData;
            Id = id;
        }

    }
}
