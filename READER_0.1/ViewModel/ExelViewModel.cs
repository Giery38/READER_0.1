using MS.WindowsAPICodePack.Internal;
using READER_0._1.Command.CommandExcel;
using READER_0._1.Model;
using READER_0._1.Model.Excel;
using READER_0._1.Model.Excel.Settings;
using READER_0._1.Model.Excel.TableData;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static DevExpress.Data.Helpers.FindSearchRichParser;
using static READER_0._1.Model.Excel.Settings.ExcelSettingsSearchFiles;

namespace READER_0._1.ViewModel
{
    public class ExcelViewModel : ViewModelBase
    {
        public readonly WindowFileBase windowFileBase;
        public ObservableCollection<Page> Pages { get; private set; }
        public ObservableCollection<ModifiedDirectory> Directories { get; private set; }
        public ObservableCollection<FolderView> FoldersView { get; private set; }
        public ObservableCollection<string> ExcelFilesСontentInDirectoriesEquals { get; private set; }
        public ObservableCollection<string> ExcelFilesСontentInDirectoriesNoEquals { get; private set; }
        public ObservableCollection<string> ColumnsNamesInPage { get; private set; }
        public ObservableCollection<string> TableInfo { get; private set; }                       
        //
        public ICommand AddFileCommand { get; }
        public ICommand AddExcelFileDropCommand { get; }
        public ICommand AddDirectoryCommand { get; }
        public ICommand CopyFileCommand { get; }
        public ICommand CreateViewFolderCommand { get; }
        public ICommand SizeChangeCommand { get; }
        public ICommand FolderViewNameChangeCommand { get; }
        public ICommand RemoveExcelFileCommand { get; }
        public ICommand RemoveFolderViewCommand { get; }
        public ICommand RepaintRowsCommand { get; }
        
