using READER_0._1.Model.Exel.Settings;
using READER_0._1.ViewModel.Settings;
using READER_0._1.ViewModel.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Command.Settings.SettingsExel
{
    class RemoveSelectedSearchingColumnNameValueCommand : CommandBase
    {
        private readonly SettingsExelViewModel settingsExelViewModel;
        private readonly ExelSettings settings;
        public RemoveSelectedSearchingColumnNameValueCommand(SettingsExelViewModel settingsExelViewModel, ExelSettings settings)
        {
            this.settingsExelViewModel = settingsExelViewModel;
            this.settings = settings;
        }

        public override void Execute(object parameter)
        {
            string value = parameter as string;
            StringContainer stringContainer = settingsExelViewModel.SelectedSearchingColumnNameValues.FirstOrDefault(item => item.Value == value);
            settingsExelViewModel.SelectedSearchingColumnNameValues.Remove(stringContainer);
            settingsExelViewModel.SelectedSearchingColumnName.Values.Remove(value);
            settings.ExelSettingsRead.SearchingColumnNames.Find(item => item == settingsExelViewModel.SelectedSearchingColumnName).Values.Remove(value);
        }
    }
}
