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

namespace READER_0._1.Model.Word
{
    public class WordFileReader
    {
        public WordFile WordFile { get; private set; }

        private WordSettingsRead settings;

        static private string TempFolderPath;
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
        public void Read()
        {           
            string tempFileName = "fas21" + WordFile.FileName + "-temp" + "." + WordFile.Format.ToString();
            string tempFilePath = Path.Combine(TempFolderPath, tempFileName);           
            while (System.IO.File.Exists(tempFilePath))
            {
                tempFileName += "1";
                tempFilePath = Path.Combine(TempFolderPath, tempFileName + WordFile.Format.ToString());
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
        }
        private ExelFilePageTable SearchingParagraph(SearchParagraph searchParagraph, string[] searchStringWords)
        {
            WordFileParagraph wordFileParagraph = new WordFileParagraph();
            List<SearchString> searchStrings = new List<SearchString>();
            ExelFilePageTable exelFilePageTable = new ExelFilePageTable();
            List<int> ignorIndex = new List<int>();
            List<SearchWord> mainSearchWords = new List<SearchWord>();
            List<SearchWord> subSearchWords = new List<SearchWord>();
            List<SearchWord> result = new List<SearchWord>();
            for (int i = 0; i < searchStringWords.Length; i++)
            {
                if (ignorIndex.Contains(i) == true)
                {
                    continue;
                }
                if (searchParagraph.MainSearchString.KeyWords.Contains(searchStringWords[i]) == true)
                { 
                    if (IsSearchString(searchStringWords, i, searchParagraph.MainSearchString, out ignorIndex) == true)
                    {
                        mainSearchWords = GetSearchWord(searchStringWords, i, searchParagraph.MainSearchString);
                    }
                }
                searchStrings = searchParagraph.GetSearchStrings(searchStringWords[i]);
                if (searchStrings.Count > 0)
                {
                    for (int j = 0; j < searchStrings.Count; j++)
                    {
                        if (IsSearchString(searchStringWords, i, searchStrings[j], out ignorIndex) == true)
                        {
                            result.Clear();
                            subSearchWords = GetSearchWord(searchStringWords, i, searchStrings[j]);
                            if (subSearchWords != null)
                            {
                                result.AddRange(subSearchWords);
                            }
                            if (mainSearchWords != null)
                            {
                                result.AddRange(mainSearchWords);
                            }
                            if (result.Count > 0)
                            {
                                exelFilePageTable.AddRow(result);
                            }                            
                        }
                    }                    
                }
            }
            return exelFilePageTable;
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
        private bool IsSearchString(string[] SearchStringWords,int positionWord, SearchString searchString, out List<int> ignor)
        {
            List<int> maskKeyWords = searchString.GetRelativeMaskKeyWords(SearchStringWords[positionWord]);
            List<string> values = new List<string>();
            ignor = new List<int>();
            for (int i = 0; i < maskKeyWords.Count; i++)
            {
                values.Add(SearchStringWords[positionWord + maskKeyWords[i]]);
                ignor.Add(positionWord + maskKeyWords[i]);
            }
            if (values.SequenceEqual(searchString.KeyWords))
            {
                return true;
            }
            ignor.Clear();
            return false;
        }
    }
}
