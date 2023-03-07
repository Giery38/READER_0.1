using READER_0._1.Command;
using READER_0._1.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace READER_0._1.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly Navigation.Navigation navigation;
        private readonly WindowFileBase windowFileBase;
        public ViewModelBase CurrentViewModel => navigation.CurrentViewModel;
        public ICommand ShiftingInExelViewModelCommand { get; }
        public ICommand ShiftingInWordViewModelCommand { get; }
        public ICommand ShiftingInSettingsViewModelCommand { get; }
        public MainViewModel(WindowFileBase windowFileBase, Navigation.Navigation navigation)
        {            
            this.navigation = navigation;
            this.windowFileBase = windowFileBase; 
            ShiftingInExelViewModelCommand = new NavigateCommand(new ExelViewModel(windowFileBase), navigation);
            ShiftingInWordViewModelCommand = new NavigateCommand(new WordViewModel(windowFileBase), navigation);
            ShiftingInSettingsViewModelCommand = new NavigateCommand(new SettingsViewModel(windowFileBase.settings), navigation);
            navigation.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
