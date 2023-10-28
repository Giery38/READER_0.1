using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace READER_0._1.Model.Excel.Settings
{
    [Serializable]
    public class ExcelSettingsSearchFiles
    {
        private List<string> formatsSearch;
        public List<string> FormatsSearch
        {
            get
            {
                return formatsSearch;
            }
            set
            {
                formatsSearch = value;
            }
        }
        private List<ConfigurationName> configurations;
        public List<ConfigurationName> Configurations
        {
            get
            {
                return configurations;
            }
            set
            {
                configurations = value;
            }
        }
        public ExcelSettingsSearchFiles()
        {
            Configurations = new List<ConfigurationName>();
            FormatsSearch = new List<string>();
        }

        public class ConfigurationName
        {
            private string name;
            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }
            private List<(int position, char symbol)> modifieds;
            public List<(int position, char symbol)> Modifieds
            {
                get
                {
                    return modifieds;
                }
                set
                {
                    modifieds = value;
                }
            }
            public ConfigurationName()
            {
                Modifieds = new List<(int position, char symbol)>();
            }
            public string SetOrRemoveConfiguration(string input)
            {
                if (input == null)
                {
                    return null;
                }
                string result = new string(input);
                if (CheckModifieds(input) == true)
                {
                    for (int i = Modifieds.Count - 1; i >= 0; i--)
                    {
                        result = result.Remove(Modifieds[i].position, 1);
                    }
                }
                else
                {
                    for (int i = 0; i < Modifieds.Count; i++)
                    {
                        result = result.Insert(Modifieds[i].position, Modifieds[i].symbol.ToString());
                    }
                }
                return result;
            }
            public bool CheckModifieds(string input)
            {
                for (int i = 0; i < Modifieds.Count; i++)
                {
                    if (Modifieds[i].position < 0 || Modifieds[i].position >= input.Length || input[Modifieds[i].position] != Modifieds[i].symbol)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
