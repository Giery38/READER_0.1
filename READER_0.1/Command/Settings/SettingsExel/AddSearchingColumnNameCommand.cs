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
    class AddSearchingColumnNameCommand : CommandBase
    {
        private readonly SettingsExcelViewModel settingsExcelViewModel;
        private readonly ExcelSettings settings;
        public AddSearchingColumnNameCommand(SettingsExcelViewModel settingsExcelViewModel, ExcelSettings settings)
        {
            this.settingsExcelViewModel = settingsExcelViewModel;
            this.settings = settings;
        }
        public override void Execute(object parameter)
        {
            SearchingColumnName addedSearchingColumnName = new SearchingColumnName();
            settingsExcelViewModel.SearchingColumnNames.Add(addedSearchingColumnName);
            settings.ExcelSettingsRead.SearchingColumnNames.Add(addedSearchingColumnName);
        }
    }
}
