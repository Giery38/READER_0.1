using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Wordprocessing;
using READER_0._1.Model.Exel.Settings;
using READER_0._1.Model.Settings;
using READER_0._1.ViewModel;
using READER_0._1.ViewModel.Settings;
using READER_0._1.ViewModel.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static READER_0._1.Model.Exel.Settings.ExelSettingsSearchFiles;

namespace READER_0._1.Command.Settings.SettingsExel
{
    class AddConfigurationsNameCommand : CommandBase
    {
        private readonly SettingsExelViewModel settingsExelViewModel;
        private readonly ExelSettings settings;
        public AddConfigurationsNameCommand(SettingsExelViewModel settingsExelViewModel, ExelSettings settings)
        {
            this.settingsExelViewModel = settingsExelViewModel;
            this.settings = settings;
        }
        public override void Execute(object parameter)
        {
            ConfigurationName addedConfigurationName = new ConfigurationName();
            for (int i = 0; i < settingsExelViewModel.InputConfigurationsNameFragmented.Count; i++)
            {
                if (settingsExelViewModel.InputConfigurationsNameFragmented[i].Item2 is bool && (bool)settingsExelViewModel.InputConfigurationsNameFragmented[i].Item2 == true)
                {
                    string temp = settingsExelViewModel.InputConfigurationsNameFragmented[i].Item1 as string;
                    addedConfigurationName.Modifieds.Add(new(i, temp.ToCharArray()[0]));
                }
            }
            List<char> nameFragment = new List<char>();
            for (int i = 0; i < addedConfigurationName.Modifieds.Count; i++)
            {
                while (addedConfigurationName.Modifieds[i].position > nameFragment.Count)
                {
                    nameFragment.Add('X');
                }
                nameFragment.Add(addedConfigurationName.Modifieds[i].symbol);
            }
            while (nameFragment.Count < 8)
            {
                nameFragment.Add('X');
            }
            addedConfigurationName.Name = new string(nameFragment.ToArray());
            settingsExelViewModel.ConfigurationNames.Add(addedConfigurationName);
            settings.ExelSettingsSearchFiles.Configurations.Add(addedConfigurationName);
            //settings.SaveSettings();
        }
    }
}
