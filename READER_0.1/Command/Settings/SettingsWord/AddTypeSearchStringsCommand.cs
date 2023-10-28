using DocumentFormat.OpenXml.Office.CustomUI;
using READER_0._1.Model.Settings.Word;
using READER_0._1.Model.Word.Settings;
using READER_0._1.ViewModel.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static READER_0._1.Model.Settings.Word.SearchParagraph;

namespace READER_0._1.Command.Settings.SettingsWord
{
    public class AddTypeSearchStringsCommand : CommandBase
    {
        private readonly SettingsWordViewModel settingsWordViewModel;
        private readonly WordSettings wordSettings;
        public AddTypeSearchStringsCommand(SettingsWordViewModel settingsWordViewModel, WordSettings wordSettings)
        {
            this.settingsWordViewModel = settingsWordViewModel;
            this.wordSettings = wordSettings; 
        }
        public override void Execute(object parameter)
        {
            /*
            settingsWordViewModel.TypesSearchStrings.Add(new TypeSearchStrings());
            SearchParagraph searchParagraph = settingsWordViewModel.SelectedSearchParagraph;
            wordSettings.WordSettingsRead.SearchParagraphs.Find(item => item == searchParagraph).SubTypesSearchStrings.Add(new TypeSearchStrings());
            */
        }
    }
}
