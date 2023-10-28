using READER_0._1.Tools;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace READER_0._1.Model.Excel.Settings
{
    [Serializable]
    public class ExcelSettingsRead
    {
        private bool multiWorksheet;
        public bool MultiWorksheet
        {
            get
            {
                return multiWorksheet;
            }
            set
            {
                multiWorksheet = value;
            }
        }
        private string searchableColumn;
        public string SearchableColumn
        {
            get
            {
                return searchableColumn;
            }
            set
            {
                searchableColumn = value;
            }
        }      
        private List<SearchingColumnName> searchingColumnNames;
        public List<SearchingColumnName> SearchingColumnNames
        {
            get
            {
                return searchingColumnNames;
            }
            set
            {
                searchingColumnNames = value;
            }
        }
        private int footerLength;
        public int FooterLength
        {
            get
            {
                return footerLength;
            }
            set
            {
                footerLength = value;
            }
        }

        public ExcelSettingsRead()
        {
            SearchingColumnNames =  new List<SearchingColumnName>();
            MultiWorksheet = true;
            FooterLength = 1;
        }
        public ExcelSettingsRead(ExcelSettingsRead excelSettingsRead)
        {
            SearchingColumnNames = excelSettingsRead.SearchingColumnNames.Select(item => new SearchingColumnName
            {
                Name = item.Name,
                Active = item.Active,
                Values = item.Values.Select(item => new string(item)).ToList()
            }).ToList();
            SearchableColumn = new string(excelSettingsRead.SearchableColumn);
            MultiWorksheet = excelSettingsRead.MultiWorksheet;
            FooterLength = excelSettingsRead.FooterLength;
        }

        public class SearchingColumnName
        {
            public string Name { get; set; }
            public List<string> Values { get; set; }
            public bool Active { get; set; }
            public SearchingColumnName()
            {

            }
        }
    }
}
