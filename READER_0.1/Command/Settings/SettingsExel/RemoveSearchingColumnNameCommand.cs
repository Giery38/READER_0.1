using Microsoft.Xaml.Behaviors.Media;
using READER_0._1.Model.Exel.Settings;
using READER_0._1.ViewModel.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static READER_0._1.Model.Exel.Settings.ExelSettingsRead;

namespace READER_0._1.Command.Settings.SettingsExel
{
    class RemoveSearchingColumnNameCommand : CommandBase
    {
        private readonly SettingsExelViewModel settingsExelViewModel;
        private readonly ExelSettings settings;
        public RemoveSearchingColumnNameCommand(SettingsExelViewModel settingsExelViewModel, ExelSettings settings)
        {
            this.settingsExelViewModel = settingsExelViewModel;
            this.settings = settings;
        }

        public override void Execute(object parameter)
        {
            string value = parameter as string;
            SearchingColumnName searchingColumnName = settingsExelViewModel.SearchingColumnNames.FirstOrDefault(item => item.Name == value);
            if (settingsExelViewModel.SelectedSearchingColumnName == searchingColumnName)
            {
                settingsExelViewModel.SelectedSearchingColumnNameValues.Clear();
            }
            settingsExelViewModel.SearchingColumnNames.Remove(searchingColumnName);
            settings.ExelSettingsRead.SearchingColumnNames.Remove(searchingColumnName);
        }
    }
}
