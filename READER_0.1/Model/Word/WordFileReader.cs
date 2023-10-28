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
using READER_0._1.Model.Excel;
using System.Threading;
using static READER_0._1.Model.Settings.Word.SearchParagraph;
using READER_0._1.Model.Excel.TableData;
using READER_0._1.Model.Word.Settings;
using ModelTable = READER_0._1.Model.Excel.TableData;
using MS.WindowsAPICodePack.Internal;
using Task = System.Threading.Tasks.Task;

namespace READER_0._1.Model.Word
{
    public class WordFileReader : IReader<List<ModelTable.Table>>
    {
        public WordFile WordFile { get; private set; }       

        private WordSettingsRead settings;        
     
        public event EventHandler<IReader<List<ModelTable.Table>>.ReadEventArgs> ReadEnd;

        public Task TaskRead { get; private set; }

        public bool Closed { get; private set; }

        private CancellationTokenSource cancelToken;

        static private string TempFolderPath;

        private Application usedApplication;
        private Document usedDocument;
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
            cancelToken = new CancellationTokenSource();
            TaskRead = new Task(() =>
            {
                List<ModelTable.Table> tables = new List<ModelTable.Table>();
                Exception exception = null;
                try
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

                    usedApplication = new Application();
                    usedDocument = usedApplication.Documents.Open(tempFilePath, Visible: false);
                    string text = usedDocument.Content.Text;
                    string[] SearchStringWords = text.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    usedDocument.Close();
                    usedApplication.Quit();
                    foreach (SearchParagraph searchParagraph in settings.SearchParagraphs)
                    {
                        ModelTable.Table table = SearchingParagraph(searchParagraph, SearchStringWords);
                        if (table.Rows.Count > 0)
                        {
                            tables.Add(table);
                        }
                    }
                }
                catch (Exception e)
                {
                    exception = e;
                }
                ReadEnd?.Invoke(this, new IReader<List<ModelTable.Table>>.ReadEventArgs(tables, exception));              
            }, cancelToken.Token);           
            TaskRead.Start();
        }      
        public void Close()
        {
            try
            {
                cancelToken.Cancel();
            }
            catch (Exception)
            {
                
            }
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
        private ModelTable.Table SearchingParagraph(SearchParagraph searchParagraph, string[] searchStringWords)
        {
            Paragraph wordFileParagraph = new Paragraph();
            List<SearchString> searchStringСandidates = new List<SearchString>();
            List<MainSearchString> mainSearchStringСandidates = new List<MainSearchString>();
            ModelTable.Table table = new ModelTable.Table();

            List<int> ignorIndex = new List<int>();
            List<SearchWord> mainSearchWords = new List<SearchWord>();
            List<SearchWord> subSearchWords = new List<SearchWord>();
            List<SearchWord> result = new List<SearchWord>();

            List<(List<int> ignoregIndexs, SearchString searchString)> ignored = new List<(List<int> ignoregIndexs, SearchString searchString)>();
            Dictionary<string, List<SearchWord>> mainSearchStrings = new Dictionary<string, List<SearchWord>>();

            foreach (MainSearchString searchString in searchParagraph.MainSearchStrings)
            {
                mainSearchStrings.Add(searchString?.Name, new List<SearchWord>());
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
                        if (mainSearchStringСandidates[j].ZeroingStrings == true)
                        {
                            foreach (string name in mainSearchStrings.Keys)
                            {
                                mainSearchStrings[name].Clear();
                            }
                        }
                        mainSearchStrings[mainSearchStringСandidates[j].Name] = GetSearchWord(searchStringWords, i, mainSearchStringСandidates[j]);
                    }
                }
                searchStringСandidates = searchParagraph.GetSubSearchStrings(searchStringWords[i]);
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
                        subSearchWords = GetSearchWord(searchStringWords, i, searchStringСandidates[j]);
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
                            table.AddRow(result);
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
            return table;
        }       
        private List<SearchWord> GetSearchWord(string[] SearchStringWords, int positionWord, SearchString searchString)
        {
            List<int> maskSearchWords = searchString.GetRelativeMaskSearchWords(SearchStringWords[positionWord]);
            List<SearchWord> values = new List<SearchWord>();
            for (int i = 0; i < maskSearchWords.Count; i++)
            {
                int realPosition = positionWord + maskSearchWords[i];
                values.Add(new SearchWord(searchString.SearchWords[i].Name, DataType.ToType(SearchStringWords[realPosition], searchString.SearchWords[i].Type), searchString.SearchWords[i].Type));
                DynamicSearchWord dynamicSearchWord = searchString.DynamicSearchWords?.Find(item => item.BaseSearchWord.Name == searchString.SearchWords[i].Name);
                int tempPosition = realPosition;
                while (dynamicSearchWord != null)
                {                    
                    if (dynamicSearchWord.PositionWord == PositionWord.Right)
                    {                       
                        for (int j = (tempPosition + 1); j < dynamicSearchWord.MaxDistance + tempPosition; j++)
                        {
                            if (dynamicSearchWord.Type == DataType.GetType(SearchStringWords[j],out object value))
                            {
                                values.Add(new SearchWord(dynamicSearchWord.Name, value, dynamicSearchWord.Type));
                                tempPosition = j;
                                break;
                            }
                        }
                    }
                    else
                    {                    
                        for (int j = (tempPosition - 1); j > dynamicSearchWord.MaxDistance - tempPosition; j--)
                        {
                            if (dynamicSearchWord.Type == DataType.GetType(SearchStringWords[j], out object value))
                            {
                                values.Add(new SearchWord(dynamicSearchWord.Name, SearchStringWords[j], dynamicSearchWord.Type));
                                tempPosition = j;
                                break;
                            }
                        }
                    }
                    dynamicSearchWord = searchString.DynamicSearchWords?.Find(item => item.BaseSearchWord.Name == dynamicSearchWord.Name);                    
                }
                //сейчас начальное ключевое слово не учитывается из-за особенности работы, если возникнет необходимость его добавить можно сделать это через Additions, или еще как, меня не калит
                DynamicSearchWord dynamicSearchRange = searchString.DynamicSearchRanges.Find(item => item.BaseSearchWord.Name == searchString.SearchWords[i].Name);
                tempPosition = realPosition;
                if (dynamicSearchRange != null)
                {
                    SearchWord newValue = values.Last();
                    newValue.Type = dynamicSearchRange.BaseSearchWord.Type;
                    newValue.Name = dynamicSearchRange.Name;
                    string result = "";
                    if (dynamicSearchRange.PositionWord == PositionWord.Right)
                    {                      
                        for (int j = tempPosition + 1; j < dynamicSearchRange.MaxDistance + tempPosition; j++)
                        {
                            if (dynamicSearchRange.Type == DataType.GetType(SearchStringWords[j], out object value))
                            {
                                break;                            
                            }
                            result = result + " " + value.ToString();
                        }
                    }
                    else
                    {
                        for (int j = tempPosition - 1; j > dynamicSearchRange.MaxDistance - tempPosition; j--)
                        {
                            if (dynamicSearchRange.Type == DataType.GetType(SearchStringWords[j], out object value))
                            {
                                break;
                            }
                            result = result + " " + value.ToString();
                        }
                    }
                    newValue.Data = result;                    
                }               
            }            
            foreach (List<int> items in searchString.AssociationsWords.Values)
            {
                List<SearchWord> result = values.Where((s, i) => items.Contains(i)).Skip(0).ToList();
                SearchWord newValue = SearchString.CollapseSearchWords(result);
                values = values.Where((item, index) => !items.Contains(index)).ToList();
                values.Insert(items[0], newValue);
            }
            foreach ((string name, SearchWord value) addition in searchString.Additions)
            {
                int index = values.FindIndex(item => item.Name == addition.name);
                if (index > 0)
                {
                    values.Insert(index + 1, addition.value);
                }
            }
            foreach ((string nameSearchWord, string value) replacement in searchString.Replacements)
            {
                SearchWord value = values.Find(item => item.Name == replacement.nameSearchWord);
                value.Data = replacement.value;
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
