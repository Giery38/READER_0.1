using READER_0._1.Command.CommandExel;
using READER_0._1.Model;
using READER_0._1.Model.Exel;
using READER_0._1.Model.Settings.Exel;
using READER_0._1.Tools;
using READER_0._1.ViewModel.ViewElement;
using READER_0._1.ViewModel.ViewElement.TableView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace READER_0._1.ViewModel 
{
    class ExelViewModel : ViewModelBase
    {
        private readonly WindowFileBase windowFileBase;
        public ObservableCollection<ExelFile> ExelFiles { get; private set; }
        public ObservableCollection<ExelFilePage> ExelFilePages { get; private set; }
        public ObservableCollection<Directory> Directories { get; private set; }
        public ObservableCollection<FolderView> FoldersView { get; private set; }
        public ObservableCollection<string> ExelFilesСontentInDirectoriesEquals { get; private set; }
        public ObservableCollection<string> ExelFilesСontentInDirectoriesNoEquals { get; private set; }
        public ObservableCollection<ConverterListInListForView> ColumnsNamesInPage { get; private set; }
        public DataView DataView { get; private set; }
        public ObservableCollection<string> TableInfo { get; private set; } 
        //
        public ICommand AddFileCommand { get; }
        public ICommand AddExelFileDropCommand { get; }
        public ICommand AddDirectoryCommand { get; }       
        public ICommand CopyFileCommand { get; }
        public ICommand CreateViewFolderCommand { get; }
        public ICommand SizeChangeCommand { get; }
        //
        private ConverterListInListForView converterListInListForView;
        //
        private ExelSettingsRead exelSettingsRead;
        //
        private List<Thread> loadedTabels;       
        public ExelViewModel(WindowFileBase windowFileBase)
        {
            this.windowFileBase = windowFileBase;
            //obs
            ExelFiles = new ObservableCollection<ExelFile>();
            ExelFilePages = new ObservableCollection<ExelFilePage>();
            Directories = new ObservableCollection<Directory>();
            ExelFilesСontentInDirectoriesEquals = new ObservableCollection<string>();
            ExelFilesСontentInDirectoriesNoEquals = new ObservableCollection<string>();
            ColumnsNamesInPage = new ObservableCollection<ConverterListInListForView>();
            FoldersView = new ObservableCollection<FolderView>();
            DataView = new DataView();
            TableInfo = new ObservableCollection<string>(new List<string>
            {
                "Все",
                "Найденные",
                "Отсутствующие"
            });
            //
            //command
            AddFileCommand = new AddExelFileCommand(this, windowFileBase);
            AddDirectoryCommand = new AddDirectoryCommand(this, windowFileBase);
            CopyFileCommand = new CopyExelFileCommand(this, windowFileBase);
            AddExelFileDropCommand = new AddExelFileDropCommand(this, windowFileBase);
            CreateViewFolderCommand = new CreateFolderCommand(this, windowFileBase);
            SizeChangeCommand = new SizeChangeCommand(this, windowFileBase);
            //
            converterListInListForView = new ConverterListInListForView();            
            //
            exelSettingsRead = windowFileBase.settings.ExelSettingsRead;
            loadedTabels = new List<Thread>();
            //
            FoldersView.CollectionChanged += PropertyChangedFolderSubscription;
            //start
            UpdateFiles();          
        }

        private string selectedTableInfo;
        public string SelectedTableInfo
        {
            get
            {
                return selectedTableInfo;
            }
            set
            {
                selectedTableInfo = value;
                OnPropertyChanged(nameof(SelectedTableInfo));
                Thread changePageTables = new Thread(() => ChangePageTables(GetTabelsForTableInfo()));
                loadedTabels.Add(changePageTables);
                foreach (Thread item in ThreadHelper.SerchThreadLive(loadedTabels))
                {
                    item.Join();
                }
                changePageTables.Start();
            }
        }
        private double loadingTable = 0;
        public double LoadingTable
        {
            get
            {
                return loadingTable;
            }
            set
            {
                loadingTable = value;
                OnPropertyChanged(nameof(LoadingTable));                
            }
        }
        private ExelFile selectedExelFile;
        public ExelFile SelectedExelFile
        {
            get
            {
                return selectedExelFile;
            }
            set
            {                
                if (value != null)
                {
                    selectedExelFile = value;
                    OnPropertyChanged(nameof(SelectedExelFile));
                    SetExelFilePages(SelectedExelFile.ExelPage);
                    SelectFerstPageInSelectedFile();
                    CreateUnitedPage();                    
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
                if (SelectedPage != value && value != null)
                {
                    selectedPage = value;
                    OnPropertyChanged(nameof(SelectedPage));
                    UpdateColumnsNamesInPage();
                    SelectMainColumnNameInPage("Номера отправки");                                     
                    SelectedTableInfo = TableInfo[0];
                }
                
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
                    ChangePageInfo(SelectedColumnName.StringValue);
                    UpdateDirectory();
                }                         
            }
        }
        private List<ExelFilePageTable> GetTabelsForTableInfo()
        {
            List<ExelFilePageTable> tables = new List<ExelFilePageTable>();
            switch (selectedTableInfo)
            {
                case "Все":
                    tables = selectedPage.Tabeles;
                    break;
                case "Найденные":
                    tables = CreateTableEquals(new List<object>(ExelFilesСontentInDirectoriesEquals.ToList()));
                    break;
                case "Отсутствующие":
                    tables = CreateTableEquals(new List<object>(ExelFilesСontentInDirectoriesNoEquals.ToList()));
                    break;
                default:
                    break;
            }
            return tables;
        }

        public List<ExelFilePageTable> CreateTableEquals(List<object> list) // добавлять не строки а клетки в словарь сделать там метод
        {
            List<ExelFilePageTable> exelFilePageTables = new List<ExelFilePageTable>();
            ExelFilePageTable exelFilePageTable = new ExelFilePageTable(); 
            List<ExelFilePageTableRow> rows = new List<ExelFilePageTableRow>();
            for (int i = 0; i < selectedPage.Tabeles.Count; i++)
            {
                exelFilePageTable = new ExelFilePageTable(selectedPage.Tabeles[i].TableColumns.Keys.ToList());
                rows.AddRange(selectedPage.Tabeles[i].SearchRowsByColumn(SelectedColumnName.StringValue, list));
                exelFilePageTable.AddRow(rows);
                exelFilePageTables.Add(exelFilePageTable);
            }                      
            return exelFilePageTables;
        }

        private void SetExelFilePages(List<ExelFilePage> exelFilePages)
        {
            ExelFilePages = new ObservableCollection<ExelFilePage>(exelFilePages);
            OnPropertyChanged(nameof(ExelFilePages));
        }
        private void SelectFerstPageInSelectedFile()
        {
            if (SelectedExelFile != null)
            {
                if (ExelFilePages.Count > 0)
                {
                    SelectedPage = ExelFilePages[0];
                }
            }
        }
        public void AddFolderView(FolderView folderView)
        {
            FoldersView.Add(folderView);
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
                
                List<string> valueolumnNameValue = new List<string>();
                exelSettingsRead.SearchingColumnName.TryGetValue(MainColumnName, out valueolumnNameValue);
                foreach (string name in valueolumnNameValue)
                {
                    searchIndex = tempList.FindIndex(item => item.StringValue == name);
                    if (searchIndex >= 0)
                    {
                        break;
                    }
                }               
                if (searchIndex < 0)
                {
                    SelectedColumnName = ColumnsNamesInPage[0];
                }
                else
                {
                    SelectedColumnName = ColumnsNamesInPage[searchIndex];
                }                              
            }
            else
            {
                searchIndex = tempList.FindIndex(item => item.StringValue == SelectedColumnName.StringValue);
                if (searchIndex < 0)
                {
                    // searchIndex = tempList.FindIndex(item => item.StringValue == MainColumnName);
                    searchIndex = 0;
                }
                SelectedColumnName = ColumnsNamesInPage[searchIndex];
            }
            ChangePageInfo(SelectedColumnName.StringValue);
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
       
        private void ChangePageTables(List<ExelFilePageTable> tables)
        {
            if (SelectedPage != null )
            {
                List<DataTable> dataTables = new List<DataTable>();
                LoadingTable = 1;
                foreach (var item in tables)
                {
                    var tt = item.ToDataTabel();
                    dataTables.Add(tt);
                }
                App.Current.Dispatcher.Invoke(() => {
                    DataView = MergeDataTables(dataTables).DefaultView;
                    OnPropertyChanged(nameof(DataView));
                });
                LoadingTable = 0;                            
            }
        }
        private DataTable MergeDataTables(List<DataTable> dataTables)
        {
            DataTable result = new DataTable();
            foreach (DataTable table in dataTables)
            {
                result.Merge(table);
            }
            return result;
        }

        private void CreateUnitedPage()
        {
            if (SelectedExelFile != null && ExelFilePages.Count > 0)
            {
                ExelFilePage exelFilePage = new ExelFilePage("Все листы");
                foreach (ExelFilePage page in ExelFilePages)
                {
                    exelFilePage.AddTabel(page.Tabeles);
                }
                ExelFilePages.Add(exelFilePage);

            }
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

        public void UpdateFiles()
          {
             FolderView folderView;
             foreach (KeyValuePair<string,List<ExelFile>> folder in windowFileBase.exelWindowFileBase.FoldersWithFiles)
              {
                folderView = GetOrCreateFolderView(folder.Key);
                
                  if (folderView.Files.Equals(folder.Value) == false)
                  {
                      int index = FoldersView.IndexOf(folderView);
                      if (FoldersView[index].Files != null)
                      {
                          FoldersView[index].AddFiles(folder.Value.Except(FoldersView[index].Files).ToList());
                      }                    
                  }
              }
          }

        private FolderView GetOrCreateFolderView(string folderName)
        {
            FolderView folderView = FoldersView.FirstOrDefault(fv => fv.Name == folderName);

            if (folderView == null)
            {
                folderView = new FolderView(folderName, typeof(ExelFile));
                FoldersView.Add(folderView);
            }

            return folderView;
        }

        public void UpdateDirectory() // добавить удаление файлов
        {            
            if (windowFileBase.exelWindowFileBase.DirectoriesBelongExelFile.TryGetValue(SelectedExelFile, out List<Directory> bindingFiles) == false || bindingFiles.Count == 0)
            {
                Directories.Clear();
                OnPropertyChanged(nameof(Directories));
                ExelFilesСontentInDirectoriesEquals.Clear();
                ExelFilesСontentInDirectoriesNoEquals.Clear();
                return;
            }
            CheckingAddedDirectory();
            Directories.Clear();          
            if (windowFileBase.exelWindowFileBase.DirectoriesBelongExelFile.TryGetValue(SelectedExelFile, out List<Directory> directoryies) == true)
            {
                Directories = new ObservableCollection<Directory>(directoryies);
                OnPropertyChanged(nameof(Directories));
            }          
            ExelFilesСontentInDirectoriesEquals.Clear();
            SearchFilesResult searchFilesResult = windowFileBase.exelWindowFileBase.SearchFilesResults.FirstOrDefault(item => item.ExelFile == SelectedExelFile && item.NameColumn == SelectedColumnName.StringValue);
            ExelFilesСontentInDirectoriesEquals = new ObservableCollection<string>(searchFilesResult.GetAllFiles().Select(file => file.FileName).ToList());
            OnPropertyChanged(nameof(ExelFilesСontentInDirectoriesEquals));
            ExelFilesСontentInDirectoriesNoEquals.Clear();
            var exelFilesСontentInDirectoriesNoEquals = SelectedPage.GetColumnsData(SelectedColumnName.StringValue).ConvertAll(x => Convert.ToString(x)).OfType<string>().ToList().Except(ExelFilesСontentInDirectoriesEquals).ToList();
            exelFilesСontentInDirectoriesNoEquals.RemoveAll(string.IsNullOrEmpty);
            ExelFilesСontentInDirectoriesNoEquals = new ObservableCollection<string>(exelFilesСontentInDirectoriesNoEquals);
            OnPropertyChanged(nameof(ExelFilesСontentInDirectoriesNoEquals));
        }

        private void CheckingAddedDirectory()
        {
            if (windowFileBase.exelWindowFileBase.SearchFilesResults.FirstOrDefault(item => item.ExelFile == SelectedExelFile && item.NameColumn == SelectedColumnName.StringValue) == null)
            {
                SearchFilesResult searchFilesResult = new SearchFilesResult();
                searchFilesResult.SetExelFile(SelectedExelFile);
                searchFilesResult.SetNameColumn(selectedColumnName.StringValue);
                windowFileBase.exelWindowFileBase.DirectoriesBelongExelFile.TryGetValue(SelectedExelFile, out List<Directory> directories);
                if (directories != null && searchFilesResult.FilesInDirectory.Keys.ToList().Equals(directories) == false)
                {
                    List<Directory> addedFilesInDirectories = directories.Except(searchFilesResult.FilesInDirectory.Keys.ToList()).ToList();
                    foreach (Directory directory in addedFilesInDirectories)
                    {
                        searchFilesResult.AddFilesInDirectory(directory, SearchFilesInDirectoryies(directory, selectedColumnName.StringValue));
                    }
                    windowFileBase.exelWindowFileBase.AddSearchFilesResult(searchFilesResult);
                }
            }
        }

        private List<Model.File> SearchFilesInDirectoryies(Directory directory, string nameColumn)
        {
            List<Model.File> EqualsFile = directory.SearchFileToName(SelectedPage.GetColumnsData(nameColumn).ConvertAll(x => Convert.ToString(x)).OfType<string>().ToList(), Formats.pdf);
            return EqualsFile;
        }

        private void PropertyChangedFolderSubscription(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (FolderView folder in FoldersView)
            {
                folder.PropertyChanged += PropertyChangedNameFolder;
            }
        }
        private void PropertyChangedNameFolder(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                FolderView folderView = (FolderView)sender;
                string newName = folderView.Name;
                List<FolderView> convertList = FoldersView.ToList();
                int index = convertList.FindIndex(item => item.Name == newName);
                windowFileBase.exelWindowFileBase.ChangeFolerName(newName, index);
                UpdateFiles();
            }
        }
    }
}
