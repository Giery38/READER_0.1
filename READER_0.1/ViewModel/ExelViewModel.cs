using READER_0._1.Command.CommandExel;
using READER_0._1.Model;
using READER_0._1.Model.Exel;
using READER_0._1.Model.Settings.Exel;
using READER_0._1.Tools;
using READER_0._1.ViewModel.tools;
using READER_0._1.ViewModel.ViewElement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace READER_0._1.ViewModel
{
    public class ExelViewModel : ViewModelBase
    {
        private readonly WindowFileBase windowFileBase;
        public ObservableCollection<ExelFilePage> ExelFilePages { get; private set; }
        public ObservableCollection<Directory> Directories { get; private set; }
        public ObservableCollection<FolderView> FoldersView { get; private set; }
        public ObservableCollection<string> ExelFilesСontentInDirectoriesEquals { get; private set; }
        public ObservableCollection<string> ExelFilesСontentInDirectoriesNoEquals { get; private set; }
        public ObservableCollection<ConverterListInListForView> ColumnsNamesInPage { get; private set; }
        public ObservableCollection<string> TableInfo { get; private set; }
        //
        public ICommand AddFileCommand { get; }
        public ICommand AddExelFileDropCommand { get; }
        public ICommand AddDirectoryCommand { get; }
        public ICommand CopyFileCommand { get; }
        public ICommand CreateViewFolderCommand { get; }
        public ICommand SizeChangeCommand { get; }
        public ICommand FolderViewNameChangeCommand { get; }
        public ICommand RemoveExelFileCommand { get; }
        public ICommand RemoveFolderViewCommand { get; }
        //
        private ConverterListInListForView converterListInListForView;
        //
        private ExelSettingsRead exelSettingsRead;
        //
        private List<Thread> loadedTabels;
        private LoadedTablesManager loadedTablesManager;
        private Dictionary<ExelFilePage, TablesStates> TablesVariations;
        public ExelViewModel(WindowFileBase windowFileBase)
        {
            this.windowFileBase = windowFileBase;
            //obs           
            ExelFilePages = new ObservableCollection<ExelFilePage>();
            Directories = new ObservableCollection<Directory>();
            ExelFilesСontentInDirectoriesEquals = new ObservableCollection<string>();
            ExelFilesСontentInDirectoriesNoEquals = new ObservableCollection<string>();
            ColumnsNamesInPage = new ObservableCollection<ConverterListInListForView>();
            FoldersView = new ObservableCollection<FolderView>();
            TableInfo = new ObservableCollection<string>(new List<string>
            {
                "Все",
                "Найденные",
                "Отсутствующие"
            });
            //
            //command
            AddFileCommand = new AddExelFileCommand(this);
            AddDirectoryCommand = new AddDirectoryCommand(this);
            CopyFileCommand = new CopyExelFileCommand(this);
            AddExelFileDropCommand = new AddExelFileDropCommand(this);
            CreateViewFolderCommand = new CreateFolderCommand(this);
            SizeChangeCommand = new SizeChangeCommand(this, windowFileBase);
            FolderViewNameChangeCommand = new FolderViewNameChangeCommand(this, windowFileBase);
            RemoveExelFileCommand = new RemoveExelFileCommand(this);
            RemoveFolderViewCommand = new RemoveFolderViewCommand(this);
            //
            converterListInListForView = new ConverterListInListForView();
            //
            exelSettingsRead = windowFileBase.settings.ExelSettingsRead;
            loadedTabels = new List<Thread>();
            TablesVariations = new Dictionary<ExelFilePage, TablesStates>();
            //start
            AddFolderView("Файлы");
            loadedTablesManager = new LoadedTablesManager();
            loadedTablesManager.LoadingTableChange += OnLoadingTableChange;
            loadedTablesManager.Start();
            

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
                ChangePageTables();
            }
        }

        private bool showDuplicatesInDataView;
        public bool ShowDuplicatesInDataView
        {
            get
            {
                return showDuplicatesInDataView;
            }
            set
            {              
                showDuplicatesInDataView = value;
                OnPropertyChanged(nameof(ShowDuplicatesInDataView));
                ChangePageTables();
            }
        }

        private DataView dataView;
        public DataView DataView
        {
            get
            {
                return dataView;
            }
            set
            {
                dataView = value;
                OnPropertyChanged(nameof(DataView));
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
        private void OnLoadingTableChange(object sender, EventArgs e)
        {
            LoadedTablesManager loadedTablesManager = sender as LoadedTablesManager;
            LoadingTable = loadedTablesManager.LoadingTable;
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
                    SetExelFilePages(SelectedExelFile.ExelPages);
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
                    UpdateDirectories();
                }
            }
        }
        private Dictionary<string, List<object>> selectedPageInfo = new Dictionary<string, List<object>>
        {
            { "ColumnsData", new List<object>() },
            { "ColumnsDataDplicates", new List<object>()  },
            { "ColumnsDataNoDplicates", new List<object>()  }
        };
        public Dictionary<string, List<object>> SelectedPageInfo
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
        //viewmodelbase 

        public void AddExelFile(List<ExelFile> AddedFiles, string FolderName)
        {            
            foreach (ExelFile exelFile in AddedFiles)
            {
                AddExelFile(exelFile, FolderName);
            }
        }

        public void AddExelFile(ExelFile AddedFile, string FolderName)
        {
            windowFileBase.exelWindowFileBase.AddFile(AddedFile, FolderName);
            FoldersView.FirstOrDefault(item => item.Name == FolderName).Files.Add(AddedFile);
            ReadExelFile(AddedFile);
        }
        public void RemoveExelFile(List<ExelFile> removedFiles, string FolderName)
        {
            foreach (ExelFile exelFile in removedFiles)
            {
                RemoveExelFile(exelFile, FolderName);
            }
        }
        public void RemoveExelFile(ExelFile removedFile, string FolderName)
        {
            windowFileBase.exelWindowFileBase.RemoveFile(removedFile, FolderName);
            FolderView folderView = FoldersView.FirstOrDefault(item => item.Name == FolderName);
            if (folderView == null)
            {
                return;
            }
            if (folderView.Files.Contains(removedFile))
            {
                folderView.Files.Remove(removedFile);
            }
            if (SelectedExelFile == removedFile)
            {
                ClearFileInfo();
            }
            ClearTablesVariationsInfo(removedFile);
        }
        public void RemoveFolderView(string folderName)
        {
            windowFileBase.exelWindowFileBase.RemoveFolderWithFiles(folderName);
            FolderView removedFolder = FoldersView.FirstOrDefault(item => item.Name == folderName);
            foreach (ExelFile exelFile in removedFolder.Files)
            {
                RemoveExelFile(exelFile, folderName);
            }            
            FoldersView.Remove(removedFolder);
        }
        public void ReadExelFile(List<ExelFile> AddedFiles)
        {
            foreach (ExelFile exelFile in AddedFiles)
            {
                ReadExelFile(exelFile);
            }
        }
        public void ReadExelFile(ExelFile AddedFile)
        {
            bool result = false;
            Thread readExelFile = new Thread(() =>
            {
                result = windowFileBase.exelWindowFileBase.TryReadExelFile(AddedFile);
            });
            readExelFile.Start();
            if (result == false)
            {

            }
        }
        public void AddDirectory(Directory AddedDirectory, ExelFile BindingFile)
        {
            ExelWindowFileBase exelWindowFileBase = windowFileBase.exelWindowFileBase;
            if (exelWindowFileBase.TryAddDirectory(AddedDirectory, BindingFile) == false)
            {
                return;
            }
            SearchFilesResult searchFilesResult = exelWindowFileBase.SearchFilesResults.FirstOrDefault(item => item.ExelFile == BindingFile && item.NameColumn == SelectedColumnName.StringValue);
            if (searchFilesResult == null)
            {
                searchFilesResult = new SearchFilesResult();
                searchFilesResult.SetExelFile(SelectedExelFile);
                searchFilesResult.SetNameColumn(selectedColumnName.StringValue);
            }
            searchFilesResult.AddFilesInDirectory(AddedDirectory, SearchFilesInDirectoryies(AddedDirectory, selectedColumnName.StringValue));
            exelWindowFileBase.TryAddSearchFilesResult(searchFilesResult);
            UpdateDirectories();
        }
        public void RemoveDirectory(Directory RemovedDirectory, ExelFile BindingFile)
        {
            ExelWindowFileBase exelWindowFileBase = windowFileBase.exelWindowFileBase;
            if (exelWindowFileBase.TryRemoveDirectory(RemovedDirectory, BindingFile) == false)
            {
                return;
            }
            if (exelWindowFileBase.TryRemoveDirectoryInSearchFilesResult(RemovedDirectory, BindingFile))
            {
                return;
            }
            UpdateDirectories();
        }
        //viewmodelbase 

        private void ClearFileInfo()
        {
            dataView = null;
            OnPropertyChanged(nameof(DataView));            
            selectedColumnName = null;
            OnPropertyChanged(nameof(SelectedColumnName));
            selectedPage = null;
            OnPropertyChanged(nameof(SelectedPage));
            ColumnsNamesInPage.Clear();
            ExelFilePages.Clear();
            ExelFilesСontentInDirectoriesEquals.Clear();
            ExelFilesСontentInDirectoriesNoEquals.Clear();
            Directories.Clear();
            selectedExelFile = null;
            OnPropertyChanged(nameof(SelectedExelFile));
            foreach (string key in SelectedPageInfo.Keys)
            {
                SelectedPageInfo[key].Clear();
            }
            OnPropertyChanged(nameof(SelectedPageInfo));            
        }      
        private void ClearTablesVariationsInfo(ExelFile removedFile)
        {
            foreach (ExelFilePage page in removedFile.ExelPages)
            {
                TablesVariations.Remove(page);
            }
            
        }


        private bool TryGetTableVariations(ExelFilePage exelFilePage, TablesStates.TableVariation tableVariation, TablesStates.DuplicatesOption duplicatesOption, out DataView dataView)
        {
            dataView = new DataView();
            if (TablesVariations.TryGetValue(exelFilePage, out TablesStates tablesStates) == true)
            {
                if (tablesStates.TryGetTables(tableVariation, duplicatesOption, out dataView))
                {
                    return true;
                }               
            }
            return false;
        }
        private bool TryAddTableVariations(ExelFilePage exelFilePage, TablesStates.TableVariation tableVariation, TablesStates.DuplicatesOption duplicatesOption, DataView dataView)
        {
            if (TablesVariations.ContainsKey(exelFilePage) == false)
            {
                if (TablesVariations.TryAdd(exelFilePage, new TablesStates()) == false)
                {
                    return false;
                }                             
            }
            if (TablesVariations[exelFilePage].TryAddTable(tableVariation, duplicatesOption, dataView) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private DataView GetOrCreateTableVariations(ExelFilePage exelFilePage, TablesStates.TableVariation tableVariation, TablesStates.DuplicatesOption duplicatesOption, List<ExelFilePageTable> exelFilePageTables)
        {            
            if (TryGetTableVariations(SelectedPage, tableVariation, duplicatesOption, out DataView dataView) == true)
            {
                return dataView;
            }
            else
            {
                List<DataTable> dataTables = new List<DataTable>();
                dataTables.AddRange(exelFilePageTables.Select(item => item.ToDataTabel()));
                dataView = MergeDataTables(dataTables).DefaultView;
                TryAddTableVariations(exelFilePage, tableVariation, duplicatesOption, dataView);
            }
            return dataView;
        }
        private DataView GetTabelsForTableInfo()
        {            
            List<ExelFilePageTable> tables = new List<ExelFilePageTable>();
            DataView dataView = new DataView();
            TablesStates.DuplicatesOption duplicatesOption;
            if (ShowDuplicatesInDataView == true)
            {
                duplicatesOption = TablesStates.DuplicatesOption.WithDuplicates;
            }
            else
            {
                duplicatesOption = TablesStates.DuplicatesOption.WithoutDuplicates;
            }
            switch (selectedTableInfo)
            {
                case "Все":
                    if (TryGetTableVariations(SelectedPage, TablesStates.TableVariation.All, duplicatesOption, out dataView) == false)
                    {
                        tables = SelectedPage.Tabeles;
                        if (duplicatesOption == TablesStates.DuplicatesOption.WithDuplicates)
                        {                            
                            tables = tables.Select(item => item.RemoveDuplicatesByColumn(SelectedColumnName?.StringValue)).ToList();
                        }
                        dataView = GetOrCreateTableVariations(SelectedPage, TablesStates.TableVariation.All, duplicatesOption, tables);
                    }                                                         
                    break;
                case "Найденные":
                    if (TryGetTableVariations(SelectedPage, TablesStates.TableVariation.Found, duplicatesOption, out dataView) == false)
                    {
                        tables = CreateTableEquals(new List<object>(ExelFilesСontentInDirectoriesEquals.ToList()));
                        if (duplicatesOption == TablesStates.DuplicatesOption.WithDuplicates)
                        {
                            tables = tables.Select(item => item.RemoveDuplicatesByColumn(SelectedColumnName?.StringValue)).ToList();
                        }
                        dataView = GetOrCreateTableVariations(SelectedPage, TablesStates.TableVariation.Found, duplicatesOption, tables);
                    }                                       
                    break;
                case "Отсутствующие":
                    if (TryGetTableVariations(SelectedPage, TablesStates.TableVariation.Missing, duplicatesOption, out dataView) == false)
                    {
                        tables = CreateTableEquals(new List<object>(ExelFilesСontentInDirectoriesNoEquals.ToList()));
                        if (duplicatesOption == TablesStates.DuplicatesOption.WithDuplicates)
                        {
                            tables = tables.Select(item => item.RemoveDuplicatesByColumn(SelectedColumnName?.StringValue)).ToList();
                        }
                        dataView = GetOrCreateTableVariations(SelectedPage, TablesStates.TableVariation.Missing, duplicatesOption, tables);
                    }                                        
                    break;       
            }
            return dataView;
        }

        public List<ExelFilePageTable> CreateTableEquals(List<object> list)
        {
            List<ExelFilePageTable> exelFilePageTables = new List<ExelFilePageTable>();
            ExelFilePageTable exelFilePageTable = new ExelFilePageTable();            
            for (int i = 0; i < selectedPage.Tabeles.Count; i++)
            {
                List<ExelFilePageTableRow> rows = new List<ExelFilePageTableRow>();
                exelFilePageTable = new ExelFilePageTable(selectedPage.Tabeles[i].TableColumns.Keys.ToList());
                rows.AddRange(selectedPage.Tabeles[i].SearchRowsByColumn(SelectedColumnName.StringValue, list));
                exelFilePageTable.AddRow(rows);
                if (exelFilePageTable.Rows.Count > 0)
                {
                    exelFilePageTables.Add(exelFilePageTable);
                }                
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
        public void AddFolderView(string nameFolder)
        {
            windowFileBase.exelWindowFileBase.AddFolder(nameFolder);
            FoldersView.Add(new FolderView(nameFolder));
        }
        private void SelectMainColumnNameInPage(string MainColumnName)
        {
            List<ConverterListInListForView> tempList = ColumnsNamesInPage.ToList();
            int searchIndex = 0;
            if (tempList.Count == 0)
            {
                return;
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
                SelectedPageInfo["ColumnsData"] = ColumnsData;
                List<object> ColumnsDataDplicates = new List<object>(SelectedPage.GetColumnsDataDplicates(ColumnName));
                ColumnsDataDplicates.RemoveAll(item => item == null);
                SelectedPageInfo["ColumnsDataDplicates"] = ColumnsDataDplicates;
                List<object> ColumnsDataNoDplicates = new List<object>(SelectedPage.GetColumnsDataNoDplicates(ColumnName));
                ColumnsDataNoDplicates.RemoveAll(item => item == null);
                SelectedPageInfo["ColumnsDataNoDplicates"] = ColumnsDataNoDplicates;
            }
            else
            {
                SelectedPageInfo["ColumnsData"] = new List<object>();
                SelectedPageInfo["ColumnsDataDplicates"] = new List<object>();
                SelectedPageInfo["ColumnsDataNoDplicates"] = new List<object>();
            }
            OnPropertyChanged(nameof(SelectedPageInfo));
        }
        
        private void ChangePageTables()
        {
            Thread thread = new Thread(() =>
            {
                if (SelectedPage != null)
                {
                    DataView dataView = GetTabelsForTableInfo();                    
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        DataView = dataView;
                    });
                }
            });
            loadedTablesManager.LoadTable(thread);
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
        private bool FolderViewExist(string folderName)
        {
            if (windowFileBase.exelWindowFileBase.FoldersWithFiles.Find(item => item.Name == folderName) == null)
            {
                return false;
            }
            else
            {
                return true;
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

        public void UpdateDirectories() 
        {
            if (TablesVariations.ContainsKey(SelectedPage) == true)
            {
                TablesVariations[SelectedPage].RemoveAllTableVariation(TablesStates.TableVariation.Found);
                TablesVariations[SelectedPage].RemoveAllTableVariation(TablesStates.TableVariation.Missing);
                ChangePageTables();
            }
            ExelWindowFileBase exelWindowFileBase = windowFileBase.exelWindowFileBase;            
            if (exelWindowFileBase.SearchFilesResults.Count == 0)
            {
                return;
            }
            Directories.Clear();
            if (exelWindowFileBase.DirectoriesBelongExelFile.TryGetValue(SelectedExelFile, out List<Directory> directoryies) == true)
            {
                Directories = new ObservableCollection<Directory>(directoryies);
                OnPropertyChanged(nameof(Directories));
            }

            ExelFilesСontentInDirectoriesEquals.Clear();
            SearchFilesResult searchFilesResult = exelWindowFileBase.SearchFilesResults.FirstOrDefault(item => item.ExelFile == SelectedExelFile && item.NameColumn == SelectedColumnName.StringValue);
            ExelFilesСontentInDirectoriesEquals = new ObservableCollection<string>(searchFilesResult?.GetAllFiles().Select(file => file.FileName)?.ToList());
            OnPropertyChanged(nameof(ExelFilesСontentInDirectoriesEquals));

            ExelFilesСontentInDirectoriesNoEquals.Clear();
            List<string> сolumnsDataNoDplicates = selectedPageInfo["ColumnsDataNoDplicates"].ConvertAll(x => Convert.ToString(x)); ////////////////////////////////["ColumnsDataNoDplicates"]
            var exelFilesСontentInDirectoriesNoEquals = сolumnsDataNoDplicates?.Where(i => !ExelFilesСontentInDirectoriesEquals.Contains(i))?.ToList();
            ExelFilesСontentInDirectoriesNoEquals = new ObservableCollection<string>(exelFilesСontentInDirectoriesNoEquals);
            OnPropertyChanged(nameof(ExelFilesСontentInDirectoriesNoEquals));
        }

        private List<Model.File> SearchFilesInDirectoryies(Directory directory, string nameColumn)
        {
            List<Model.File> EqualsFile = directory.SearchFileToName(selectedPageInfo["ColumnsDataNoDplicates"].ConvertAll(x => Convert.ToString(x)).OfType<string>().ToList(), Formats.pdf);
            return EqualsFile;
        }
        public void SetFolderViewName(string oldName, string newName)
        {
            newName = newName.TrimEnd();
            newName = newName.TrimStart();
            FolderView oldNameFolderView = FoldersView.FirstOrDefault(element => element.Name == oldName);
            FolderView newNameFolderView = FoldersView.FirstOrDefault(element => element.Name == newName);
            if (oldName == newName  && newNameFolderView == oldNameFolderView)
            {
                windowFileBase.exelWindowFileBase.SetFolerName(oldName, newName);
                oldNameFolderView.CorrectName = true;
                oldNameFolderView.Name = newName;
                return;
            }           
            if (newNameFolderView == null)
            {
                windowFileBase.exelWindowFileBase.SetFolerName(oldName, newName);
                oldNameFolderView.CorrectName = true;
                oldNameFolderView.Name = newName;
            }
            else
            {
                oldNameFolderView.CorrectName = false;
            }
        }
        private class TablesStates
        {
            private Dictionary<(TableVariation, DuplicatesOption), DataView> tables = new Dictionary<(TableVariation, DuplicatesOption), DataView>();          
            public bool TryAddTable(TableVariation variation, DuplicatesOption duplicatesOption, DataView table)
            {
                if (tables.TryAdd((variation, duplicatesOption), table) == true)
                {
                    return true;
                }
                return false;
            }

            public bool TryGetTables(TableVariation variation, DuplicatesOption duplicatesOption, out DataView table)
            {
                table = new DataView();
                if (tables.TryGetValue((variation, duplicatesOption), out DataView tablesResult))
                {
                    table = tablesResult;
                    return true;
                }
                return false;
            }  
            public void RemoveAllTableVariation(TableVariation variation)
            {
                List<(TableVariation, DuplicatesOption)> keys = tables.Where(item => item.Key.Item1 == variation).Select(item => item.Key).ToList();
                foreach ((TableVariation, DuplicatesOption) key in keys)
                {
                    tables.Remove(key);
                }
            }
            public enum TableVariation
            {
                All,
                Found,
                Missing
            }
            public enum DuplicatesOption
            {
                WithDuplicates,
                WithoutDuplicates
            }
        }
        public override void Deactivation()
        {
            throw new NotImplementedException();
        }
    }
}
