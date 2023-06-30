using READER_0._1.Command;
using READER_0._1.Model;
using READER_0._1.Model.Settings;
using READER_0._1.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace READER_0._1.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly Navigation.Navigation settingsNavigation;
        private readonly Settings settings;
        public ViewModelBase CurrentSettingsViewModel => settingsNavigation.CurrentViewModel;
        public ICommand ShiftingInExelSettingViewModelCommand { get; }        
        public SettingsViewModel(Settings settings)
        {
            this.settings = settings;
            settingsNavigation = new Navigation.Navigation();
            settingsNavigation.CurrentViewModel = new ExelViewModel(new WindowFileBase("", settings));
           // ShiftingInExelSettingViewModelCommand = new NavigateCommand(new ExelViewModel(new WindowFileBase("",settings)), navigation);
        }       
    }
}
