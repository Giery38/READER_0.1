using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using READER_0._1.Model;
using READER_0._1.Model.Exel;
using READER_0._1.Model.Word;
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
                wordViewModel.ReadWordFile(wordViewModel.Files[index], wordViewModel.SelectedSettings.WordSettingsRead);
                wordViewModel.Files[index].ReadEnd += ConvertWordFileToExcelCommand_ReadEnd;
            }                              
        }

        private void ConvertWordFileToExcelCommand_ReadEnd(object sender, EventArgs e)
        {           
            if (sender is WordFile)
            {
                WordFile wordFile = sender as WordFile;
                wordFile.ReadEnd -= ConvertWordFileToExcelCommand_ReadEnd;
                wordViewModel.CreateExcelFile(wordFile);
            }           
        }
    }
}
