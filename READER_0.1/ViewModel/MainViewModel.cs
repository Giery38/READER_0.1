using READER_0._1.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace READER_0._1.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        public ViewModelBase CurrentViewModel { get; }
        public MainViewModel(WindowFileBase windowFileBase)
        {
            CurrentViewModel = new ExelViewModel(windowFileBase);
        }
    }
}
