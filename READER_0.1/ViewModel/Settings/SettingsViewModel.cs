using READER_0._1.Command.Settings;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace READER_0._1.ViewModel.Settings
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly Navigation.Navigation settingsNavigation;
        private readonly Model.Settings.Settings settings;
        public ViewModelBase CurrentSettingsViewModel => settingsNavigation.CurrentViewModel;
        //command       
        public ICommand SaveSettingsCommand { get; }

        //obs
        public ObservableCollection<ViewModelBase> SettingsWindows { get; set; }
        public SettingsViewModel(Model.Settings.Settings settings)
        {
            this.settings = settings;
            //vm
            SettingsExelViewModel settingsExelViewModel = new SettingsExelViewModel(settings.ExelSettings)
            {
                Name = "Настройки Excel"
            };
            SettingsWordViewModel settingsWordViewModel = new SettingsWordViewModel(settings.WordSettings)
            {
                Name = "Настройки Word"
            };
            SettingsWindows = new ObservableCollection<ViewModelBase>()
            {
                settingsExelViewModel,
                settingsWordViewModel
            };
            //nav
            settingsNavigation = new Navigation.Navigation();
            settingsNavigation.CurrentViewModel = settingsWordViewModel;           
            //obs
            //command
            SaveSettingsCommand = new SaveSettingsCommand(this,settings);
        }
        private ViewModelBase selectedSettingsWindow;
        public ViewModelBase SelectedSettingsWindow
        {
            get
            {
                return selectedSettingsWindow;
            }
            set
            {
                if (selectedSettingsWindow != value)
                {
                    selectedSettingsWindow = value;
                    OnPropertyChanged(nameof(SelectedSettingsWindow));
                    settingsNavigation.CurrentViewModel = value;
                    OnPropertyChanged(nameof(CurrentSettingsViewModel));
                }               
            }
        }
    }
}
