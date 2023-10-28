using READER_0._1.Command.CommandWord;
using READER_0._1.Model;
using READER_0._1.Model.Excel.Settings;
using READER_0._1.Model.Excel;
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
using READER_0._1.Model.Word.Settings;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using Microsoft.Win32;
using Microsoft.Office.Interop.Word;
using DocumentFormat.OpenXml.Drawing;
using READER_0._1.Tools;
using System.Windows.Media;

namespace READER_0._1.ViewModel
{
    public class WordViewModel : ViewModelBase
    {
        private readonly WindowFileBase windowFileBase;
        public ObservableCollection<WordFile> Files { get; set; } 
        public ICommand AddWordFileCommand { get; }
        public ICommand ConvertWordFileToExcelCommand { get; }
        public ICommand AddSearchStringInSettingsCommand { get; }
        public WordViewModel(WindowFileBase windowFileBase)
        {
            Files = new ObservableCollection<WordFile>();            
            this.windowFileBase = windowFileBase;
            //
            AddWordFileCommand = new AddWordFileCommand(this, windowFileBase);
            ConvertWordFileToExcelCommand = new ConvertWordFileToExcelCommand(this, windowFileBase);
            AddSearchStringInSettingsCommand = new AddSearchStringInSettingsCommand(this, windowFileBase);
            //
            SelectedSettings = windowFileBase.settings.WordSettings;
            SearchParagraphs = (SearchParagraph[])SelectedSettings.WordSettingsRead.SearchParagraphs.ToArray().Clone();
            if (SearchParagraphs.Length > 2)
            {               
                SearchParagraphOne = SearchParagraphs[0];
                SearchParagraphTwo = SearchParagraphs[1];
                SearchParagraphProvider = SearchParagraphs[2];
            }            
        }
        public SearchParagraph[] SearchParagraphs { get; private set; }
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
        private WordSettings selectedSettings;
        public WordSettings SelectedSettings
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

        private SearchParagraph searchParagraphOne;
        public SearchParagraph SearchParagraphOne
        {
            get
            {
                return searchParagraphOne;
            }
            set
            {
                searchParagraphOne = value;
                OnPropertyChanged(nameof(SearchParagraphOne));
            }
        }
        private SearchParagraph searchParagraphTwo;
        public SearchParagraph SearchParagraphTwo
        {
            get
            {
                return searchParagraphTwo;
            }
            set
            {
                searchParagraphTwo = value;
                OnPropertyChanged(nameof(SearchParagraphTwo));
            }
        }
        private SearchParagraph searchParagraphProvider;
        public SearchParagraph SearchParagraphProvider
        {
            get
            {
                return searchParagraphProvider;
            }
            set
            {
                searchParagraphProvider = value;
                OnPropertyChanged(nameof(SearchParagraphProvider));
            }
        }          
       public void CreateExcelFile(WordFile wordFile)
       {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = new string(wordFile.Name);
            saveFileDialog.Filter = "Excel файлы(*.xlsx)|*.xlsx|Все файлы(*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                ExcelFile excelFile = new ExcelFile(saveFileDialog.FileName, true);
                excelFile.SetTempCopyPath(excelFile.Path);
                ExcelFileWriter excelFileWriter = new ExcelFileWriter(excelFile);               
                if (wordFile.Readed == true && wordFile.Corrupted == false) // убатьзависимость от выделения
                {                  
                    excelFileWriter.Open();
                    excelFileWriter.TablesWrite(wordFile.Tables[0],0, "B2");
                    excelFileWriter.Close();
                }
            }
       }
        public void UpdateFiles()
        {
            Files.Clear();
            Files = new ObservableCollection<WordFile>(windowFileBase.wordWindowFileBase.WordFiles);
            OnPropertyChanged(nameof(Files));
            SelectedWordFile = selectedWordFile;
        }
        public void ReadWordFile(WordFile readFile, WordSettingsRead wordSettingsRead)
        {
            if (windowFileBase.wordWindowFileBase.WordReaderManager.WordFileReaders.Find(item => item.WordFile == readFile) != null)
            {
               
            }
            windowFileBase.wordWindowFileBase.ReadWordFile(readFile, wordSettingsRead);                      
        }        
    }
}
