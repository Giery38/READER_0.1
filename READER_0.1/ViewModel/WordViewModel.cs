using READER_0._1.Command.CommandWord;
using READER_0._1.Model;
using READER_0._1.Model.Exel.Settings;
using READER_0._1.Model.Exel;
using READER_0._1.Model.Word;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using READER_0._1.Model.Settings.Word;

namespace READER_0._1.ViewModel
{
    public class WordViewModel : ViewModelBase
    {
        private readonly WindowFileBase windowFileBase;
        public ObservableCollection<WordFile> WordFiles { get; set; } 
        public ICommand AddWordFileCommand { get; }
        public ICommand ReadWordFileCommand { get; }
        public ICommand AddSearchStringInSettingsCommand { get; }
        public WordViewModel(WindowFileBase windowFileBase)
        {
            WordFiles = new ObservableCollection<WordFile>();            
            this.windowFileBase = windowFileBase;
            //
            AddWordFileCommand = new AddWordFileCommand(this, windowFileBase);
            ReadWordFileCommand = new ReadWordFileCommand(this, windowFileBase);
            AddSearchStringInSettingsCommand = new AddSearchStringInSettingsCommand(this, windowFileBase);
            //
            SelectedSettings = windowFileBase.settings.WordSettingsRead;
            SearchParagraphRates = SelectedSettings.SearchParagraphs.Find(item => item.Name == "Тарифы");
            SearchParagraphStanding = SelectedSettings.SearchParagraphs.Find(item => item.Name == "Простои");
        }
        private WordFile selectedWordFile;
        public WordFile SelectedWordFile
        {
            get
            {               
                return selectedWordFile;
            }
            set
            {
                if (value != null)
                {
                    selectedWordFile = null;
                    OnPropertyChanged(nameof(SelectedWordFile));
                    selectedWordFile = value;
                    OnPropertyChanged(nameof(SelectedWordFile));
                }                    
            }            
        }
        private WordSettingsRead selectedSettings;
        public WordSettingsRead SelectedSettings
        {
            get
            {
                return selectedSettings;
            }
            set
            {
                selectedSettings = value;
                OnPropertyChanged(nameof(SelectedSettings));
            }
        }

        private SearchParagraph searchParagraphRates;
        public SearchParagraph SearchParagraphRates
        {
            get
            {
                return searchParagraphRates;
            }
            set
            {
                searchParagraphRates = value;
                OnPropertyChanged(nameof(SearchParagraphRates));
            }
        }
        private SearchParagraph searchParagraphStanding;
        public SearchParagraph SearchParagraphStanding
        {
            get
            {
                return searchParagraphStanding;
            }
            set
            {
                searchParagraphStanding = value;
                OnPropertyChanged(nameof(SearchParagraphStanding));
            }
        }
        private SearchParagraph searchParagraphOther;
        public SearchParagraph SearchParagraphOther
        {
            get
            {
                return searchParagraphOther;
            }
            set
            {
                searchParagraphOther = value;
                OnPropertyChanged(nameof(SearchParagraphOther));
            }
        }      
       
        public void UpdateFiles()
        {
            WordFiles.Clear();
            WordFiles = new ObservableCollection<WordFile>(windowFileBase.wordWindowFileBase.WordFiles);
            OnPropertyChanged(nameof(WordFiles));
            SelectedWordFile = selectedWordFile;
        }
        public void ReadWordFile(WordFile AddedFile, WordSettingsRead wordSettingsRead)
        {
            bool result = false;
            Thread readWordFile = new Thread(() =>
            {
                result = windowFileBase.wordWindowFileBase.TryReadWordFile(AddedFile, wordSettingsRead);
            });
            readWordFile.Start();
            if (result == false)
            {

            }
        }        
    }
}
