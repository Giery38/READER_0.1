using READER_0._1.Command.CommandWord;
using READER_0._1.Model;
using READER_0._1.Model.Word;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace READER_0._1.ViewModel
{
    public class WordViewModel : ViewModelBase
    {
        private readonly WindowFileBase windowFileBase;
        public ObservableCollection<WordFile> WordFiles { get; set; } 
        public ICommand AddWordFileCommand { get; }
        public WordViewModel(WindowFileBase windowFileBase)
        {
            WordFiles = new ObservableCollection<WordFile>();            
            this.windowFileBase = windowFileBase;
            AddWordFileCommand = new AddWordFileCommand(this, windowFileBase);
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
        public void UpdateFiles()
        {
            WordFiles.Clear();
            WordFiles = new ObservableCollection<WordFile>(windowFileBase.wordWindowFileBase.WordFiles);
            OnPropertyChanged(nameof(WordFiles));
            SelectedWordFile = selectedWordFile;
        }

        public override void Deactivation()
        {
            throw new NotImplementedException();
        }
    }
}