        //
        private ConverterListInListForView converterListInListForView;
        //
        private ExcelSettings excelSettings;
        //
        private Dictionary<Page, TablesStates> TablesVariations;
        public ExcelViewModel(WindowFileBase windowFileBase)
        {
            this.windowFileBase = windowFileBase;
            #region Collectios
            Pages = new ObservableCollection<Page>();
            Directories = new ObservableCollection<ModifiedDirectory>();
            ExcelFilesСontentInDirectoriesEquals = new ObservableCollection<string>();
            ExcelFilesСontentInDirectoriesNoEquals = new ObservableCollection<string>();
            ColumnsNamesInPage = new ObservableCollection<string>();
            FoldersView = new ObservableCollection<FolderView>();
            RecentColors = new ObservableCollection<Color>();
            TableInfo = new ObservableCollection<string>(new List<string>
            {
                "Все",
                "Найденные",
                "Отсутствующие"
            });
            InfoPages = new ObservableCollection<string>(new List<string>
            {
                "MainInfo",
                "DetailedInfo"
            });
            SelectedTableInfoReport = new ObservableCollection<string>();
            #endregion 
            #region Command
            AddFileCommand = new AddExcelFileCommand(this);
            AddDirectoryCommand = new AddDirectoryCommand(this);
            CopyFileCommand = new CopyExcelFileCommand(this);
            AddExcelFileDropCommand = new AddExcelFileDropCommand(this);
            CreateViewFolderCommand = new CreateFolderCommand(this);
            SizeChangeCommand = new SizeChangeCommand(this, windowFileBase);           
            RemoveExcelFileCommand = new RemoveExcelFileCommand(this);
            RemoveFolderViewCommand = new RemoveFolderViewCommand(this);
            RepaintRowsCommand = new RepaintRowsCommand(this);
            CreateReportCommand = new CreateReportCommand(this);
            ChangeInfoPageCommand = new ChangeInfoPageCommand(this);
            #endregion
            converterListInListForView = new ConverterListInListForView();            
            excelSettings = windowFileBase.settings.ExcelSettings;
            TablesVariations = new Dictionary<Page, TablesStates>();           
            //start
            AddFolderView("Файлы");
            CurrentInfoPage = InfoPages[0];
            SelecedColor = Colors.Black;

            //ThreadsManagaer gg = new ThreadsManagaer("gag", 20);
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
                if (value != null)
                {
                    selectedTableInfo = value;
                    OnPropertyChanged(nameof(SelectedTableInfo));
                    ChangePageTablse();
                }                
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
                if (showDuplicatesInDataView != value)
                {
                    showDuplicatesInDataView = value;
                    OnPropertyChanged(nameof(ShowDuplicatesInDataView));
                    ChangePageTablse();
                }              
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
        private ExcelFile selectedExcelFile;
        public ExcelFile SelectedExcelFile
        {
            get
            {
                return selectedExcelFile;
            }
            set
            {
                if (value != null)
                {
                    selectedExcelFile = value;
                    OnPropertyChanged(nameof(SelectedExcelFile));
                    SetPages(SelectedExcelFile.ExcelPages);
                    SelectFirstPageInSelectedFile();
                    if (SelectedExcelFile.ExcelPages.Count > 1)
                    {
                        CreateUnitedPage();
                    }
                }
            }
        }

        private Page selectedPage;
        public Page SelectedPage
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
                    //SelectedColumnName = ColumnsNamesInPage[0];
                    SelectMainColumnNameInPage("Номера отправки");///////////////////////////////////////////////////////                                        
                }

            }
        }        
        private string selectedColumnName;
        public string SelectedColumnName
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
                    ChangePageInfo(value);
                }
                UpdateDirectories();               
            }
        }       
        private Dictionary<string, ObservableCollection<object>> selectedPageInfo = new Dictionary<string, ObservableCollection<object>>
        {

            { "ColumnsData", new ObservableCollection<object>() },
            { "ColumnsDataDplicates", new ObservableCollection<object>()  },
            { "ColumnsDataNoDplicates", new ObservableCollection<object>()  }
        };
        public Dictionary<string, ObservableCollection<object>> SelectedPageInfo
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
        #region InternalNavigation
        public ObservableCollection<string> InfoPages { get; private set; }
        private string currentInfoPage;
        public string CurrentInfoPage
        {
            get
            {             
                return currentInfoPage;
            }
            set
            {
                currentInfoPage = value;
                OnPropertyChanged(nameof(CurrentInfoPage));
                switch (CurrentInfoPage)
                {
                    case "MainInfo":
                        OnPropertyChanged(nameof(SelectedPageInfo));                        
                        break;
                    case "DetailedInfo":
                        if (SelectedTableInfo == null)
                        {
                            SelectedTableInfo = TableInfo[0];
                        }
                        else
                        {
                            OnPropertyChanged(nameof(SelectedTableInfo));
                        }                        
                        break;
                }
            }
        }
        public ICommand ChangeInfoPageCommand { get; }
        #endregion
        #region ColorPicker
        public ObservableCollection<Color> RecentColors { get; private set; }

        private Color selecedColor;
        public Color SelecedColor
        {
            get
            {
                return selecedColor;
            }
            set
            {
                selecedColor = value;
                OnPropertyChanged(nameof(SelecedColor));
            }
        }

        private Color[] recentColors = new Color[10];
        public void AddRecentColor(Color addedColor)
        {
            int index = Array.IndexOf(recentColors, addedColor);
            if (index == 0)
            {
                return;
            }
            Color newColor = new Color()
            {
                A = addedColor.A,
                R = addedColor.R,
                G = addedColor.G,
                B = addedColor.B
            };
            if (index > 0)
            {
                for (int i = index; i > 0; i--)
                {
                    recentColors[i] = recentColors[i - 1];
                }
            }
            else
            {
                for (int i = recentColors.Length - 1; i > 0; i--)
                {
                    recentColors[i] = recentColors[i - 1];
                }
            }
            recentColors[0] = newColor;
            RecentColors = new ObservableCollection<Color>(recentColors.Where(c => c != default(Color)));
            OnPropertyChanged(nameof(RecentColors));
        }
        #endregion
        #region Table
        #region CreateReport
        public ObservableCollection<string> SelectedTableInfoReport { get; private set; }
        private bool getDuplicatesReport;
        public bool GetDuplicatesReport
        {
            get
            {
                return getDuplicatesReport;
            }
            set
            {
                getDuplicatesReport = value;
                OnPropertyChanged(nameof(GetDuplicatesReport));
            }
        }
        ICommand CreateReportCommand;
        #endregion
        #endregion

        private FolderView FindFileInFolders(ExcelFile excelFile)
        {
            for (int folder = 0; folder < FoldersView.Count; folder++)
            {
                for (int file = 0; file < FoldersView[folder].Files.Count; file++)
                {
                    if (FoldersView[folder].Files[file].Equals(excelFile) == true)
                    {
                        return FoldersView[folder];
                    }
                }
            }
            return null;
        }
        //viewmodelbase 

        public void AddExcelFile(List<ExcelFile> addedFiles, Guid folderId)
        {
            foreach (ExcelFile excelFile in addedFiles)
            {
                AddExcelFile(excelFile, folderId);
            }
        }

        public void AddExcelFile(ExcelFile addedFile, Guid folderId)
        {
            if (FindFileInFolders(addedFile) != null)
            {               
                return;
            }           
            FolderView folderView = FoldersView.FirstOrDefault(item => item.Id == folderId);
            folderView.Files.Add(addedFile);
            windowFileBase.excelWindowFileBase.AddFile(addedFile, folderId);
            ReadExcelFile(addedFile, folderView.Parameter as ExcelSettingsRead); 
        }

        public void RemoveExcelFile(List<ExcelFile> removedFiles, FolderView folderView)
        {
            foreach (ExcelFile excelFile in removedFiles)
            {
                RemoveExcelFile(excelFile, folderView);
            }
        }        
        public void RemoveExcelFile(ExcelFile removedFile, FolderView folderView)
        {         
            if (folderView.Files.Contains(removedFile))
            {
                folderView.Files.Remove(removedFile);
            }
            if (windowFileBase.excelWindowFileBase.FoldersWithFiles.Find(item => item.Name == folderView.Name) != null)
            {
                windowFileBase.excelWindowFileBase.RemoveFile(removedFile, folderView.Id);
            }
            if (folderView == null)
            {
                return;
            }
            if (SelectedExcelFile == removedFile)
            {
                ClearFileInfo();
            }
            ClearTablesVariationsInfo(removedFile);

        }
        public void RemoveFolderView(FolderView removedFolder)
        {           
            FoldersView.Remove(removedFolder);            
            for (int i = 0; i < removedFolder.Files.Count; i++)
            {
                RemoveExcelFile(removedFolder.Files[i] as ExcelFile, removedFolder);
            }
            windowFileBase.excelWindowFileBase.RemoveFolder(removedFolder.Id);
        }
                        
        public void ReadExcelFile(ExcelFile AddedFile, ExcelSettingsRead excelSettingsRead)
        {                                
            windowFileBase.excelWindowFileBase.ReadExcelFile(AddedFile, excelSettingsRead);           
        }
        public void AddDirectory(ModifiedDirectory AddedDirectory, ExcelFile BindingFile)
        {
            ExcelWindowFileBase excelWindowFileBase = windowFileBase.excelWindowFileBase;
            if (excelWindowFileBase.TryAddDirectory(AddedDirectory, BindingFile) == false)
            {
                return;
            }
            SearchFilesResult searchFilesResult = excelWindowFileBase.SearchFilesResults.FirstOrDefault(item => item.ExcelFile == BindingFile && item.NameColumn == SelectedColumnName);
            if (searchFilesResult == null)
            {
                searchFilesResult = new SearchFilesResult(SelectedExcelFile, selectedColumnName);                
                excelWindowFileBase.TryAddSearchFilesResult(searchFilesResult);                
            }
            searchFilesResult.AddFilesInDirectory(AddedDirectory, SearchFilesInDirectories(AddedDirectory));
            UpdateDirectories();
        }
        public void RemoveDirectory(ModifiedDirectory RemovedDirectory, ExcelFile BindingFile)
        {
            ExcelWindowFileBase excelWindowFileBase = windowFileBase.excelWindowFileBase;
            if (excelWindowFileBase.TryRemoveDirectory(RemovedDirectory, BindingFile) == false)
            {
                return;
            }
            if (excelWindowFileBase.TryRemoveDirectoryInSearchFilesResult(RemovedDirectory, BindingFile))
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
            Pages.Clear();
            ExcelFilesСontentInDirectoriesEquals.Clear();
            ExcelFilesСontentInDirectoriesNoEquals.Clear();
            Directories.Clear();
            selectedExcelFile = null;
            OnPropertyChanged(nameof(SelectedExcelFile));
            foreach (string key in SelectedPageInfo.Keys)
            {
                SelectedPageInfo[key].Clear();
            }
            OnPropertyChanged(nameof(SelectedPageInfo));            
        }      
        private void ClearTablesVariationsInfo(ExcelFile removedFile)
        {
            foreach (Page page in removedFile.ExcelPages)
            {
                TablesVariations.Remove(page);
            }
            
        }


        private bool TryGetTableVariations(Page page, TablesStates.TableVariation tableVariation, TablesStates.DuplicatesOption duplicatesOption, out DataView dataView)
        {
            dataView = new DataView();
            if (TablesVariations.TryGetValue(page, out TablesStates tablesStates) == true)
            {
                if (tablesStates.TryGetTables(tableVariation, duplicatesOption, out dataView))
                {
                    return true;
                }               
            }
            return false;
        }
        private bool TryAddTableVariations(Page page, TablesStates.TableVariation tableVariation, TablesStates.DuplicatesOption duplicatesOption, DataView dataView)
        {
            if (TablesVariations.ContainsKey(page) == false)
            {
                if (TablesVariations.TryAdd(page, new TablesStates()) == false)
                {
                    return false;
                }                             
            }
            if (TablesVariations[page].TryAddTable(tableVariation, duplicatesOption, dataView) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private DataView GetOrCreateTableVariations(Page page, TablesStates.TableVariation tableVariation, TablesStates.DuplicatesOption duplicatesOption, List<Table> tables)
        {            
            if (TryGetTableVariations(SelectedPage, tableVariation, duplicatesOption, out DataView dataView) == true)
            {
                return dataView;
            }
            else
            {
                List<DataTable> dataTables = new List<DataTable>();
                dataTables.AddRange(tables.Select(item => item.ToDataTable()));                     
                dataView = MergeDataTables(dataTables).DefaultView;
                TryAddTableVariations(page, tableVariation, duplicatesOption, dataView);                
            }
            return dataView;
        }
        private DataView GetTabelsForTableInfo()
        {            
            List<Table> tables = new List<Table>();
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
                        tables = SelectedPage.Tables;                                                           
                        if (duplicatesOption == TablesStates.DuplicatesOption.WithDuplicates)
                        {                            
                            tables = tables.Select(item => item.RemoveDuplicatesByColumn(SelectedColumnName)).ToList();
                        }
                        dataView = GetOrCreateTableVariations(SelectedPage, TablesStates.TableVariation.All, duplicatesOption, tables);
                    }                                                         
                    break;
                case "Найденные": 
                    if (TryGetTableVariations(SelectedPage, TablesStates.TableVariation.Found, duplicatesOption, out dataView) == false)
                    {
                        tables = CreateTableEquals(new List<object>(ExcelFilesСontentInDirectoriesEquals.ToList()));
                        if (duplicatesOption == TablesStates.DuplicatesOption.WithDuplicates)
                        {
                            tables = tables.Select(item => item.RemoveDuplicatesByColumn(SelectedColumnName)).ToList();
                        }
                        dataView = GetOrCreateTableVariations(SelectedPage, TablesStates.TableVariation.Found, duplicatesOption, tables);
                    }                                       
                    break;
                case "Отсутствующие":
                    if (TryGetTableVariations(SelectedPage, TablesStates.TableVariation.Missing, duplicatesOption, out dataView) == false)
                    {
                        tables = CreateTableEquals(new List<object>(ExcelFilesСontentInDirectoriesNoEquals.ToList()));
                        if (duplicatesOption == TablesStates.DuplicatesOption.WithDuplicates)
                        {
                            tables = tables.Select(item => item.RemoveDuplicatesByColumn(SelectedColumnName)).ToList();
                        }
                        dataView = GetOrCreateTableVariations(SelectedPage, TablesStates.TableVariation.Missing, duplicatesOption, tables);
                    }                                        
                    break;       
            }
            return dataView;
        }

        public List<Table> CreateTableEquals(List<object> list) 
        {
            List<Table> tables = new List<Table>();
            Table table = new Table();
            Page page = Pages.FirstOrDefault(item => item == SelectedPage);
            string selectedColumnName = new string(SelectedColumnName);
            for (int i = 0; i < page.Tables.Count; i++)
            {
                List<Row> rows = new List<Row>();
                table = new Table(page.Tables[i].TableColumns.Keys.ToList());
                rows.AddRange(page.Tables[i].SearchRowsByColumn(selectedColumnName, list));
                table.AddRow(rows);                
                tables.Add(table);
            }
            return tables;
        }        

        private void SetPages(List<Page> pages)
        {
            Pages = new ObservableCollection<Page>(pages);
            OnPropertyChanged(nameof(Pages));
        }
        private void SelectFirstPageInSelectedFile()
        {
            if (SelectedExcelFile != null)
            {
                if (Pages.Count > 0)
                {
                    SelectedPage = Pages[0];
                }
            }
        }
        public void AddFolderView(string nameFolder)
        {
            Directory directory = new Directory(nameFolder);
            windowFileBase.excelWindowFileBase.AddFolder(directory);
            FolderView folderView = new FolderView(nameFolder, typeof(ExcelFile), directory.Id);
            folderView.Parameter = new ExcelSettingsRead(excelSettings.ExcelSettingsRead);
            FoldersView.Add(folderView);
        }
        private void SelectMainColumnNameInPage(string MainColumnName)
        {
            if (ColumnsNamesInPage.Count == 0)
            {
                return;
            }
            List<string> valueolumnNameValue = excelSettings.ExcelSettingsRead.SearchingColumnNames.Find(item => item.Name == MainColumnName).Values;
            foreach (string name in valueolumnNameValue)
            {
                string value = ColumnsNamesInPage.FirstOrDefault(item => item == name);
                if (value != null)
                {
                    SelectedColumnName = value;
                    break;
                }
            }
            if (SelectedColumnName == null)
            {
                SelectedColumnName = ColumnsNamesInPage[0];
            }          
        }
        private void UpdateColumnsNamesInPage()
        {
            if (SelectedPage != null)
            {
                ColumnsNamesInPage.Clear();           
                ColumnsNamesInPage = new ObservableCollection<string>(SelectedPage.GetColumnsNameAllTabels());
                OnPropertyChanged(nameof(ColumnsNamesInPage));               
            }
        }

        private void ChangePageInfo(string ColumnName)
        {
            if (SelectedPage != null &&
                ColumnName != null &&
                    SelectedPage.GetColumnsData(ColumnName) != null)                
            {
                List<object> ColumnsData = new List<object>(SelectedPage.GetColumnsData(ColumnName));
                ColumnsData.RemoveAll(item => item == null);
                SelectedPageInfo["ColumnsData"] = new ObservableCollection<object>(ColumnsData);
                List<object> ColumnsDataDuplicates = new List<object>(SelectedPage.GetColumnsDataDuplicates(ColumnName));
                ColumnsDataDuplicates.RemoveAll(item => item == null);
                SelectedPageInfo["ColumnsDataDplicates"] = new ObservableCollection<object>(ColumnsDataDuplicates);
                List<object> ColumnsDataNoDuplicates = new List<object>(SelectedPage.GetColumnsDataNoDuplicates(ColumnName));
                ColumnsDataNoDuplicates.RemoveAll(item => item == null);                    
                SelectedPageInfo["ColumnsDataNoDplicates"] = new ObservableCollection<object>(ColumnsDataNoDuplicates);
            }
            else
            {
                SelectedPageInfo["ColumnsData"] = new ObservableCollection<object>();
                SelectedPageInfo["ColumnsDataDplicates"] = new ObservableCollection<object>();
                SelectedPageInfo["ColumnsDataNoDplicates"] = new ObservableCollection<object>();
            }
            OnPropertyChanged(nameof(SelectedPageInfo));
        }
        private CancellationTokenSource cancellationTokenLoadTabel;
        private Task changePageTablesTask;     
        private async void ChangePageTablse()
        {                 
            if (changePageTablesTask?.IsCompleted == false)
            {
                cancellationTokenLoadTabel.Cancel();
            }
            cancellationTokenLoadTabel = new CancellationTokenSource();
            CancellationToken token = cancellationTokenLoadTabel.Token;
            changePageTablesTask = Task.Run(() =>
            {           
                if (SelectedPage != null)
                {
                    LoadingTable = 1;
                    DataView dataViewChange = GetTabelsForTableInfo();
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        DataView = dataViewChange;
                    });
                    LoadingTable = 0;
                }                
            }, token);
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
            if (SelectedExcelFile != null && Pages.Count > 0)
            {
                Page page = new Page("Все листы");
                foreach (Page item in Pages)
                {
                    page.AddTable(item.Tables);
                }
                Pages.Add(page);

            }
        }       
        private bool FolderViewExist(Guid folderId)
        {
            if (windowFileBase.excelWindowFileBase.FoldersWithFiles.Find(item => item.Id == folderId) == null)
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
                folderView = new FolderView(folderName, typeof(ExcelFile));
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
                ChangePageTablse();
            }
            ExcelWindowFileBase excelWindowFileBase = windowFileBase.excelWindowFileBase;            
            if (excelWindowFileBase.SearchFilesResults.Count == 0)
            {
                return;
            }
            Directories.Clear();
            if (excelWindowFileBase.DirectoriesBelongExcelFile.TryGetValue(SelectedExcelFile, out List<ModifiedDirectory> directoryies) == true)
            {
                Directories = new ObservableCollection<ModifiedDirectory>(directoryies);
                OnPropertyChanged(nameof(Directories));
            }

            ExcelFilesСontentInDirectoriesEquals.Clear();
            SearchFilesResult searchFilesResult = excelWindowFileBase.SearchFilesResults.FirstOrDefault(item => item.ExcelFile == SelectedExcelFile && item.NameColumn == SelectedColumnName);
            ExcelFilesСontentInDirectoriesEquals = new ObservableCollection<string>(searchFilesResult?.GetAllFiles()?.Select(file => file.Name) ?? new List<string>());
            OnPropertyChanged(nameof(ExcelFilesСontentInDirectoriesEquals));

            ExcelFilesСontentInDirectoriesNoEquals.Clear();
            List<string> сolumnsDataNoDplicates = new List<string>(selectedPageInfo["ColumnsDataNoDplicates"].Select(item => item.ToString()));// ConvertAll(x => Convert.ToString(x));        ////////////////////////////////["ColumnsDataNoDplicates"]
            var excelFilesСontentInDirectoriesNoEquals = сolumnsDataNoDplicates?.Where(i => !ExcelFilesСontentInDirectoriesEquals.Contains(i))?.ToList();
            ExcelFilesСontentInDirectoriesNoEquals = new ObservableCollection<string>(excelFilesСontentInDirectoriesNoEquals);
            OnPropertyChanged(nameof(ExcelFilesСontentInDirectoriesNoEquals));
        }

        private List<Model.File> SearchFilesInDirectories(ModifiedDirectory directory)
        {
            List<string> searchingData = new List<string>(selectedPageInfo["ColumnsDataNoDplicates"].Select(item => item.ToString()));
            List<Model.File> EqualsFile = new List<Model.File>();           
            foreach (ConfigurationName configuration in excelSettings.ExcelSettingsSearchFiles.Configurations)
            {
                directory.SetFilesModifiedName(configuration); // сделать от числа зависимость
            }                       
            foreach (string format in excelSettings.ExcelSettingsSearchFiles.FormatsSearch)
            {
                EqualsFile.AddRange(directory.SearchFileToName(searchingData, format, excelSettings.ExcelSettingsSearchFiles.Configurations));
            }                       
            return EqualsFile;
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
    }
}
