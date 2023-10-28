using READER_0._1.Model.Excel.Settings;
using READER_0._1.ViewModel.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static READER_0._1.Model.Excel.Settings.ExcelSettingsSearchFiles;

namespace READER_0._1.Command.Settings.SettingsExcel
{
    class RemoveConfigurationNameCommand : CommandBase
    {
        private readonly SettingsExcelViewModel settingsExcelViewModel;
        private readonly ExcelSettings settings;
        public RemoveConfigurationNameCommand(SettingsExcelViewModel settingsExcelViewModel, ExcelSettings settings)
        {
            this.settingsExcelViewModel = settingsExcelViewModel;
            this.settings = settings;
        }

        public override void Execute(object parameter)
        {
            string value = parameter as string;
            ConfigurationName configurationName = settingsExcelViewModel.ConfigurationNames.FirstOrDefault(item => item.Name == value);            
            settingsExcelViewModel.ConfigurationNames.Remove(configurationName);
            settings.ExcelSettingsSearchFiles.Configurations.Remove(configurationName);
        }
    }
}
