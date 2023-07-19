using READER_0._1.Model.Settings.Word;
//using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using READER_0._1.Tools;
using System.IO;
using Microsoft.Office.Interop.Word;
using READER_0._1.Model.Exel;
using System.Threading;
using static READER_0._1.Model.Settings.Word.SearchParagraph;

namespace READER_0._1.Model.Word
{
    public class WordFileReader
    {
        public WordFile WordFile { get; private set; }

        public bool Closed { get; private set; }

        private WordSettingsRead settings;

        static private string TempFolderPath;

#pragma warning disable CS0649 // Полю "WordFileReader.usedApplication" нигде не присваивается значение, поэтому оно всегда будет иметь значение по умолчанию null.
        private Application usedApplication;
#pragma warning restore CS0649 // Полю "WordFileReader.usedApplication" нигде не присваивается значение, поэтому оно всегда будет иметь значение по умолчанию null.
#pragma warning disable CS0649 // Полю "WordFileReader.usedDocument" нигде не присваивается значение, поэтому оно всегда будет иметь значение по умолчанию null.
        private Document usedDocument;
#pragma warning restore CS0649 // Полю "WordFileReader.usedDocument" нигде не присваивается значение, поэтому оно всегда будет иметь значение по умолчанию null.
        public WordFileReader(WordFile wordFile, string tempFolderPath, WordSettingsRead settings)
        {
            WordFile = wordFile;
            TempFolderPath = tempFolderPath;
            this.settings = settings;
        }
        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);
        /*
        static Process GetWordProcess(Window window)
        {
            GetWindowThreadProcessId(window.Hwnd, out int id);
            return Process.GetProcessById(id);
        }
        */
        public List<ExelFilePageTable> Read()
        {           
            string tempName = "fas21" + WordFile.Name + "-temp" + "." + WordFile.Format.ToString();
            string tempFilePath = Path.Combine(TempFolderPath, tempName);           
            while (System.IO.File.Exists(tempFilePath))
            {
                tempName += "1";
                tempFilePath = Path.Combine(TempFolderPath, tempName + WordFile.Format.ToString());
            }
            System.IO.File.Copy(WordFile.Path, tempFilePath, true);
            System.IO.File.SetAttributes(tempFilePath, FileAttributes.Hidden);
            WordFile.SetTempCopyPath(tempFilePath);
            
            Application wordApp = new Application();
            Document wordDoc = wordApp.Documents.Open(tempFilePath, Visible: false);
            string text = wordDoc.Content.Text;
            string[] SearchStringWords = text.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            wordDoc.Close();
            wordApp.Quit();
            List<ExelFilePageTable> exelFilePageTables = new List<ExelFilePageTable>();
            
            foreach (SearchParagraph searchParagraph in settings.SearchParagraphs)
            {
                exelFilePageTables.Add(SearchingParagraph(searchParagraph, SearchStringWords));
            }
            return exelFilePageTables;
        }
        public void Close()
        {
            try
            {
                usedDocument?.Close();
            }
            catch (Exception)
            {

            }
            try
            {
                usedApplication?.Quit();
            }
            catch (Exception)
            {

            }
            Closed = true;
        }
        private ExelFilePageTable SearchingParagraph(SearchParagraph searchParagraph, string[] searchStringWords)
        {
            WordFileParagraph wordFileParagraph = new WordFileParagraph();
            List<SearchString> searchStringСandidates = new List<SearchString>();
            List<SearchString> mainSearchStringСandidates = new List<SearchString>();
            ExelFilePageTable exelFilePageTable = new ExelFilePageTable();

            List<int> ignorIndex = new List<int>();
            List<SearchWord> mainSearchWords = new List<SearchWord>();
            List<SearchWord> subSearchWords = new List<SearchWord>();
            List<SearchWord> result = new List<SearchWord>();

            List<(List<int> ignoregIndexs, SearchString searchString)> ignored = new List<(List<int> ignoregIndexs, SearchString searchString)>();
            Dictionary<string, List<SearchWord>> mainSearchStrings = new Dictionary<string, List<SearchWord>>();

            foreach (SearchString searchString in searchParagraph.MainSearchStrings)
            {
                mainSearchStrings.Add(searchString.Name, new List<SearchWord>());
            }
            for (int i = 0; i < searchStringWords.Length; i++)
            {
                mainSearchStringСandidates = searchParagraph.GetMainStrings(searchStringWords[i]);
                for (int j = 0; j < mainSearchStringСandidates.Count; j++)
                {
                    if (ignored.Find(item => item.searchString == mainSearchStringСandidates[j]).searchString == null
                        && IsSearchString(searchStringWords, i, mainSearchStringСandidates[j], out (List<int> ignoregIndexs, SearchString searchString) ignor) == true)
                    {
                        ignored.Add(ignor);
                        mainSearchStrings[mainSearchStringСandidates[j].Name] = GetSearchWord(searchStringWords, i, mainSearchStringСandidates[j]);
                    }
                }
                searchStringСandidates = searchParagraph.GetSearchStrings(searchStringWords[i]);
                for (int p = 0; p < searchStringСandidates.Count; p++)
                {
                    if (searchStringСandidates[p].Active == false)
                    {
                        searchStringСandidates.Remove(searchStringСandidates[p]);
                    }
                }               
                for (int j = 0; j < searchStringСandidates.Count; j++)
                {                                                        
                    if (ignored.Find(item => item.searchString == mainSearchStringСandidates[j]).searchString == null &&
                        IsSearchString(searchStringWords, i, searchStringСandidates[j], out (List<int> ignoregIndexs, SearchString searchString) ignor) == true)
                    {
                        ignored.Add(ignor);
                        result.Clear();
                        subSearchWords = GetSearchWord(searchStringWords, i, searchStringСandidates[j], searchParagraph.GetTypeSearchString(searchStringСandidates[j]));
                        if (subSearchWords != null)
                        {
                            result.AddRange(subSearchWords);
                        }
                        foreach (string key in mainSearchStrings.Keys)
                        {
                            if (mainSearchStrings[key].Count > 0)
                            {
                                result.AddRange(mainSearchStrings[key]);
                            }
                        }
                        if (result.Count > 0)
                        {
                            exelFilePageTable.AddRow(result);
                        }
                    }
                }
                for (int j = 0; j < ignored.Count; j++)
                {
                    if (ignored[j].ignoregIndexs.Last() <= i)
                    {
                        ignored.Remove(ignored[j]);
                    }
                }
            }
            return exelFilePageTable;
        }
        private List<SearchWord> GetSearchWord(string[] SearchStringWords, int positionWord, SearchString searchString, TypeSearchStrings typeSearchStrings)//добавить тип к которому относиться стринг и получить его конфигурацию
        {            
            List<int> maskSearchWords = searchString.GetRelativeMaskSearchWords(SearchStringWords[positionWord]);
            List<SearchWord> values = new List<SearchWord>();
            for (int i = 0; i < maskSearchWords.Count; i++)
            {
                if (typeSearchStrings.Replacement.nameSearchWord == searchString.SearchWords[i].Name)
                {
                    values.Add(new SearchWord(searchString.SearchWords[i].Name, typeSearchStrings.Replacement.value));
                }
                else
                {
                    values.Add(new SearchWord(searchString.SearchWords[i].Name, SearchStringWords[positionWord + maskSearchWords[i]]));
                }                
            }
            if (searchString.AssociationsWords.Count > 0)
            {
                foreach (var item in searchString.AssociationsWords.Values)
                {
                    List<SearchWord> result = values.Where((s, i) => item.Contains(i)).Skip(1).ToList();
                    values[item.First()].CollapseSearchWord(result);
                    values = values.Where((s, i) => i == item.First() || !item.Contains(i)).ToList();
                }               
            }
            return values;
        }
        private List<SearchWord> GetSearchWord(string[] SearchStringWords, int positionWord, SearchString searchString)
        {
            List<int> maskSearchWords = searchString.GetRelativeMaskSearchWords(SearchStringWords[positionWord]);
            List<SearchWord> values = new List<SearchWord>();
            for (int i = 0; i < maskSearchWords.Count; i++)
            {
                values.Add(new SearchWord(searchString.SearchWords[i].Name, SearchStringWords[positionWord + maskSearchWords[i]]));
            }
            if (searchString.AssociationsWords.Count > 0)
            {
                foreach (var item in searchString.AssociationsWords.Values)
                {
                    List<SearchWord> result = values.Where((s, i) => item.Contains(i)).Skip(1).ToList();
                    values[item.First()].CollapseSearchWord(result);
                    values = values.Where((s, i) => i == item.First() || !item.Contains(i)).ToList();
                }
            }
            return values;
        }
        private bool IsSearchString(string[] SearchStringWords,int positionWord, SearchString searchString, out (List<int> ignoregIndexs, SearchString searchString) ignor)
        {
            List<int> maskKeyWords = searchString.GetRelativeMaskKeyWords(SearchStringWords[positionWord]);
            List<string> values = new List<string>();
            ignor = (new List<int>(), null);
            for (int i = 0; i < maskKeyWords.Count; i++)
            {
                values.Add(SearchStringWords[positionWord + maskKeyWords[i]]);
                ignor.ignoregIndexs.Add(positionWord + maskKeyWords[i]);
            }
            if (values.SequenceEqual(searchString.KeyWords))
            {
                ignor.searchString = searchString;
                return true;
            }
            ignor.ignoregIndexs.Clear();
            return false;
        }
    }
}
