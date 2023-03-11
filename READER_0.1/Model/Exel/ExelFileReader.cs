using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using READER_0._1.Tools;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using READER_0._1.Model.Settings.Exel;

namespace READER_0._1.Model.Exel
{
    public class ExelFileReader
    {
        public ExelFile ExelFile { get; private set; }

        private ExelSettingsRead settings;

        private WorkbookPart workbookPart;

        static private string TempFolderPath;

        public bool Closed { get; private set; } = false;

        private Excel.Application usedAppliacation;
        private Excel.Workbook usedWorkbook;
        private Process usedProcess;
        private SpreadsheetDocument usedDocument;

        public ExelFileReader(ExelFile exelFile, string tempFolderPath, ExelSettingsRead settings)
        {
            ExelFile = exelFile;
            this.settings = settings;
            TempFolderPath = tempFolderPath;
        }
        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);       
        static Process GetExcelProcess(Excel.Application exelApplication)
        {
            GetWindowThreadProcessId(exelApplication.Hwnd, out int id);
            return Process.GetProcessById(id);
        }   
        
        public List<ExelFilePage> Read()
        {            
            Excel.Application exelApplication = new Excel.Application();
            usedAppliacation = exelApplication;
            Excel.Workbook exelWorkbook = exelApplication.Workbooks.Open(ExelFile.Path, ReadOnly: true);
            usedWorkbook = exelWorkbook;
            Process applicationProcess = GetExcelProcess(exelApplication);
            usedProcess = applicationProcess;
            if (Closed == true)
            {
                return null;
            }           
            string tempFilePath = CreateTempFile(exelApplication, exelWorkbook);
            exelWorkbook.Close();

            exelWorkbook = exelApplication.Workbooks.Open(tempFilePath);
            List<Excel.Worksheet> exelWorksheets = GetWorksheets(exelWorkbook);
            Dictionary<int, object[,]> DataInPage = GetDataInPages(exelWorksheets);
            exelWorksheets = null;
            exelWorkbook.Close();
            exelApplication.Quit();
            applicationProcess.Kill();
            usedProcess = null;
            Marshal.ReleaseComObject(exelWorkbook);
            usedWorkbook = null;
            Marshal.ReleaseComObject(exelApplication);
            usedAppliacation = null;     
            
            GC.Collect();       
            List<ExelFilePage> exelFilePages = ReadWorksheets(DataInPage);           
            return exelFilePages;
        }       
        public void Close()
        {
            Closed = true;
            try
            {
                if (usedAppliacation != null)
                {
                    usedAppliacation.Quit();
                    Marshal.ReleaseComObject(usedAppliacation);
                }                
            }
            catch (Exception)
            {

            }
            try
            {
                if (usedWorkbook != null)
                {
                    usedWorkbook.Close();
                    Marshal.ReleaseComObject(usedWorkbook);
                }               
            }
            catch (Exception)
            {
               
            }
            try
            {
                if (usedProcess != null)
                {
                    usedProcess.Kill();
                }
               
            }
            catch (Exception)
            {

            }
            try
            {
                if (usedDocument != null)
                {
                    usedDocument.Close();
                    usedDocument.Dispose();
                    Marshal.ReleaseComObject(usedDocument);
                }                
            }
            catch (Exception)
            {
               
            }
            try
            {
                if (ExelFile.TempCopyPath != null)
                {
                    System.IO.File.Delete(ExelFile.TempCopyPath);
                }
            }
            catch (Exception)
            {

            }            
        }
        private string CreateTempFile(Excel.Application exelApplication, Excel.Workbook exelWorkbook)
        {
            GetWindowThreadProcessId(exelApplication.Hwnd, out int idProcess);
            string tempFileName = "id022-" + idProcess + "id022-" + ExelFile.FileName + "-temp" + "." + ExelFile.Format.ToString();
            string tempFilePath = Path.Combine(TempFolderPath, tempFileName);            
            while (System.IO.File.Exists(tempFilePath))
            {
                tempFileName += "1";
                tempFilePath = Path.Combine(TempFolderPath, tempFileName + ExelFile.Format.ToString());
            }
            try
            {
                exelWorkbook.SaveCopyAs(tempFilePath);
                
            }
            catch (Exception)
            {
                System.IO.File.Delete(tempFilePath);
                exelWorkbook.SaveCopyAs(tempFilePath);
            }
            System.IO.File.SetAttributes(tempFilePath, FileAttributes.Hidden);
            ExelFile.SetTempCopyPath(tempFilePath);
            return tempFilePath;
        }

