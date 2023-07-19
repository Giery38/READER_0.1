using READER_0._1.ViewModel.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Command.Settings
{
    public class SaveSettingsCommand : CommandBase
    {
        private readonly SettingsViewModel settingsViewModel;
        private readonly Model.Settings.Settings settings;
        public SaveSettingsCommand(SettingsViewModel settingsViewModel, Model.Settings.Settings settings)
        {
            this.settingsViewModel = settingsViewModel;
            this.settings = settings;
        }
        public override void Execute(object parameter)
        {           
            settings.SaveSettings();
        }
    }
}
