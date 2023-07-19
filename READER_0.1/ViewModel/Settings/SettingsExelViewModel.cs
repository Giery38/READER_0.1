using READER_0._1.ViewModel.tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static READER_0._1.Model.Exel.Settings.ExelSettingsRead;
using READER_0._1.Model.Exel.Settings;
using static READER_0._1.Model.Exel.Settings.ExelSettingsSearchFiles;
using System.Windows.Input;
using READER_0._1.Command.Settings.SettingsExel;

namespace READER_0._1.ViewModel.Settings
{
    public class SettingsExelViewModel : ViewModelBase
    {
        private readonly ExelSettings settings;
        //commands
        public ICommand AddConfigurationsNameCommand { get; }   
        public ICommand AddSearchingColumnNameCommand { get; }
        public ICommand AddSearchingColumnNameValueCommand { get; }
        public ICommand AddSearchingFormatCommand { get; }
        public ICommand RemoveSearchingColumnNameCommand { get; }
        public ICommand RemoveSelectedSearchingColumnNameValueCommand { get; }
        public ICommand RemoveSearchingFormatCommand { get; }
        public ICommand RemoveConfigurationNameCommand { get; }
        public SettingsExelViewModel(ExelSettings exelSettings)
        {           
            settings = exelSettings;
            //commands
            AddConfigurationsNameCommand = new AddConfigurationsNameCommand(this, settings);
            AddSearchingColumnNameCommand = new AddSearchingColumnNameCommand(this, settings);
            AddSearchingColumnNameValueCommand = new AddSearchingColumnNameValueCommand(this, settings);
            AddSearchingFormatCommand = new AddSearchingFormatCommand(this, settings);
            RemoveSearchingColumnNameCommand = new RemoveSearchingColumnNameCommand(this, settings);
            RemoveSelectedSearchingColumnNameValueCommand = new RemoveSelectedSearchingColumnNameValueCommand(this, settings);
            RemoveSearchingFormatCommand = new RemoveSearchingFormatCommand(this, settings);
            RemoveConfigurationNameCommand = new RemoveConfigurationNameCommand(this, settings);
            //obs
            SearchingColumnNames = new ObservableCollection<SearchingColumnName>(settings.ExelSettingsRead.SearchingColumnNames);
            SelectedSearchingColumnNameValues = new ObservableCollection<StringContainer>();
            SearchingFormats = new ObservableCollection<StringContainer>(settings.ExelSettingsSearchFiles.FormatsSearch.Select(item => new StringContainer(item, settings.ExelSettingsSearchFiles.FormatsSearch)));
            ConfigurationNames = new ObservableCollection<ConfigurationName>(settings.ExelSettingsSearchFiles.Configurations);
        }
        private ObservableCollection<SearchingColumnName> searchingColumnNames;
        public ObservableCollection<SearchingColumnName> SearchingColumnNames
        {
            get
            {
                return searchingColumnNames;                   
            }
            set
            {
                searchingColumnNames = value;
                OnPropertyChanged(nameof(SearchingColumnNames));
            }
        }
        private SearchingColumnName selectedSearchingColumnName;
        public SearchingColumnName SelectedSearchingColumnName
        {
            get
            {
                return selectedSearchingColumnName;
            }
            set
            {                
                selectedSearchingColumnName = value;
                OnPropertyChanged(nameof(SelectedSearchingColumnName));
                if (SelectedSearchingColumnName != null && SelectedSearchingColumnName.Values != null)
                {
                    SelectedSearchingColumnNameValues = new ObservableCollection<StringContainer>(SelectedSearchingColumnName.Values.Select(item => new StringContainer(item, SelectedSearchingColumnName.Values)));                   
                }
            }
        }
        private ObservableCollection<StringContainer> selectedSearchingColumnNameValues;
        public ObservableCollection<StringContainer> SelectedSearchingColumnNameValues
        {
            get 
            {                                              
                return selectedSearchingColumnNameValues;
            }  
            set
            {
                selectedSearchingColumnNameValues = value;
                OnPropertyChanged(nameof(SelectedSearchingColumnNameValues));
            }
        }
        private ObservableCollection<StringContainer> searchingFormats;
        public ObservableCollection<StringContainer> SearchingFormats
        {
            get
            {               
                return searchingFormats;
            }            
            set
            {
                searchingFormats = value;
                OnPropertyChanged(nameof(SearchingFormats));
            }            
        }
        private ObservableCollection<ConfigurationName> configurationNames;
        public ObservableCollection<ConfigurationName> ConfigurationNames
        {
            get
            {
                return configurationNames;
            }
            set
            {
                configurationNames = value;
                OnPropertyChanged(nameof(ConfigurationNames));
            }
        }
        private string inputConfigurationNameText;
        public string InputConfigurationNameText
        {
            get
            {
                return inputConfigurationNameText;
            }
            set
            {
                inputConfigurationNameText = value;
                OnPropertyChanged(nameof(InputConfigurationNameText));
                InputConfigurationsNameFragmented?.Clear();
                var tt = value.ToList().Select(item => item.ToString()).ToList();
                InputConfigurationsNameFragmented = new ObservableCollection<BindingTuple>();
                foreach (var item in tt)
                {
                    InputConfigurationsNameFragmented.Add(new BindingTuple(item, false));
                }
            }
        }
        private ObservableCollection<BindingTuple> inputConfigurationsNameFragmented;
        public ObservableCollection<BindingTuple> InputConfigurationsNameFragmented
        {
            get
            {
                return inputConfigurationsNameFragmented;
            }
            set
            {
                inputConfigurationsNameFragmented = value;
                OnPropertyChanged(nameof(InputConfigurationsNameFragmented));
            }
        }       
    }
}
