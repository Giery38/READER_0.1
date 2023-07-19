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
    class RemoveSearchingFormatCommand : CommandBase
    {
        private readonly SettingsExelViewModel settingsExelViewModel;
        private readonly ExelSettings settings;
        public RemoveSearchingFormatCommand(SettingsExelViewModel settingsExelViewModel, ExelSettings settings)
        {
            this.settingsExelViewModel = settingsExelViewModel;
            this.settings = settings;
        }

        public override void Execute(object parameter)
        {
           string value = parameter as string;
           StringContainer stringContainer = settingsExelViewModel.SearchingFormats.FirstOrDefault(item => item.Value == value);
           settingsExelViewModel.SearchingFormats.Remove(stringContainer);
           settings.ExelSettingsSearchFiles.FormatsSearch.Remove(value);
        }
    }
}
