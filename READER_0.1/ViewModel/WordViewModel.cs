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
using READER_0._1.Model.Word.Settings;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using Microsoft.Win32;

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

            SearchParagraphs = new ObservableCollection<SearchParagraph>(SelectedSettings.WordSettingsRead.SearchParagraphs);
          
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
                OnPropertyChanged(nameof(SearchParagraphs));
            }
        }       
       public void CreateExcelFile(WordFile wordFile)
       {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = new string(wordFile.Name);
            saveFileDialog.Filter = "Excel файлы(*.xlsx)|*.xlsx|Все файлы(*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(saveFileDialog.FileName, SpreadsheetDocumentType.Workbook))
                {
                    // Добавляем рабочую книгу
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    // Добавляем лист в рабочую книгу
                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    // Добавляем ссылку на лист в рабочую книгу
                    Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Лист1" };
                    sheets.Append(sheet);
                }

                if (wordFile.Readed == true && wordFile.Corrupted == false) // убатьзависимость от выделения
                {
                    ExelFile exelFile = new ExelFile(saveFileDialog.FileName);
                    exelFile.SetTempCopyPath(exelFile.Path);
                    ExelFileWriter exelFileWriter = new ExelFileWriter(exelFile);
                    exelFileWriter.Open();
                    exelFileWriter.TablesWrite(wordFile.Tables[0], "A1");
                    exelFileWriter.Close();
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
