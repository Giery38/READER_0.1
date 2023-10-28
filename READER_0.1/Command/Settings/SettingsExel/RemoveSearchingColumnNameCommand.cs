using Microsoft.Xaml.Behaviors.Media;
using READER_0._1.Model.Excel.Settings;
using READER_0._1.ViewModel.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static READER_0._1.Model.Excel.Settings.ExcelSettingsRead;

namespace READER_0._1.Command.Settings.SettingsExcel
{
    class RemoveSearchingColumnNameCommand : CommandBase
    {
        private readonly SettingsExcelViewModel settingsExcelViewModel;
        private readonly ExcelSettings settings;
        public RemoveSearchingColumnNameCommand(SettingsExcelViewModel settingsExcelViewModel, ExcelSettings settings)
        {
            this.settingsExcelViewModel = settingsExcelViewModel;
            this.settings = settings;
        }

        public override void Execute(object parameter)
        {
            string value = parameter as string;
            SearchingColumnName searchingColumnName = settingsExcelViewModel.SearchingColumnNames.FirstOrDefault(item => item.Name == value);
            if (settingsExcelViewModel.SelectedSearchingColumnName == searchingColumnName)
            {
                settingsExcelViewModel.SelectedSearchingColumnNameValues.Clear();
            }
            settingsExcelViewModel.SearchingColumnNames.Remove(searchingColumnName);
            settings.ExcelSettingsRead.SearchingColumnNames.Remove(searchingColumnName);
        }
    }
}
