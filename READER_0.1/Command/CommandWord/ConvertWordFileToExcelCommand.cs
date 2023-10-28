using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using READER_0._1.Model;
using READER_0._1.Model.Excel;
using READER_0._1.Model.Settings.Word;
using READER_0._1.Model.Word;
using READER_0._1.Model.Word.Settings;
using READER_0._1.Tools;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace READER_0._1.Command.CommandWord
{
    public class ConvertWordFileToExcelCommand : CommandBase
    {
        private readonly WindowFileBase windowFileBase;
        private readonly WordViewModel wordViewModel;
        public ConvertWordFileToExcelCommand(WordViewModel wordViewModel, WindowFileBase windowFileBase) // добавляет все файлы, переопрелить для каждого окна
        {
            this.windowFileBase = windowFileBase;
            this.wordViewModel = wordViewModel;
        }
        public override void Execute(object parameter)
        {
            if (wordViewModel.SelectedWordFile != null)
            {
                int index = wordViewModel.Files.IndexOf(wordViewModel.SelectedWordFile);
                SearchParagraph[] searchParagraphs = (SearchParagraph[])wordViewModel.SearchParagraphs.Clone();
                if (searchParagraphs.Length == 0)
                {
                    return;
                }
                SearchParagraph searchParagraphFirst = (SearchParagraph)searchParagraphs[0];
                for (int i = 1; i < searchParagraphs.Length; i++)
                {
                    for (int main = 0; main < searchParagraphs[i].MainSearchStrings.Count; main++)
                    {
                        if (searchParagraphs[i].MainSearchStrings[main].Active == true && searchParagraphFirst.MainSearchStrings.Find(item => item.Name == searchParagraphs[i].MainSearchStrings[main].Name) == null)
                        {
                            searchParagraphFirst.MainSearchStrings.Add(searchParagraphs[i].MainSearchStrings[main]);
                        }
                    }
                    for (int sub = 0; sub < searchParagraphs[i].SubSearchStrings.Count; sub++)
                    {
                        if (searchParagraphs[i].SubSearchStrings[sub].Active == true && searchParagraphFirst.SubSearchStrings.Find(item => item.Name == searchParagraphs[i].SubSearchStrings[sub].Name) == null)
                        {
                            searchParagraphFirst.SubSearchStrings.Add(searchParagraphs[i].SubSearchStrings[sub]);
                        }
                    }
                }
                wordViewModel.ReadWordFile(wordViewModel.Files[index], new WordSettingsRead() { SearchParagraphs = new List<SearchParagraph>() { searchParagraphFirst } });
               // wordViewModel.Files[index].ReadEnd += ConvertWordFileToExcelCommand_ReadEnd;
            }                              
        }
        /*
        private void ConvertWordFileToExcelCommand_ReadEnd(object sender, EventArgs e)
        {           
            if (sender is WordFile)
            {
                WordFile wordFile = sender as WordFile;
                wordFile.ReadEnd -= ConvertWordFileToExcelCommand_ReadEnd;
                wordViewModel.CreateExcelFile(wordFile);
            }           
        }
        */
    }
}
