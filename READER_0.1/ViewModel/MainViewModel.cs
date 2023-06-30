using READER_0._1.Command;
using READER_0._1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public MainViewModel(WindowFileBase windowFileBase)
        {
            //ExelViewModel exelViewModel = new ExelViewModel(windowFileBase);
            WordViewModel wordViewModel = new WordViewModel(windowFileBase);
            navigation = new Navigation.Navigation();           
            this.windowFileBase = windowFileBase;          
            if (navigation.CurrentViewModel == null)
            {
                navigation.CurrentViewModel = wordViewModel;
                //создать homePage
            }
            ShiftingInExelViewModelCommand = new NavigateCommand(new ExelViewModel(windowFileBase), navigation);
            //ShiftingInExelViewModelCommand = new NavigateCommand(exelViewModel, navigation);
            ShiftingInWordViewModelCommand = new NavigateCommand(wordViewModel, navigation); 
            //ShiftingInWordViewModelCommand = new NavigateCommand(new WordViewModel(windowFileBase), navigation);         
            ShiftingInSettingsViewModelCommand = new NavigateCommand(new SettingsViewModel(windowFileBase.settings), navigation);
            //
            navigation.CurrentViewModelChanged += Navigation_CurrentViewModelChanged;
        }

        private void Navigation_CurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }              
    }
}
