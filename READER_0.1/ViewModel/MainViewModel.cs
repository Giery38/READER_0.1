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
        private List<ICommand> shiftingCommands = new List<ICommand>();
        public MainViewModel(WindowFileBase windowFileBase)
        {
            ExelViewModel exelViewModel = new ExelViewModel(windowFileBase);            
            navigation = new Navigation.Navigation();           
            this.windowFileBase = windowFileBase;          
            if (navigation.CurrentViewModel == null)
            {
                navigation.CurrentViewModel = exelViewModel;
                //создать homePage
            }
            //ShiftingInExelViewModelCommand = new NavigateCommand(new ExelViewModel(windowFileBase), navigation);
            ShiftingInExelViewModelCommand = new NavigateCommand(exelViewModel, navigation);
            shiftingCommands.Add(ShiftingInExelViewModelCommand);
            ShiftingInWordViewModelCommand = new NavigateCommand(new WordViewModel(windowFileBase), navigation);
            shiftingCommands.Add(ShiftingInWordViewModelCommand);
            ShiftingInSettingsViewModelCommand = new NavigateCommand(new SettingsViewModel(windowFileBase.settings), navigation);
            shiftingCommands.Add(ShiftingInSettingsViewModelCommand);
            navigation.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
            shiftingCommands.OfType<NavigateCommand>().ToList().ForEach(item => item.OnCanExecutedChanged());
        }

        public override void Deactivation()
        {
            throw new NotImplementedException();
        }
    }
}