        private List<Excel.Worksheet> GetWorksheets(Excel.Workbook exelWorkbook)
        {
            List<Excel.Worksheet> exelWorksheets = new List<Excel.Worksheet>();
            for (int page = 1; page < exelWorkbook.Sheets.Count + 1; page++)
            {
                exelWorksheets.Add((Excel.Worksheet)exelWorkbook.Sheets[page]);
            }
            return exelWorksheets;
        }

        private Dictionary<int, object[,]> GetDataInPages(List<Excel.Worksheet> exelWorksheets)
        {
            Dictionary<int, object[,]> DataInPage = new Dictionary<int, object[,]>();
            for (int page = 0; page < exelWorksheets.Count; page++)
            {
                DataInPage.TryAdd(exelWorksheets[page].Index, GetDataInPage(exelWorksheets[page]));
                if (settings.MultiWorksheet == false)
                {
                    break;
                }
            }
            return DataInPage;
        }
        private List<ExelFilePage> ReadWorksheets(Dictionary<int, object[,]> DataInPage)
        {
            if (Closed == true)
            {
                return null;
            }
            List<ExelFilePage> exelFilePages = new List<ExelFilePage>();
            SpreadsheetDocument document = SpreadsheetDocument.Open(ExelFile.TempCopyPath, true);            
            usedDocument = document;
            this.workbookPart = document.WorkbookPart;
            List<WorksheetPart> worksheetsParts = workbookPart.WorksheetParts.ToList();
            foreach (int index in DataInPage.Keys)
            {                
                if (DataInPage[index] != null)
                {                    
                    exelFilePages.Add(ReadWorksheetExel(workbookPart.GetPartById("rId" + index.ToString()) as WorksheetPart, DataInPage[index]));
                }

            }
            if (document != null)
            {
                document.Close();
                document.Dispose();
            }
            usedDocument = null;
            return exelFilePages;
        }

        private static string ConvertToLetter(int columnNumber)
        {
            int dividend = columnNumber;
            string columnLetter = string.Empty;

            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                columnLetter = Convert.ToChar('A' + modulo) + columnLetter;
                dividend = (dividend - modulo) / 26;
            }

