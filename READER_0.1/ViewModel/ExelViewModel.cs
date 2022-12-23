using READER_0._1.Command;
using READER_0._1.Model;
using READER_0._1.Model.Exel;
using READER_0._1.Model.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace READER_0._1.ViewModel 
{
    class ExelViewModel : ViewModelBase
    {
        private readonly  WindowFileBase windowFileBase;
        public ObservableCollection<ExelFile> ExelFiles { get; private set; }
        public ObservableCollection<Directory> Directories { get; private set; }
        public ObservableCollection<string> ExelFilesСontentInDirectoriesEquals { get; private set; }
        public ObservableCollection<string> ExelFilesСontentInDirectoriesNoEquals { get; private set; }
        public ObservableCollection<ConverterListInListForView> ColumnsNamesInPage { get; private set; }
        private ConverterListInListForView converterListInListForView;
        private ExelSettingsRead exelSettingsRead;
        public ICommand AddFileCommand { get; }
        public ICommand AddFileDropCommand { get; }
        public ICommand AddDirectoryCommand { get; }       
        public ICommand CopyFileCommand { get; }

        private Thread checkFilesRead;

        public ExelViewModel(WindowFileBase windowFileBase)
        {
            ExelFiles = new ObservableCollection<ExelFile>();
            Directories = new ObservableCollection<Directory>();
            this.windowFileBase = windowFileBase;
            AddFileCommand = new AddFileExelCommand(this, windowFileBase);
            AddDirectoryCommand = new AddDirectoryCommand(this, windowFileBase);
            CopyFileCommand = new CopyFileCommand(this, windowFileBase);
            AddFileDropCommand = new AddFileDropCommand(this, windowFileBase);
            ExelFilesСontentInDirectoriesEquals = new ObservableCollection<string>();
            ExelFilesСontentInDirectoriesNoEquals = new ObservableCollection<string>();
            converterListInListForView = new ConverterListInListForView();
            ColumnsNamesInPage = new ObservableCollection<ConverterListInListForView>();
            exelSettingsRead = new ExelSettingsRead();
            checkFilesRead = new Thread(() => CheckFilesRead());
        }        

        private ExelFile selectedExelFiles;
        public ExelFile SelectedExelFiles
        {
            get
            {
                return selectedExelFiles;
            }
            set
            {
                if (value != null)
                {
                    selectedExelFiles = value;
                    OnPropertyChanged(nameof(SelectedExelFiles));
                    SelectFerstPageInSelectedFile();
                }
            }
        }        
        private void SelectFerstPageInSelectedFile()
        {
            if (SelectedExelFiles != null)
            {
                if (selectedExelFiles.ExelPage.Count > 0)
                {
                    SelectedPage = SelectedExelFiles.ExelPage[0];
                    UpdateDirectory();
                }
            }
        }
       
        private ExelFilePage selectedPage;
        public ExelFilePage SelectedPage
        {
            get
            {
                return selectedPage;
            }
            set
            {
                selectedPage = value;
                OnPropertyChanged(nameof(SelectedPage));
                UpdateColumnsNamesInPage();
                SelectMainColumnNameInPage("Номера отправки");
            }
        }
        private ConverterListInListForView selectedColumnName;
        public ConverterListInListForView SelectedColumnName
        {
            get
            {
                return selectedColumnName;
            }
            set
            {
                if (value != null)
                {
                    selectedColumnName = value;
                    OnPropertyChanged(nameof(SelectedColumnName));
                    ChangePageInfo(SelectedColumnName.stringValue);
                }                         
            }
        }       
        private void SelectMainColumnNameInPage(string MainColumnName) 
        {
            List<ConverterListInListForView> tempList = ColumnsNamesInPage.ToList();
            int searchIndex = 0;
            if (tempList.Count == 0)
            {
                return ;
            }
            if (SelectedColumnName == null)
            {
                searchIndex = tempList.FindIndex(item => item.stringValue == MainColumnName);
                SelectedColumnName = ColumnsNamesInPage[searchIndex];               
            }
            else
            {
                searchIndex = tempList.FindIndex(item => item.stringValue == SelectedColumnName.stringValue);
                if (searchIndex < 0)
                {
                    searchIndex = tempList.FindIndex(item => item.stringValue == MainColumnName);
                }
                SelectedColumnName = ColumnsNamesInPage[searchIndex];
            }

            ChangePageInfo(SelectedColumnName.stringValue);
        }
        private void UpdateColumnsNamesInPage()
        {
            if (SelectedPage != null)
            {
                ColumnsNamesInPage.Clear();
                var columnsNamesInPage = converterListInListForView.ConvertList(SelectedPage.GetColumnsNameAllTabels());
                foreach (ConverterListInListForView name in columnsNamesInPage)
                {
                    ColumnsNamesInPage.Add(name);
                }                
            }
        }

        private void ChangePageInfo(string ColumnName)
        {
            if (SelectedPage != null &&
                    SelectedPage.GetColumnsData(ColumnName) != null)
            {
                List<object> ColumnsData = new List<object>(SelectedPage.GetColumnsData(ColumnName));
                ColumnsData.RemoveAll(item => item == null);
                SelectedPageInfo["ColumnsData"] = ColumnsData.Count;
                List<object> ColumnsDataDplicates = new List<object>(SelectedPage.GetColumnsDataDplicates(ColumnName));
                ColumnsDataDplicates.RemoveAll(item => item == null);
                SelectedPageInfo["ColumnsDataDplicates"] = ColumnsDataDplicates.Count;
                List<object> ColumnsDataNoDplicates = new List<object>(SelectedPage.GetColumnsDataNoDplicates(ColumnName));
                ColumnsDataNoDplicates.RemoveAll(item => item == null);
                SelectedPageInfo["ColumnsDataNoDplicates"] = ColumnsDataNoDplicates.Count;                
            }
            else
            {
                SelectedPageInfo["ColumnsData"] = 0;
                SelectedPageInfo["ColumnsDataDplicates"] = 0;
                SelectedPageInfo["ColumnsDataNoDplicates"] = 0;
            }
            OnPropertyChanged(nameof(SelectedPageInfo));
        }

        private Dictionary<string, int> selectedPageInfo = new Dictionary<string, int>
        {
            { "ColumnsData", 0 },
            { "ColumnsDataDplicates", 0 },
            { "ColumnsDataNoDplicates", 0 }
        };
        public Dictionary<string, int> SelectedPageInfo
        {
            get
            {
                return selectedPageInfo;
            }
            set
            {
                selectedPageInfo = value;
                OnPropertyChanged(nameof(SelectedPageInfo));
            }
        }
        public void UpdateFiles(object sender, EventArgs eventArgs)
        {            
            UpdateFiles();
        }

        public async void UpdateFiles()
        {
            if (windowFileBase.exelWindowFileBase.ExelFiles.Count == 0)
            {
                return;
            }
            //List<ExelFile> addedFiles = windowFileBase.exelWindowFileBase.ExelFiles.Except(ExelFiles.ToList()).ToList();
            if (checkFilesRead.IsAlive == true)
            {
                System.Windows.Controls.ListView listView = new System.Windows.Controls.ListView();
                checkFilesRead.Join(300); //тут может появиться очень мерзкий баг                
            }                    
            foreach (ExelFile exelFile in addedFiles)
            {
                ExelFiles.Add(exelFile);
            }
            if (checkFilesRead.IsAlive == false)
            {
                checkFilesRead = new Thread(() => CheckFilesRead());
                checkFilesRead.Start();
            }            
        }
        public void UpdateFile(ExelFile exelFile)
        {
            List<ExelFile> ExelFilesConvert = ExelFiles.ToList();
            int fileFindedIndex = ExelFilesConvert.FindIndex(item => item.FileName == exelFile.FileName);            
            ExelFiles.RemoveAt(fileFindedIndex);
            ExelFiles.Insert(fileFindedIndex, exelFile);            
        }

        private void CheckFilesRead()
        {
            bool stop = false;
            List<ExelFile> exelFiles = new List<ExelFile>();
            for (int i = 0; i < ExelFiles.Count; i++)
            {
                if (ExelFiles[i].Readed == true)
                {
                    exelFiles.Add(ExelFiles[i]);
                }
            }
            int count = 0;
            while (stop == false)
            {
                count++;
                stop = true;
                for (int i = 0; i < windowFileBase.exelWindowFileBase.ExelFiles.Count; i++)
                {
                    stop = stop & windowFileBase.exelWindowFileBase.ExelFiles[i].Readed;
                }
                for (int i = 0; i < windowFileBase.exelWindowFileBase.ExelFiles.Count; i++)
                {
                    ExelFile find = exelFiles.Find(item => item.FileName == windowFileBase.exelWindowFileBase.ExelFiles[i].FileName);
                    if (windowFileBase.exelWindowFileBase.ExelFiles[i].Readed == true && find == null)
                    {
                        exelFiles.Add(windowFileBase.exelWindowFileBase.ExelFiles[i]);
                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            UpdateFile(windowFileBase.exelWindowFileBase.ExelFiles[i]); // проверку на нуль сделать                            
                        });                        
                    }                                        
                }           
            }
        }

        public void UpdateDirectory(object sender, EventArgs e)
        {
            UpdateDirectory();
        }
        public void UpdateDirectory()
        {            
            if (windowFileBase.exelWindowFileBase.ExelFilesСontentInDirectories.Count == 0)
            {
                return ;
            }
            Directories.Clear();
            List<Directory> directoryies = new List<Directory>();
            bool availabilityDirectories = windowFileBase.exelWindowFileBase.ExelFilesСontentInDirectories.TryGetValue(SelectedExelFiles, out directoryies);
            if (availabilityDirectories == true)
            {
                foreach (Directory directory in directoryies)
                {
                    Directories.Add(directory);
                }
            }
            ExelFilesСontentInDirectoriesEquals.Clear();
            List<Model.File> exelFilesСontentInDirectoriesEquals = new List<Model.File>();            
            bool availabilityDirectoriesСontentEquals = windowFileBase.exelWindowFileBase.ExelFilesСontentInDirectoriesEquals.TryGetValue(SelectedPage, out exelFilesСontentInDirectoriesEquals);
            if (availabilityDirectoriesСontentEquals == true)
            {
                foreach (Model.File file in exelFilesСontentInDirectoriesEquals)
                {
                    ExelFilesСontentInDirectoriesEquals.Add(file.FileName);
                }
            }
            ExelFilesСontentInDirectoriesNoEquals.Clear();
            List<Model.File> exelFilesСontentInDirectoriesNoEquals = new List<Model.File>();
            bool availabilityDirectoriesСontentNoEquals = windowFileBase.exelWindowFileBase.ExelFilesСontentInDirectoriesNoEquals.TryGetValue(SelectedPage, out exelFilesСontentInDirectoriesNoEquals);
            if (availabilityDirectoriesСontentNoEquals == true)
            {
                foreach (Model.File file in exelFilesСontentInDirectoriesNoEquals)
                {
                    ExelFilesСontentInDirectoriesNoEquals.Add(file.FileName);
                }
            }
        }        
    }
}
