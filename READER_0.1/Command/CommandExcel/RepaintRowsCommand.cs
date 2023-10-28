using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using READER_0._1.Model.Excel;
using READER_0._1.Model.Excel.TableData;
using READER_0._1.Tools;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace READER_0._1.Command.CommandExcel
{
    public class RepaintRowsCommand : CommandBase
    {
        private readonly ExcelViewModel excelViewModel;
        public RepaintRowsCommand(ExcelViewModel excelViewModel)
        {
            this.excelViewModel = excelViewModel;
            excelViewModel.PropertyChanged += ExcelViewModel_PropertyChanged;
        }
        private void ExcelViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(excelViewModel.ExcelFilesСontentInDirectoriesNoEquals))
            {
                OnCanExecutedChanged();
            }
        }
        public override bool CanExecute(object parameter)
        {
            if (excelViewModel.ExcelFilesСontentInDirectoriesNoEquals.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override void Execute(object parameter)
        {           
            string format = excelViewModel.SelectedExcelFile.Format;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = $"Excel Files (*{format})|*{format}";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.FileName = excelViewModel.SelectedExcelFile.Name + format;            
            if (saveFileDialog.ShowDialog() == true)
            {
                Color selectedColor = excelViewModel.SelecedColor;
                excelViewModel.AddRecentColor(selectedColor);
                string colorHex = selectedColor.ToString().Substring(1);
                ExcelFileWriter excelFileWriter = new ExcelFileWriter(excelViewModel.SelectedExcelFile);
                excelFileWriter.Open();                
                List<Table> tables = excelViewModel.CreateTableEquals(excelViewModel.ExcelFilesСontentInDirectoriesNoEquals.Cast<object>().ToList());
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                for (int table = 0; table < tables.Count; table++)
                {         
                    excelFileWriter.RepaintTable(excelViewModel.SelectedPage.Tables[table], tables[table].Rows, excelViewModel.SelectedExcelFile.ExcelPages.IndexOf(excelViewModel.SelectedPage), colorHex);
                }
                excelFileWriter.Close();
                stopwatch.Stop();
                string filePath = saveFileDialog.FileName;
                System.IO.File.Copy(excelViewModel.SelectedExcelFile.TempCopyPath, filePath, true);
                System.IO.File.SetAttributes(filePath, FileAttributes.Normal);
            }
        }
    }
}
