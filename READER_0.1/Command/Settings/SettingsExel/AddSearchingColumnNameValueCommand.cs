using READER_0._1.Model.Excel.Settings;
using READER_0._1.ViewModel.Settings;
using READER_0._1.ViewModel.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Command.Settings.SettingsExcel
{
    class AddSearchingColumnNameValueCommand : CommandBase
    {
        private readonly SettingsExcelViewModel settingsExcelViewModel;
        private readonly ExcelSettings settings;
        public AddSearchingColumnNameValueCommand(SettingsExcelViewModel settingsExcelViewModel, ExcelSettings settings)
        {
            this.settingsExcelViewModel = settingsExcelViewModel;
            this.settings = settings;
        }
        public override void Execute(object parameter)
        {           
            settingsExcelViewModel.SelectedSearchingColumnNameValues.Add(new StringContainer("", settingsExcelViewModel.SelectedSearchingColumnName.Values));
            settings.ExcelSettingsRead.SearchingColumnNames.Find(item => item == settingsExcelViewModel.SelectedSearchingColumnName).Values.Add("");
        }
    }
}