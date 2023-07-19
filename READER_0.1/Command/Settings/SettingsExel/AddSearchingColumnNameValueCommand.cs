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
    class AddSearchingColumnNameValueCommand : CommandBase
    {
        private readonly SettingsExelViewModel settingsExelViewModel;
        private readonly ExelSettings settings;
        public AddSearchingColumnNameValueCommand(SettingsExelViewModel settingsExelViewModel, ExelSettings settings)
        {
            this.settingsExelViewModel = settingsExelViewModel;
            this.settings = settings;
        }
        public override void Execute(object parameter)
        {           
            settingsExelViewModel.SelectedSearchingColumnNameValues.Add(new StringContainer("", settingsExelViewModel.SelectedSearchingColumnName.Values));
            settings.ExelSettingsRead.SearchingColumnNames.Find(item => item == settingsExelViewModel.SelectedSearchingColumnName).Values.Add("");
        }
    }
}