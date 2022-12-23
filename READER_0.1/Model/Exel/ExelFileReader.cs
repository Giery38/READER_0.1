using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions; 
using System.Windows;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using static READER_0._1.Model.Settings.ExelSettingsRead;
using System.Threading;
using System.Threading.Tasks;
using System.Printing;
using System.Reflection;
using System.Reflection.Emit;
using READER_0._1.Tools;

namespace READER_0._1.Model.Exel
{
    class ExelFileReader
    {
        public ExelFile ExelFile { get; private set; }

        private Settings.ExelSettingsRead Settings;

        private List<ExelFilePageTable> exelFilePageTableReadedThread;
        public ExelFileReader(ExelFile exelFile)
        {
            ExelFile = exelFile;
            Settings = new Settings.ExelSettingsRead();
            exelFilePageTableReadedThread = new List<ExelFilePageTable>();
        }
        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);

        static Process GetExcelProcess(Excel.Application excelApp)
        {
            GetWindowThreadProcessId(excelApp.Hwnd, out int id);
            return Process.GetProcessById(id);
        }
        public List<ExelFilePage> ReadWorksheetsExel(bool MultiWorksheet)
        {
            Excel.Application xlApp = new Excel.Application();
            Process appProcess = GetExcelProcess(xlApp);
            Excel.Workbook xlWb = xlApp.Workbooks.Open(ExelFile.Path);
            List<Excel.Worksheet> xlShts = new List<Excel.Worksheet>();
            List<ExelFilePage> exelFilePages = new List<ExelFilePage>();
            List<List<object>> text = new List<List<object>>();
            //Dictionary<Excel.Worksheet, List<List<object>>> textInWorksheet = new Dictionary<Excel.Worksheet, List<List<object>>>();
            for (int page = 1; page < xlWb.Sheets.Count + 1; page++)
            {
                xlShts.Add((Excel.Worksheet)xlWb.Sheets[page]);
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int page = 0; page < xlShts.Count; page++)
            {
                exelFilePages.Add(new ExelFilePage(xlShts[page]));                
                ReadWorksheetExel(xlShts[page]);
                exelFilePages[page].AddTabel(exelFilePageTableReadedThread);
                if (MultiWorksheet == false)
                {
                    break;
                }
            }
            stopwatch.Stop();          
            xlWb.Close();
            xlApp.Quit();
            appProcess.Kill();
            GC.Collect();
            return exelFilePages;
        }
        private void ReadWorksheetExel(Excel.Worksheet worksheet)
        {
            exelFilePageTableReadedThread.Clear();
            Excel.Worksheet xlSht = worksheet;
            var tt = worksheet.Name;
            //Excel.Range cells = (Excel.Range)xlSht.Cells[xlSht.Columns.Rows.Count, 1];                   
            Excel.Range last = xlSht.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing);
            string range1 = last.AddressLocal;
            Excel.Range rangeExel = xlSht.get_Range("A1", last);
            string range = rangeExel.AddressLocal;
            object[,] arrData = (object[,])xlSht.Range[range].Value;
            if (arrData == null)
            {
                return ;
            }
            Excel.ListObjects smartTables = xlSht.ListObjects;            
            List<Tuple<int,int>> positions = SearchPositionColumnName(arrData, "Номера отправки");
            positions = RemovePositionInTablesRange(rangeExel, smartTables, positions);
            List<ExelFilePageTable> exelFilePageTable = new List<ExelFilePageTable>();
            List<Thread> threadsReadTable = new List<Thread>();
            int i = 0;
            foreach (Tuple<int,int> position in positions)
            {                
                Thread readTable = new Thread(() => CreateTable(position, rangeExel, arrData, worksheet));
                threadsReadTable.Add(readTable);
                while (ThreadHelper.SerchThreadLive(threadsReadTable).Count >= 7)
                {
                    Thread.Sleep(500);
                }
                threadsReadTable[i].Name = "Чтение таблици " + i;
                threadsReadTable[i].Start();
                i++;
            }
            while (threadsReadTable.Find(item => item.IsAlive == true) != null)
            {

            }
            threadsReadTable.Clear();
        }       
        private int SerchCountThreadLive(List<Thread> threads)
        {
            int count = 0;
            for (int i = 0; i < threads.Count; i++)
            {
                if (threads[i].IsAlive == true)
                {
                    count++;
                }               
            }
            return count;
        }
        private void CreateTable(Tuple<int, int> nameColumnPosition, Excel.Range range, object[,] arrData, Excel.Worksheet worksheet)
        {
            Dictionary<string, Tuple<int, int>> TitlePosition = TableHeaderRead(nameColumnPosition, range);
            Tuple<int, int> firstRowInTable = GetFirstRowInTable(nameColumnPosition, range);
            Tuple<int, int> lirstRowInTable = GetLastRowInTable(firstRowInTable, range);
            if (TitlePosition != null)
            {
                ExelFilePageTable exelFilePageTable = new ExelFilePageTable(TitlePosition.Keys.ToList());
                foreach (string key in TitlePosition.Keys)
                {
                    Tuple<int, int> firstPosition = new Tuple<int, int>(firstRowInTable.Item1, TitlePosition[key].Item2);
                    Tuple<int, int> lastPosition = new Tuple<int, int>(lirstRowInTable.Item1, TitlePosition[key].Item2);
                    List<MergeCell> mergeCellsTemp = new List<MergeCell>();
                    List<object> columnData = ReadColumn(arrData, firstPosition, lastPosition, range, out mergeCellsTemp);                    
                    exelFilePageTable.AddMergeCells(mergeCellsTemp);
                    exelFilePageTable.AddColumn(columnData, key);
                }
                exelFilePageTableReadedThread.Add(exelFilePageTable);
            }
        }

        private List<Tuple<int, int>> RemovePositionInTablesRange(Excel.Range range, Excel.ListObjects tables, List<Tuple<int, int>> positions)
        {
            List<Tuple<int, int>> positionsOutsideInRange = new List<Tuple<int, int>>();
            for (int i = 1; i < tables.Count + 1; i++)
            {
                for (int j = 0; j < positions.Count; j++)
                {
                    bool positionInRAnge  = CheckingCellInRange(range, tables[i].Range, positions[j]);
                    if (positionInRAnge == false)
                    {
                        positionsOutsideInRange.Add(positions[j]);
                    }
                }
            }
            if (positionsOutsideInRange.Count == 0 )
            {
                return positions;
            }
            else
            {
                return positionsOutsideInRange;
            }            
        }

        private bool CheckingCellInRange(Excel.Range range, Excel.Range rangeTable, Tuple<int,int> cellPosition)
        {
            Excel.Range cellPositionInRange = (Excel.Range)range[cellPosition.Item1, cellPosition.Item2];
            Excel.Range resultIntersect = range.Application.Intersect(rangeTable, cellPositionInRange);
            if (resultIntersect != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private List<Tuple<int, int>> SearchPositionColumnName(object[,] dataInWorksheet, string nameColumnSearch)
        {
            List<string> namesColumnSearch = new List<string>();
            List<Tuple<int, int>> positions = new List<Tuple<int, int>>();
            Settings.SearchingColumnName.TryGetValue(nameColumnSearch, out namesColumnSearch);                        
            var columnCount = dataInWorksheet.GetLength(1);
            var rowCount = dataInWorksheet.Length / columnCount;
            for (int column = 1; column < columnCount + 1; column++)
            {
                for (int row = 1; row < rowCount - 1; row++)
                {
                    if (dataInWorksheet[row, column] != null)
                    {
                        if (namesColumnSearch.Find(item => item == dataInWorksheet[row, column].ToString()) != null)
                        {
                            positions.Add(new Tuple<int, int>(row, column));
                        }
                    }
                }
            }
            return positions;
        }
        
        private List<object> ReadColumn(object[,] arrData, Tuple<int, int> firstRowInTable, Tuple<int, int> lirstRowInTable, Excel.Range range, out List<MergeCell> mergeCells)
        {
            List<object> DataInColumn = new List<object>();
            mergeCells = new List<MergeCell>();
            object mergeCellValue = new object();
            Tuple<int, int> startPosition = new Tuple<int, int>(0,0);
            int size = 1;
            for (int i = firstRowInTable.Item1; i <= firstRowInTable.Item1 + (lirstRowInTable.Item1 - firstRowInTable.Item1); i++)
            {
                Excel.Range cell = (Excel.Range)range[i, lirstRowInTable.Item2];
                if (arrData[i, lirstRowInTable.Item2] != null && (bool)cell.MergeCells == true)
                {
                    startPosition = new Tuple<int,int>(i, lirstRowInTable.Item2);
                    mergeCellValue = arrData[i, lirstRowInTable.Item2];
                }
                if (size > 1 && (bool)cell.MergeCells == false)
                {                    
                    mergeCells.Add(new MergeCell(startPosition,size));                    
                    size = 0;
                    DataInColumn.Add(mergeCellValue);
                }
                if (arrData[i, lirstRowInTable.Item2] == null && (bool)cell.MergeCells == true)
                {
                    size++;
                }
                else
                {
                    DataInColumn.Add(arrData[i, lirstRowInTable.Item2]);
                }                
            }
            return DataInColumn;
         }
        private List<object> ReadColumn(object[,] arrData, Tuple<int, int> firstRowInTable, Tuple<int, int> lirstRowInTable, Excel.Worksheet worksheet)
        {
            List<object> DataInColumn = new List<object>();
            for (int i = firstRowInTable.Item1; i <= firstRowInTable.Item1 + (lirstRowInTable.Item1 - firstRowInTable.Item1); i++)
            {                
                DataInColumn.Add(arrData[i, lirstRowInTable.Item2]);
            }
            return DataInColumn;
        }
        private Tuple<int, int> GetLastRowInTable(Tuple<int, int> nameColumnPosition, Excel.Range range) // 1.27
        {
            Tuple<int, int> StartCell = new Tuple<int, int>(nameColumnPosition.Item1 + 1, nameColumnPosition.Item2);
            Excel.Range StartCellInRange = (Excel.Range)range[StartCell.Item1, StartCell.Item2];        
            for (int i = 0; i < range.Rows.Count; i++)
            {
                if ((Excel.XlLineStyle)StartCellInRange.Borders.get_Item(Excel.XlBordersIndex.xlEdgeBottom).LineStyle != Excel.XlLineStyle.xlContinuous &&
                    (Excel.XlLineStyle)StartCellInRange.Borders.get_Item(Excel.XlBordersIndex.xlEdgeTop).LineStyle == Excel.XlLineStyle.xlContinuous)
                {
                    return new Tuple<int, int>(StartCell.Item1 - 1, StartCell.Item2);
                }
                StartCell = new Tuple<int, int>(StartCell.Item1 + 1, StartCell.Item2);
                StartCellInRange = (Excel.Range)range[StartCell.Item1, StartCell.Item2];                   
            }
            return StartCell;
        }
        private Tuple<int,int> GetFirstRowInTable(Tuple<int, int> nameColumnPosition, Excel.Range range) // 0.02
        {
            Tuple<int, int> FirstRowInTable = new Tuple<int, int>(nameColumnPosition.Item1 + 1, nameColumnPosition.Item2);
            Excel.Range nameColumnPositionInRange = (Excel.Range)range[FirstRowInTable.Item1, FirstRowInTable.Item2];
            for (int i = 0; i < range.Rows.Count; i++)
            {
                if ((bool)nameColumnPositionInRange.MergeCells == true)
                {                    
                    FirstRowInTable = new Tuple<int, int>(FirstRowInTable.Item1 + i, FirstRowInTable.Item2);
                    nameColumnPositionInRange = (Excel.Range)range[FirstRowInTable.Item1, FirstRowInTable.Item2];
                }
                else
                {
                    break;
                }
            }
            return FirstRowInTable;
        }
        private Dictionary<string, Tuple<int, int>> TableHeaderRead(Tuple<int, int> nameColumnPosition, Excel.Range range) //0.04
        {
            Tuple<int, int> firstCell = new Tuple<int, int>(0, 0);
            Tuple<int, int> lastCell = new Tuple<int, int>(0, 0);
            Dictionary<string, Tuple<int, int>> nameAndPosition = new Dictionary<string, Tuple<int, int>>();
            List<string> TableNames = new List<string>();
            List<Tuple<int, int>> TableNamesPosition = new List<Tuple<int, int>>();
            int startRow = firstCell.Item1;
            TableHeaderRange(nameColumnPosition, range, out firstCell, out lastCell);
            Tuple<int, int> positionName = new Tuple<int, int>(0,0);
            Tuple<int, int> positionNameTrue = null;
            string name = null;
            if (firstCell.Item1 == lastCell.Item1 && firstCell.Item2 == lastCell.Item2)
            {
                CheckHeaderOnNameCell(firstCell, range, out name, out positionNameTrue);
                if (name != null && positionNameTrue != null)
                {
                    nameAndPosition.TryAdd(name, positionNameTrue);
                }
            }
            for (int column = firstCell.Item2; column <= firstCell.Item2 + lastCell.Item2 - firstCell.Item2; column++)
            {
                positionName = new Tuple<int,int>(firstCell.Item1, column);
                positionNameTrue = null;
                name = null;
                CheckHeaderOnNameCell(positionName, range, out name, out positionNameTrue);
                if (name != null && positionNameTrue != null)
                {
                    nameAndPosition.TryAdd(name, positionNameTrue);
                }
            }
            if (nameAndPosition.Count > 0)
            {
                return nameAndPosition;
            }
            else
            {
                return null;
            }
        }
        private void CheckHeaderOnNameCell(Tuple<int, int> nameColumnPosition, Excel.Range range, out string HeaderName,out Tuple<int,int> nameColumnPositionTrue)
        {
            nameColumnPositionTrue = null;
            HeaderName = "error";
            int row = nameColumnPosition.Item1;
            string valueInCell = null;          
            for (int i = row - 1; i < row + 3; i++) 
            {
                if (i <= 0)
                {
                    i = 1;
                }
                Excel.Range cellInRange = (Excel.Range)range[i, nameColumnPosition.Item2];
                if (cellInRange != null)
                {
                    if (cellInRange.Value != null)
                    {
                        valueInCell = (string)cellInRange.Value.ToString();
                    }
                    string nameColumn = CheckValueCellOfNameColumn(valueInCell);
                    if (nameColumn != "error")
                    {
                        HeaderName = nameColumn;
                        nameColumnPositionTrue = new Tuple<int, int>(i, nameColumnPosition.Item2);
                        break;
                    }
                }                                
            }
        }
        private string CheckValueCellOfNameColumn(string valueInCell)
        {
            foreach (string key in Settings.SearchingColumnName.Keys)
            {
                if (valueInCell == null)
                {
                    break;
                }
                for (int i = 0; i < Settings.SearchingColumnName[key].Count; i++)
                {
                    if (Settings.SearchingColumnName[key].Find(item => item == valueInCell) != null)
                    {
                        return key;
                    }
                }                
            }
            return "error";
        }

        private void TableHeaderRange(Tuple<int, int> nameColumnPosition, Excel.Range range, out Tuple<int, int> firstCell, out Tuple<int, int> lastCell)//0.0091
        {
            firstCell = nameColumnPosition;
            while (ItsLastHeaderCell(range, firstCell) == false)
            {
                firstCell = new Tuple<int, int>(firstCell.Item1, firstCell.Item2 - 1);
            }
            firstCell = new Tuple<int, int>(firstCell.Item1, firstCell.Item2 + 1);
            lastCell = nameColumnPosition;
            while (ItsLastHeaderCell(range, lastCell) == false)
            {
                lastCell = new Tuple<int, int>(lastCell.Item1, lastCell.Item2 + 1);
            }
            lastCell = new Tuple<int, int>(lastCell.Item1, lastCell.Item2 - 1);
        }
        private bool ItsLastHeaderCell(Excel.Range range, Tuple<int, int> position)
        {
            if (position.Item2 == 0)
            {
                return true;
            }
            Excel.Range exelPositin = (Excel.Range)range[position.Item1, position.Item2];
            if ((exelPositin.Value == null && (bool)exelPositin.MergeCells == false) || position.Item2 == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