            return columnLetter;
        }
        private static int ConvertToNumber(string columnName)
        {
            int result = 0;
            for (int i = 0; i < columnName.Length; i++)
            {
                char c = columnName[i];
                result = result * 26 + (c - 'A' + 1);
            }
            return result;
        }       
        private object[,] GetDataInPage(Excel.Worksheet worksheet)
        {
            Excel.Range last = worksheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing);
            string range1 = last.AddressLocal;
            Excel.Range rangeExel = worksheet.get_Range("A1", last);
            string range = rangeExel.AddressLocal;
            object[,] arrData = (object[,])worksheet.Range[range].Value;
            return arrData;
        }

        private ExelFilePage ReadWorksheetExel(WorksheetPart worksheetPart, object[,] arrData)
        {            
            List<Tuple<int, int>> positions = SearchPositionColumnName(arrData, settings.SearchableColumn);            
            string relationshipId = workbookPart.GetIdOfPart(worksheetPart);
            Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Id == relationshipId);
            ExelFilePage exelFilePage = new ExelFilePage(sheet.Name);          
            foreach (Tuple<int, int> position in positions)
            {
                exelFilePage.AddTabel(CreateTable(position, arrData, worksheetPart));                                
            }
            return exelFilePage;
        }        
        private ExelFilePageTable CreateTable(Tuple<int, int> positionNameColumn, object[,] arrData, WorksheetPart worksheetPart)
        {
            Dictionary<string, Tuple<int, int>> titlePositions = ReadTableHeader(positionNameColumn,arrData, worksheetPart);
            int startRow = GetFirstRowInTable(titlePositions, worksheetPart);
            int lastRow  = GetLastRowInTable(new Tuple<int, int>(startRow, titlePositions.Values.First().Item2), arrData, worksheetPart);            
            ExelFilePageTable exelFilePageTable = new ExelFilePageTable(titlePositions.Keys.ToList());
            exelFilePageTable.SetRangeBody(GetRangeBodyTable(positionNameColumn, arrData, startRow, lastRow, worksheetPart));
            foreach (string key in titlePositions.Keys)
            {
                exelFilePageTable.AddColumn(ReadColumn(titlePositions[key],startRow,lastRow, arrData, worksheetPart), key);
            }
            
            return exelFilePageTable;
        }
        private Exel.Range GetRangeBodyTable(Tuple<int, int> positionNameColumn, object[,] arrData, int startRow, int lastRow, WorksheetPart worksheetPart)
        {
            Exel.Range headerRange =  GetTableHeaderRange(positionNameColumn, arrData, worksheetPart);
            Tuple<int, int> firstCell = headerRange.Start;
            Tuple<int, int> lastCell = headerRange.End;
            Exel.Range rangeBody = new Exel.Range(new Tuple<int, int>(startRow, firstCell.Item2), new Tuple<int, int>(lastRow, lastCell.Item2));
            return rangeBody;
        }

        private List<object> ReadColumn(Tuple<int, int> titlePosition,int startRow,int lastRow, object[,] arrData, WorksheetPart worksheetPart)
        {
            Tuple<int, int> ReadCell = titlePosition;
            List<object> DataColumn = new List<object>();
            MergeCells mergeCells = null;
            DocumentFormat.OpenXml.Spreadsheet.MergeCell mergeCell = null;
            Cell cell = null;
            object value;
            if (worksheetPart.Worksheet.Elements<MergeCells>().Count<MergeCells>() > 0)
            {
                mergeCells = worksheetPart.Worksheet.Elements<MergeCells>().First();
            }            
            for (int row = startRow; row <= lastRow; row++)
            {
                ReadCell = new Tuple<int, int>(row, titlePosition.Item2);
                value = arrData[ReadCell.Item1, ReadCell.Item2];
                if (value == null && mergeCells != null)
                {
                    cell = worksheetPart.Worksheet.Descendants<Cell>().FirstOrDefault(c => c.CellReference == ConvertToLetter(ReadCell.Item2) + ReadCell.Item1.ToString());
                    mergeCell = GetCellInMergeCells(cell, mergeCells);
                    if (mergeCell != null)
                    {
                        value = GetVelueMergeCell(GetCellInMergeCells(cell, mergeCells), arrData);
                    }
                    
                }
                if (value != null && value.ToString() == "-2146826246")
                {
                    value = null;
                }
                DataColumn.Add(value);
            }
            return DataColumn;
        }
        private Dictionary<string, Tuple<int, int>> ReadTableHeader(Tuple<int, int> positionNameColumn, object[,] arrData, WorksheetPart worksheetPart)
        {            
            Exel.Range headerRange =  GetTableHeaderRange(positionNameColumn, arrData, worksheetPart);
            Tuple<int, int> firstCell = headerRange.Start;
            Tuple<int, int> lastCell = headerRange.End;
            Dictionary<string, Tuple<int, int>> titlePosition = new Dictionary<string, Tuple<int, int>>();
            for (int i = firstCell.Item2; i < lastCell.Item2 - firstCell.Item2 + 1 + firstCell.Item2; i++)
            {
                positionNameColumn = CheckRealPositionNameColumn(new Tuple<int, int>(firstCell.Item1, i), arrData);
                if (positionNameColumn != null)
                {
                    titlePosition.TryAdd(arrData[positionNameColumn.Item1, positionNameColumn.Item2].ToString(),positionNameColumn);
                }
            }
            return titlePosition;
        }
        private Exel.Range GetTableHeaderRange(Tuple<int, int> positionNameColumn, object[,] arrData, WorksheetPart worksheetPart)
        {
            Tuple<int, int>  firstCell = positionNameColumn;
            Tuple<int, int>  lastCell = positionNameColumn;
            Worksheet worksheet = worksheetPart.Worksheet;
            MergeCells mergeCells = worksheet.Elements<MergeCells>().FirstOrDefault();
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            int rightBorderEnd;
            int leftBorderEnd;
            bool isLastCell = false;
            int rowIndex = positionNameColumn.Item1;
            int startColumn = positionNameColumn.Item2;
            Row row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
            List<Cell> cells = row.Elements<Cell>().ToList();
            string columnName;
            string cellReference;
            Cell cell;
            while (isLastCell == false)
            {
                columnName = ConvertToLetter(startColumn);
                cellReference = columnName + rowIndex.ToString();
                cell = cells.Find(cell => cell.CellReference == cellReference);
                if (cell == null)
                {
                    isLastCell = true;
                    rightBorderEnd = startColumn - 1;
                    lastCell = new Tuple<int, int>(rowIndex, rightBorderEnd);
                }
                startColumn++;
            }
            startColumn = positionNameColumn.Item2;
            isLastCell = false;
            while (isLastCell == false)
            {
                columnName = ConvertToLetter(startColumn);
                cellReference = columnName + rowIndex.ToString();
                cell = cells.Find(cell => cell.CellReference == cellReference);
                if (cell == null)
                {
                    isLastCell = true;
                    leftBorderEnd = startColumn + 1;
                    firstCell = new Tuple<int, int>(rowIndex, leftBorderEnd);
                }
                startColumn--;
            }
            return new Exel.Range(firstCell, lastCell);
        }
        
        private object GetVelueMergeCell(DocumentFormat.OpenXml.Spreadsheet.MergeCell mergeCell, object[,] arrData)
        {
            string[] mergeCellReference = mergeCell.Reference.Value.Split(':');
            (string column, int row) = GetColumnAndRow(mergeCellReference[0]);           
            Tuple<int, int> mergeCellValuePosition = new Tuple<int, int>(row, ConvertToNumber(column));
            //Tuple<int, int> mergeCellValuePosition = new Tuple<int, int>(Int32.Parse(mergeCellReference[0].Substring(1, 1)), ConvertToNumber(column));
            return arrData[mergeCellValuePosition.Item1, mergeCellValuePosition.Item2];
        }
        private static (string column, int row) GetColumnAndRow(string cellReference)
        {
            string column = new string(cellReference.TakeWhile(char.IsLetter).ToArray());
            int row = int.Parse(cellReference.Substring(column.Length));
            return (column, row);
        }

        private DocumentFormat.OpenXml.Spreadsheet.MergeCell GetCellInMergeCells(Cell cell, MergeCells mergeCells)
        {
            foreach (DocumentFormat.OpenXml.Spreadsheet.MergeCell mergeCell in mergeCells.Elements())
            {
                if (IsCellInMergeCell(cell, mergeCell) == true)
                {
                    return mergeCell;
                }
            }
            return null;
        }
        private bool IsCellInMergeCell(Cell cell, DocumentFormat.OpenXml.Spreadsheet.MergeCell mergeCell)
        {
            if (cell == null)
            {
                return false;
            }
            string[] mergeCellReference = mergeCell.Reference.Value.Split(':');
            string[] cellReference = cell.CellReference.Value.Split(':');            
            if (mergeCellReference[0] == cellReference[0] || mergeCellReference[1] == cellReference[0])
            {
                return true;
            }

            return false;
        }               
        private List<Tuple<int, int>> SearchPositionColumnName(object[,] dataInWorksheet, string nameColumnSearch)
        {
            List<string> namesColumnSearch = new List<string>();
            List<Tuple<int, int>> positions = new List<Tuple<int, int>>();
            settings.SearchingColumnName.TryGetValue(nameColumnSearch, out namesColumnSearch);                        
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
                            var tt = namesColumnSearch.Find(item => item == dataInWorksheet[row, column].ToString());
                            var gg = dataInWorksheet[row, column].ToString();
                            positions.Add(new Tuple<int, int>(row, column));
                        }
                    }
                }
            }
            return positions;
        }                          
        private int GetLastRowInTable(Tuple<int, int> startCell, object[,] arrData, WorksheetPart worksheetPart)
        {
            var rows = arrData.GetLength(0);
            Cell cell = worksheetPart.Worksheet.Descendants<Cell>().FirstOrDefault(c => c.CellReference == "A1");
            SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();            
            string columnName;
            string cellReference;
            Border border = new Border();
            for (int i = startCell.Item1; i < rows; i++)
            {                
                if (arrData[startCell.Item1, startCell.Item2] == null)
                {
                    columnName = ConvertToLetter(startCell.Item2);
                    cellReference = columnName + startCell.Item1.ToString();
                    cell = worksheetPart.Worksheet.Descendants<Cell>().FirstOrDefault(c => c.CellReference == cellReference);
                    if (cell != null)
                    {
                        border = GetBorder(cell,worksheetPart);
                        if (border.TopBorder != null && border.BottomBorder == null)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }                                         
                startCell = new Tuple<int, int>(startCell.Item1 + 1, startCell.Item2);
            }
            if (startCell.Item1 - 1 >= 1)
            {
                return startCell.Item1 - 1;
            }
            return 1;
        }

        private Border GetBorder(Cell cell, WorksheetPart worksheetPart)
        {          
            if (cell == null)
            {
                return null;
            }
            Border result = new Border();
            (string column, int row) = GetColumnAndRow(cell.CellReference);
            Cell cellTop = worksheetPart.Worksheet.Descendants<Cell>().FirstOrDefault(c => c.CellReference == column + (row - 1).ToString());
            Cell cellBottom = worksheetPart.Worksheet.Descendants<Cell>().FirstOrDefault(c => c.CellReference == column + (row + 1).ToString());
            Border cellTopBorder = GetImaginaryBorder(cellTop);
            Border cellBorder = GetImaginaryBorder(cell);           
            Border cellBottomBorder = GetImaginaryBorder(cellBottom);
            if (cellTopBorder != null)
            {
                if (cellBorder.TopBorder != null || cellTopBorder.BottomBorder != null)
                {
                    result.TopBorder = new TopBorder();
                }                
            }
            if (cellBottomBorder != null)
            {
                if (cellBorder.BottomBorder != null || cellBottomBorder.TopBorder != null)
                {
                    result.BottomBorder = new BottomBorder();
                }
            }
            return result;
        }
        private Border GetImaginaryBorder(Cell cell)
        {
            if (cell == null || cell.StyleIndex == null)
            {
                return null;
            }        
            Borders borders = workbookPart.WorkbookStylesPart.Stylesheet.Borders;
            CellFormat cellFormat = (CellFormat)workbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ElementAt((int)cell.StyleIndex.Value);
            Border border = borders.ChildElements.GetItem((int)cellFormat.BorderId.Value) as Border;
            Border result = new Border();
            if (border.LeftBorder.HasAttributes == true)
            {
                result.LeftBorder = new LeftBorder();
            }
            if (border.TopBorder.HasAttributes == true)
            {
                result.TopBorder = new TopBorder();
            }
            if (border.RightBorder.HasAttributes == true)
            {
                result.RightBorder = new RightBorder();
            }
            if (border.BottomBorder.HasAttributes == true)
            {
                result.BottomBorder = new BottomBorder();
            }
            return result;
        }

        private int GetFirstRowInTable(Dictionary<string, Tuple<int, int>> titlePosition, WorksheetPart worksheetPart)
        {            
            Worksheet worksheet = worksheetPart.Worksheet;
            MergeCells mergeCells = null;
            if (worksheet.Elements<MergeCells>().Count<MergeCells>() > 0)
            {
                mergeCells = worksheet.Elements<MergeCells>().First();
            }      
            int maxRow = 0;
            int maxRowInMergeCell = 0;
            string columnName;
            string cellReference;
            Cell cell;
            DocumentFormat.OpenXml.Spreadsheet.MergeCell mergeCell;
            foreach (string title in titlePosition.Keys)
            {
                if (mergeCells != null)
                {
                    columnName = ConvertToLetter(titlePosition[title].Item2);
                    cellReference = columnName + titlePosition[title].Item1.ToString();
                    cell = worksheet.Descendants<Cell>().FirstOrDefault(c => c.CellReference == cellReference);                    
                    mergeCell = GetCellInMergeCells(cell, mergeCells);
                    if (mergeCell != null)
                    {
                        maxRowInMergeCell = GetLastRowInMergeCell(mergeCell);
                        if (maxRowInMergeCell > maxRow)
                        {
                            maxRow = maxRowInMergeCell;
                        }
                    }
                }                
                if (titlePosition[title].Item1 > maxRow)
                {
                    maxRow = titlePosition[title].Item1;
                }                
            }
            return maxRow + 1;
        }

        private int GetLastRowInMergeCell(DocumentFormat.OpenXml.Spreadsheet.MergeCell mergeCell)
        {
            string input = mergeCell.Reference;
            string[] parts = input.Split(':');
            string firstCoordinate = Regex.Match(parts[0], @"\d+").Value;
            string secondCoordinate = Regex.Match(parts[1], @"\d+").Value;
            int firstNumber = int.Parse(firstCoordinate);
            int secondNumber = int.Parse(secondCoordinate);
            int maxNumber = Math.Max(firstNumber, secondNumber);
            return maxNumber;
        }               
        private Tuple<int, int> CheckRealPositionNameColumn(Tuple<int, int> nameColumnPosition, object[,] arrData)
        {
            var rows = arrData.GetLength(0);
            //var columns = arrData.GetLength(1);            
            Tuple<int, int> realPosition = nameColumnPosition;
            if (arrData[nameColumnPosition.Item1, nameColumnPosition.Item2] != null && CheckValueCellOfNameColumn(arrData[nameColumnPosition.Item1, nameColumnPosition.Item2].ToString()))
            {
                return realPosition;
            }
            if (nameColumnPosition.Item1 - 1 > 0 && arrData[nameColumnPosition.Item1 - 1, nameColumnPosition.Item2] != null && CheckValueCellOfNameColumn(arrData[nameColumnPosition.Item1 - 1, nameColumnPosition.Item2].ToString()) == true)
            {
                return new Tuple<int, int>(nameColumnPosition.Item1 - 1, nameColumnPosition.Item2);
            }
            if (nameColumnPosition.Item1 + 1 <= rows && arrData[nameColumnPosition.Item1 + 1, nameColumnPosition.Item2] != null && CheckValueCellOfNameColumn(arrData[nameColumnPosition.Item1 + 1, nameColumnPosition.Item2].ToString()) == true)
            {
                return new Tuple<int, int>(nameColumnPosition.Item1 + 1, nameColumnPosition.Item2);
            }
            return null;
        }
        private bool CheckValueCellOfNameColumn(string valueInCell)
        {
            foreach (string key in settings.SearchingColumnName.Keys)
            {
                if (valueInCell == null)
                {
                    break;
                }
                for (int i = 0; i < settings.SearchingColumnName[key].Count; i++)
                {
                    if (settings.SearchingColumnName[key].Find(item => item == valueInCell) != null)
                    {
                        return true;
                    }
                }                
            }
            return false;
        }              
    }
}

