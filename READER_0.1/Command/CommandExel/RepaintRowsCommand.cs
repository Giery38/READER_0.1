using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using READER_0._1.Model.Exel;
using READER_0._1.Tools;
using READER_0._1.ViewModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace READER_0._1.Command.CommandExel
{
    class RepaintRowsCommand : CommandBase
    {
        private readonly ExelViewModel exelViewModel;
        public RepaintRowsCommand(ExelViewModel exelViewModel)
        {
            this.exelViewModel = exelViewModel;
            exelViewModel.PropertyChanged += ExelViewModel_PropertyChanged;
        }
        private void ExelViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(exelViewModel.ExelFilesСontentInDirectoriesNoEquals))
            {
                OnCanExecutedChanged();
            }
        }
        public override bool CanExecute(object parameter)
        {
            if (exelViewModel.ExelFilesСontentInDirectoriesNoEquals.Count > 0)
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
            string format = exelViewModel.SelectedExelFile.Format;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = $"Excel Files (*{format})|*{format}";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.FileName = exelViewModel.SelectedExelFile.Name + format;            
            if (saveFileDialog.ShowDialog() == true)
            {
                Color selectedColor = (Color)parameter;
                exelViewModel.AddRecentColor(selectedColor);
                string colorHex = selectedColor.ToString().Substring(1);
                ExelFileWriter exelFileWriter = new ExelFileWriter(exelViewModel.SelectedExelFile);
                exelFileWriter.Open();
                List<ExelFilePageTable> exelFilePageTables = exelViewModel.CreateTableEquals(exelViewModel.ExelFilesСontentInDirectoriesNoEquals.Cast<object>().ToList());
                for (int table = 0; table < exelViewModel.SelectedPage.Tabeles.Count; table++)
                {
                    for (int row = 0; row < exelFilePageTables[table].Rows.Count; row++)
                    {
                        (int row, int column) startCell =
                            (exelFilePageTables[table].Rows[row].Number + exelViewModel.SelectedPage.Tabeles[table].RangeBody.Start.row, exelViewModel.SelectedPage.Tabeles[table].RangeBody.Start.column);
                        (int row, int column) lastCell =
                            (exelFilePageTables[table].Rows[row].Number + exelViewModel.SelectedPage.Tabeles[table].RangeBody.Start.row, exelViewModel.SelectedPage.Tabeles[table].RangeBody.End.column);
                        exelFileWriter.RepaintRange(startCell, lastCell, colorHex);
                    }
                }
                exelFileWriter.Close();

                string filePath = saveFileDialog.FileName;
                System.IO.File.Copy(exelViewModel.SelectedExelFile.TempCopyPath, filePath, true);
                System.IO.File.SetAttributes(filePath, FileAttributes.Normal);
            }
        }
    }
}
