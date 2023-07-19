using READER_0._1.Command.CommandExel;
using READER_0._1.Model;
using READER_0._1.Model.Exel;
using READER_0._1.Model.Exel.Settings;
using READER_0._1.Tools;
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

namespace READER_0._1.ViewModel
{
    public class ExelViewModel : ViewModelBase
    {
        public readonly WindowFileBase windowFileBase;
        public ObservableCollection<ExelFilePage> ExelFilePages { get; private set; }
        public ObservableCollection<Directory> Directories { get; private set; }
        public ObservableCollection<FolderView> FoldersView { get; private set; }
        public ObservableCollection<string> ExelFilesСontentInDirectoriesEquals { get; private set; }
        public ObservableCollection<string> ExelFilesСontentInDirectoriesNoEquals { get; private set; }
        public ObservableCollection<string> ColumnsNamesInPage { get; private set; }
        public ObservableCollection<string> TableInfo { get; private set; }
        public ObservableCollection<Color> RecentColors { get; private set; }
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
        public ICommand RepaintRowsCommand { get; }
        
        //
        private ConverterListInListForView converterListInListForView;
        //
        private ExelSettings exelSettings;
        //
        private Dictionary<ExelFilePage, TablesStates> TablesVariations;
        public ExelViewModel(WindowFileBase windowFileBase)
        {
            this.windowFileBase = windowFileBase;
            //obs           
            ExelFilePages = new ObservableCollection<ExelFilePage>();
            Directories = new ObservableCollection<Directory>();
            ExelFilesСontentInDirectoriesEquals = new ObservableCollection<string>();
            ExelFilesСontentInDirectoriesNoEquals = new ObservableCollection<string>();
            ColumnsNamesInPage = new ObservableCollection<string>();
            FoldersView = new ObservableCollection<FolderView>();
            RecentColors = new ObservableCollection<Color>();
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
            RepaintRowsCommand = new RepaintRowsCommand(this);
            //
            converterListInListForView = new ConverterListInListForView();
            //
            exelSettings = windowFileBase.settings.ExelSettings;
            TablesVariations = new Dictionary<ExelFilePage, TablesStates>();
            //start
            AddFolderView("Файлы");
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
                    if (SelectedExelFile.ExelPages.Count > 1)
                    {
                        CreateUnitedPage();
                    }
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
                    //SelectedColumnName = ColumnsNamesInPage[0];
                    SelectMainColumnNameInPage("Номера отправки");///////////////////////////////////////////////////////
                    SelectedTableInfo = TableInfo[0];
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
                }
                UpdateDirectories();
                ChangePageInfo(value);
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
            FolderView folderView = FoldersView.FirstOrDefault(item => item.Name == FolderName);
            folderView.Files.Add(AddedFile);
            ReadExelFile(AddedFile, folderView.ExelSettingsRead);
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
        /*
        public void ReadExelFile(List<ExelFile> AddedFiles)
        {
            foreach (ExelFile exelFile in AddedFiles)
            {
                ReadExelFile(exelFile);
            }
        }
        */
        public void ReadExelFile(ExelFile AddedFile, ExelSettingsRead exelSettingsRead)
        {
            bool result = false;
            Thread readExelFile = new Thread(() =>
            {
                result = windowFileBase.exelWindowFileBase.TryReadExelFile(AddedFile, exelSettingsRead);
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
            SearchFilesResult searchFilesResult = exelWindowFileBase.SearchFilesResults.FirstOrDefault(item => item.ExelFile == BindingFile && item.NameColumn == SelectedColumnName);
            if (searchFilesResult == null)
            {
                searchFilesResult = new SearchFilesResult();
                searchFilesResult.SetExelFile(SelectedExelFile);
                searchFilesResult.SetNameColumn(selectedColumnName);
            }
            searchFilesResult.AddFilesInDirectory(AddedDirectory, SearchFilesInDirectoryies(AddedDirectory, selectedColumnName));
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
                            tables = tables.Select(item => item.RemoveDuplicatesByColumn(SelectedColumnName)).ToList();
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
                            tables = tables.Select(item => item.RemoveDuplicatesByColumn(SelectedColumnName)).ToList();
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
                            tables = tables.Select(item => item.RemoveDuplicatesByColumn(SelectedColumnName)).ToList();
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
                rows.AddRange(selectedPage.Tabeles[i].SearchRowsByColumn(SelectedColumnName, list));
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
        public void AddFolderView(string nameFolder)
        {
            windowFileBase.exelWindowFileBase.AddFolder(nameFolder);
            FolderView folderView = new FolderView(nameFolder);
            folderView.ExelSettingsRead = new ExelSettingsRead(exelSettings.ExelSettingsRead);
            FoldersView.Add(folderView);
        }
        private void SelectMainColumnNameInPage(string MainColumnName)
        {
            if (ColumnsNamesInPage.Count == 0)
            {
                return;
            }
            List<string> valueolumnNameValue = exelSettings.ExelSettingsRead.SearchingColumnNames.Find(item => item.Name == MainColumnName).Values;
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
        private CancellationTokenSource cancellationTokenLoadTabel;
        private Task changePageTablesTask;
#pragma warning disable CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
        private async void ChangePageTables()
#pragma warning restore CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
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
            SearchFilesResult searchFilesResult = exelWindowFileBase.SearchFilesResults.FirstOrDefault(item => item.ExelFile == SelectedExelFile && item.NameColumn == SelectedColumnName);
            ExelFilesСontentInDirectoriesEquals = new ObservableCollection<string>(searchFilesResult?.GetAllFiles()?.Select(file => file.Name) ?? new List<string>());
            OnPropertyChanged(nameof(ExelFilesСontentInDirectoriesEquals));

            ExelFilesСontentInDirectoriesNoEquals.Clear();
            List<string> сolumnsDataNoDplicates = selectedPageInfo["ColumnsDataNoDplicates"].ConvertAll(x => Convert.ToString(x)); ////////////////////////////////["ColumnsDataNoDplicates"]
            var exelFilesСontentInDirectoriesNoEquals = сolumnsDataNoDplicates?.Where(i => !ExelFilesСontentInDirectoriesEquals.Contains(i))?.ToList();
            ExelFilesСontentInDirectoriesNoEquals = new ObservableCollection<string>(exelFilesСontentInDirectoriesNoEquals);
            OnPropertyChanged(nameof(ExelFilesСontentInDirectoriesNoEquals));
        }

        private List<Model.File> SearchFilesInDirectoryies(Directory directory, string nameColumn)//nameColumn не надо хотя без него работает selectedPageInfo["ColumnsDataNoDplicates"] он берет от сюда
        {          
            List<string> searchingData = selectedPageInfo["ColumnsDataNoDplicates"].ConvertAll(x => Convert.ToString(x)).OfType<string>().ToList();
            List<Model.File> EqualsFile = new List<Model.File>();
            directory.SetFilseModifiedName(exelSettings.ExelSettingsSearchFiles.Configurations[1]);
            foreach (string format in exelSettings.ExelSettingsSearchFiles.FormatsSearch)
            {
                EqualsFile.AddRange(directory.SearchFileToName(searchingData, format, exelSettings.ExelSettingsSearchFiles.Configurations));
            }                       
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
    }
}
