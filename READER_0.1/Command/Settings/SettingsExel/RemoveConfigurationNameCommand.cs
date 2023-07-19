using READER_0._1.Model.Exel.Settings;
using READER_0._1.ViewModel.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static READER_0._1.Model.Exel.Settings.ExelSettingsSearchFiles;

namespace READER_0._1.Command.Settings.SettingsExel
{
    class RemoveConfigurationNameCommand : CommandBase
    {
        private readonly SettingsExelViewModel settingsExelViewModel;
        private readonly ExelSettings settings;
        public RemoveConfigurationNameCommand(SettingsExelViewModel settingsExelViewModel, ExelSettings settings)
        {
            this.settingsExelViewModel = settingsExelViewModel;
            this.settings = settings;
        }

        public override void Execute(object parameter)
        {
            string value = parameter as string;
            ConfigurationName configurationName = settingsExelViewModel.ConfigurationNames.FirstOrDefault(item => item.Name == value);            
            settingsExelViewModel.ConfigurationNames.Remove(configurationName);
            settings.ExelSettingsSearchFiles.Configurations.Remove(configurationName);
        }
    }
}
