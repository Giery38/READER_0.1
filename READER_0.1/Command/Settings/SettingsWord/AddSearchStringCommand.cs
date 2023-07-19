using READER_0._1.Model.Settings.Word;
using READER_0._1.Model.Word.Settings;
using READER_0._1.ViewModel.Settings;
using READER_0._1.ViewModel.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Command.Settings.SettingsWord
{
    public class AddSearchStringCommand : CommandBase
    {
        private readonly SettingsWordViewModel settingsWordViewModel;
        private readonly WordSettings wordSettings;
        public AddSearchStringCommand(SettingsWordViewModel settingsWordViewModel, WordSettings wordSettings)
        {
            this.settingsWordViewModel = settingsWordViewModel;
            this.wordSettings = wordSettings;
        }
        public override void Execute(object parameter)
        {
            SearchString baseSearchString = new SearchString(wordSettings.WordSettingsRead.BaseSearchString);
            SearchString searchString = new SearchString();

            string temp = baseSearchString.KeyWords.First();
            string keyWordName = baseSearchString.SearchWords.Find(item => item.Data == temp).Name;

            List<SearchWord> baseWords = new List<SearchWord>(baseSearchString.SearchWords);
            List<int> associationsWords = new List<int>();
            for (int i = 0; i < baseWords.Count; i++)
            {
                if (baseSearchString.KeyWords.Find(item => item == baseWords[i].Data) != null)
                {
                    baseWords.Remove(baseWords[i]);
                }
            }
            int countSearchWordsWithoutKeys = baseWords.Count;
            int counter = 0;
            foreach (BindingTuple word in settingsWordViewModel.SearchStringWords)
            {
                string value = word.Item1 as string;
                searchString.SearchStringWords.Add(value);
                bool selected = (bool)word.Item2;
                if (selected == true)
                {
                    searchString.KeyWords.Add(value);
                    searchString.SearchWords.Add(new SearchWord(keyWordName, value));
                    associationsWords.Add(settingsWordViewModel.SearchStringWords.IndexOf(word));
                }
                else
                {
                    if (counter < countSearchWordsWithoutKeys)
                    {
                        searchString.SearchWords.Add(new SearchWord(baseWords[counter].Name, value));
                        counter++;
                    }                    
                }                
            }
            int key = 0;
            if (searchString.AssociationsWords.Count > 0)
            {
                key = searchString.AssociationsWords.Last().Key + 1;
            }
            searchString.AssociationsWords.Add(key, associationsWords);
            searchString.SetPositionKeyWordsMask();
            searchString.SetPositionSearchWordsMask();            
        }
    }
}
