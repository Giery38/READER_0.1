using READER_0._1.Command.Settings.SettingsWord;
using READER_0._1.Model.Exel.Settings;
using READER_0._1.Model.Settings.Word;
using READER_0._1.Model.Word.Settings;
using READER_0._1.ViewModel.tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static READER_0._1.Model.Settings.Word.SearchParagraph;

namespace READER_0._1.ViewModel.Settings
{
    public class SettingsWordViewModel : ViewModelBase
    {
        private readonly WordSettings settings;
        //command
        public ICommand AddTypeSearchStringsCommand { get; }
        public ICommand AddSearchStringCommand { get; }

        public SettingsWordViewModel(WordSettings settings)
        {
            this.settings = settings;
            SearchParagraphs = new ObservableCollection<SearchParagraph>(settings.WordSettingsRead.SearchParagraphs);
            //command
            AddTypeSearchStringsCommand = new AddTypeSearchStringsCommand(this, settings);
            AddSearchStringCommand = new AddSearchStringCommand(this, settings);
        }
        private ObservableCollection<SearchParagraph> searchParagraphs;
        public ObservableCollection<SearchParagraph> SearchParagraphs
        {
            get
            {
                return searchParagraphs;
            }
            set
            {
                searchParagraphs = value;
                OnPropertyChanged(nameof(searchParagraphs));
            }
        }
        private SearchParagraph selectedSearchParagraph;
        public SearchParagraph SelectedSearchParagraph
        {
            get
            {
                return selectedSearchParagraph;
            }
            set
            {
                selectedSearchParagraph = value;
                OnPropertyChanged(nameof(SelectedSearchParagraph));
                TypesSearchStrings = new ObservableCollection<TypeSearchStrings>(SelectedSearchParagraph.TypesSearchStrings);
            }
        }
        private ObservableCollection<TypeSearchStrings> typesSearchStrings;
        public ObservableCollection<TypeSearchStrings> TypesSearchStrings
        {
            get
            {
                return typesSearchStrings;
            }
            set
            {
                typesSearchStrings = value;
                OnPropertyChanged(nameof(TypesSearchStrings));               
            }
        }
        private TypeSearchStrings selectedTypeSearchStrings;
        public TypeSearchStrings SelectedTypeSearchStrings
        {
            get
            {
                return selectedTypeSearchStrings;
            }
            set
            {
                selectedTypeSearchStrings = value;
                OnPropertyChanged(nameof(SelectedTypeSearchStrings));
                if (SearchStrings != null)
                {
                    SearchStrings.Clear();
                }
                if (SelectedTypeSearchStrings != null)
                {
                    SearchStrings = new ObservableCollection<SearchString>(SelectedTypeSearchStrings.SearchStrings);
                }                
            }
        }

        private ObservableCollection<SearchString> searchStrings;
        public ObservableCollection<SearchString> SearchStrings
        {
            get
            {
                return searchStrings;
            }
            set
            {
                searchStrings = value;
                OnPropertyChanged(nameof(SearchStrings));
            }
        }

        private string inputSearchStringsText;
        public string InputSearchStringsText
        {
            get
            {
                return inputSearchStringsText;
            }
            set
            {              
                inputSearchStringsText = value;
                OnPropertyChanged(nameof(InputSearchStringsText));
                SearchStringWords = new ObservableCollection<BindingTuple>(
                    inputSearchStringsText.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(item => new BindingTuple(item, false)));
            }
        }
        private ObservableCollection<BindingTuple> searchStringWords;
        public ObservableCollection<BindingTuple> SearchStringWords
        {
            get
            {
                return searchStringWords;
            }
            set
            {
                searchStringWords = value;
                OnPropertyChanged(nameof(SearchStringWords));
            }
        }
    }
}
