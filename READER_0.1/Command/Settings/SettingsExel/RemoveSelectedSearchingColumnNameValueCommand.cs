﻿using READER_0._1.Model.Excel.Settings;
using READER_0._1.ViewModel.Settings;
using READER_0._1.ViewModel.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Command.Settings.SettingsExcel
{
    class RemoveSelectedSearchingColumnNameValueCommand : CommandBase
    {
        private readonly SettingsExcelViewModel settingsExcelViewModel;
        private readonly ExcelSettings settings;
        public RemoveSelectedSearchingColumnNameValueCommand(SettingsExcelViewModel settingsExcelViewModel, ExcelSettings settings)
        {
            this.settingsExcelViewModel = settingsExcelViewModel;
            this.settings = settings;
        }

        public override void Execute(object parameter)
        {
            string value = parameter as string;
            StringContainer stringContainer = settingsExcelViewModel.SelectedSearchingColumnNameValues.FirstOrDefault(item => item.Value == value);
            settingsExcelViewModel.SelectedSearchingColumnNameValues.Remove(stringContainer);
            settingsExcelViewModel.SelectedSearchingColumnName.Values.Remove(value);
            settings.ExcelSettingsRead.SearchingColumnNames.Find(item => item == settingsExcelViewModel.SelectedSearchingColumnName).Values.Remove(value);
        }
    }
}
