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
    class AddSearchingColumnNameCommand : CommandBase
    {
        private readonly SettingsExelViewModel settingsExelViewModel;
        private readonly ExelSettings settings;
        public AddSearchingColumnNameCommand(SettingsExelViewModel settingsExelViewModel, ExelSettings settings)
        {
            this.settingsExelViewModel = settingsExelViewModel;
            this.settings = settings;
        }
        public override void Execute(object parameter)
        {
            SearchingColumnName addedSearchingColumnName = new SearchingColumnName();
            settingsExelViewModel.SearchingColumnNames.Add(addedSearchingColumnName);
            settings.ExelSettingsRead.SearchingColumnNames.Add(addedSearchingColumnName);
        }
    }
}
