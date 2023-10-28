using ExcelInterop = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using System.Text.RegularExpressions;
using System.IO;
using READER_0._1.Model.Excel.Settings;
using static READER_0._1.Model.Excel.Settings.ExcelSettingsRead;
using DocumentFormat.OpenXml.Drawing.Charts;
using READER_0._1.Model.Excel.TableData;
using DevExpress.Data.Extensions;
using OpenXml = DocumentFormat.OpenXml.Spreadsheet;
using ModelTable = READER_0._1.Model.Excel.TableData;
using System.Threading;

namespace READER_0._1.Model.Excel
{
    public class ExcelFileReader
    {
        public ExcelFile ExcelFile { get; private set; }

        private ExcelSettingsRead settings;

        private WorkbookPart workbookPart;

        static private string TempFolderPath;

        private Cell[] currentCells;

        public bool Closed { get; private set; } = false;

        private ExcelInterop.Application usedApplication;
        private ExcelInterop.Workbook usedWorkbook;
        private Process usedProcess;
        private SpreadsheetDocument usedDocument;
        public bool isUsedApplicationNull;

        public ExcelFileReader(ExcelFile excelFile, string tempFolderPath, ExcelSettingsRead settings)
        {
            ExcelFile = excelFile;
            this.settings = settings;
            TempFolderPath = tempFolderPath;
        }
        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);       
        static Process GetExcelProcess(ExcelInterop.Application excelApplication)
        {
            GetWindowThreadProcessId(excelApplication.Hwnd, out int id);
            return Process.GetProcessById(id);
        }   
        
        public List<Page> Read()
        {
            ExcelInterop.Application excelApplication = new ExcelInterop.Application();
            usedApplication = excelApplication;
            ExcelInterop.Workbook excelWorkbook = excelApplication.Workbooks.Open(ExcelFile.Path, ReadOnly: true);
            usedWorkbook = excelWorkbook;
            Process applicationProcess = GetExcelProcess(excelApplication);
            usedProcess = applicationProcess;
            if (Closed == true)
            {
                return null;
            }           
            string tempFilePath = CreateTempFile(excelApplication, excelWorkbook);
            excelWorkbook.Close();

            excelWorkbook = excelApplication.Workbooks.Open(tempFilePath);
            List<ExcelInterop.Worksheet> excelWorksheets = GetWorksheets(excelWorkbook);
            Dictionary<int, object[,]> DataInPage = GetDataInPages(excelWorksheets);
            excelWorksheets = null;
            excelWorkbook.Close();
            excelApplication.Quit();
            applicationProcess.Kill();
            usedProcess = null;
            Marshal.ReleaseComObject(excelWorkbook);
            usedWorkbook = null;
            Marshal.ReleaseComObject(excelApplication);
            usedApplication = null;
         
            List<Page> pages = ReadWorksheets(DataInPage);           
            return pages;
        }       
        public void Close()
        {            
            try
            {
                if (usedApplication != null)
                {
                    usedApplication.Quit();
                    Marshal.ReleaseComObject(usedApplication);
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
                    usedDocument.Dispose();
                    Marshal.ReleaseComObject(usedDocument);
                }                
            }
            catch (Exception)
            {
               
            }
            Closed = true;
        }
        private string CreateTempFile(ExcelInterop.Application excelApplication, ExcelInterop.Workbook excelWorkbook)
        {
            GetWindowThreadProcessId(excelApplication.Hwnd, out int idProcess);
            string tempName = "id022-" + idProcess + "id022-" + ExcelFile.Name + "-temp" + "." + ExcelFile.Format.ToString();
            string tempFilePath = Path.Combine(TempFolderPath, tempName);            
            while (System.IO.File.Exists(tempFilePath))
            {
                tempName += "1";
                tempFilePath = Path.Combine(TempFolderPath, tempName + ExcelFile.Format.ToString());
            }
            try
            {
                excelWorkbook.SaveCopyAs(tempFilePath);
                
            }
            catch (Exception)
            {
                System.IO.File.Delete(tempFilePath);
                excelWorkbook.SaveCopyAs(tempFilePath);
            }
            System.IO.File.SetAttributes(tempFilePath, FileAttributes.Hidden);
            ExcelFile.SetTempCopyPath(tempFilePath);
            return tempFilePath;
        }

        private List<ExcelInterop.Worksheet> GetWorksheets(ExcelInterop.Workbook excelWorkbook)
        {
            List<ExcelInterop.Worksheet> excelWorksheets = new List<ExcelInterop.Worksheet>();
            for (int page = 1; page < excelWorkbook.Sheets.Count + 1; page++)
            {
                excelWorksheets.Add((ExcelInterop.Worksheet)excelWorkbook.Sheets[page]);
            }
            return excelWorksheets;
        }

        private Dictionary<int, object[,]> GetDataInPages(List<ExcelInterop.Worksheet> excelWorksheets)
        {
            Dictionary<int, object[,]> DataInPage = new Dictionary<int, object[,]>();
            for (int page = 0; page < excelWorksheets.Count; page++)
            {
                DataInPage.TryAdd(excelWorksheets[page].Index, GetDataInPage(excelWorksheets[page]));
                if (settings.MultiWorksheet == false)
                {
                    break;
                }
            }
            return DataInPage;
        }
        private List<Page> ReadWorksheets(Dictionary<int, object[,]> DataInPage)
        {
            if (Closed == true)
            {
                return null;
            }
            List<Page> pages = new List<Page>();
            usedDocument = SpreadsheetDocument.Open(ExcelFile.TempCopyPath, false);            
            this.workbookPart = usedDocument.WorkbookPart;
            
            foreach (int index in DataInPage.Keys)
            {                
                if (DataInPage[index] != null)
                {
                    WorksheetPart worksheetPart = workbookPart.GetPartById("rId" + index.ToString()) as WorksheetPart;
                    currentCells = worksheetPart.Worksheet.Descendants<Cell>().ToArray();
                    pages.Add(ReadWorksheetExcel(worksheetPart, DataInPage[index]));
                }

            }
            if (usedDocument != null)
            {
                usedDocument.Dispose();
            }
            usedDocument = null;
            return pages;
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
        private object[,] GetDataInPage(ExcelInterop.Worksheet worksheet)
        {
            ExcelInterop.Range last = worksheet.Cells.SpecialCells(ExcelInterop.XlCellType.xlCellTypeLastCell, Type.Missing);
            string range1 = last.AddressLocal;
            ExcelInterop.Range rangeExcel = worksheet.get_Range("A1", last);
            string range = rangeExcel.AddressLocal;
            object[,] arrData = (object[,])worksheet.Range[range].Value;
            return arrData;
        }

        private Page ReadWorksheetExcel(WorksheetPart worksheetPart, object[,] arrData)
        {            
            List<(int row, int column)> positions = SearchPositionColumnName(arrData, settings.SearchableColumn);            
            string relationshipId = workbookPart.GetIdOfPart(worksheetPart);
            Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Id == relationshipId);
            Page page = new Page(sheet.Name);          
            foreach ((int row, int column) position in positions)
            {
                ModelTable.Table table = CreateTable(position, arrData, worksheetPart);          
                page.AddTable(table);                                
            }
            return page;
        }      
        private ModelTable.Table CreateTable((int row, int column) positionNameColumn, object[,] arrData, WorksheetPart worksheetPart)
        {
            Dictionary<string, (int row, int column)> titlePositions = ReadTableHeader(positionNameColumn,arrData, worksheetPart);
            int startRow = GetFirstRowInTable(titlePositions, worksheetPart);
            int lastRow  = GetLastRowInTable(new Tuple<int, int>(startRow, titlePositions.Values.First().Item2), arrData, worksheetPart);            
            ModelTable.Table table = new ModelTable.Table(titlePositions.Keys.ToList());
            table.SetRangeBody(GetRangeBodyTable(positionNameColumn, arrData, startRow, lastRow, worksheetPart));
            foreach (string key in titlePositions.Keys)
            {
                int counter = 0;
                table.AddDataToColumn(ReadColumn(titlePositions[key],startRow,lastRow, arrData, worksheetPart, ref counter), key);
            }           
            return table;
        }
        private ModelTable.Range GetRangeBodyTable((int row, int column) positionNameColumn, object[,] arrData, int startRow, int lastRow, WorksheetPart worksheetPart)
        {
            ModelTable.Range headerRange =  GetTableHeaderRange(positionNameColumn, arrData, worksheetPart);
            (int row, int column) firstCell = headerRange.Start;
            (int row, int column) lastCell = headerRange.End;
            ModelTable.Range rangeBody = new ModelTable.Range(new (startRow, firstCell.Item2), new (lastRow, lastCell.Item2));
            return rangeBody;
        }
        private List<object> ReadColumn((int row, int column) titlePosition,int startRow,int lastRow, object[,] arrData, WorksheetPart worksheetPart, ref int counter)// добавить предыдущую клетку
        {
            (int row, int column) ReadCell = titlePosition;
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
                ReadCell = new (row, titlePosition.Item2);
                value = arrData[ReadCell.Item1, ReadCell.Item2];
                counter++;
                if (value == null && mergeCells != null)
                {                                      
                    for (int i = counter; i < currentCells.Count(); i++)
                    {
                        if (currentCells[i].CellReference == ConvertToLetter(ReadCell.Item2) + ReadCell.Item1.ToString())
                        {
                            cell = currentCells[i];
                        }
                    }
                    mergeCell = GetCellInMergeCells(cell, mergeCells);
                    if (mergeCell != null)
                    {
                        value = GetValueMergeCell(GetCellInMergeCells(cell, mergeCells), arrData);
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
        private Dictionary<string, (int row, int column)> ReadTableHeader((int row, int column) positionNameColumn, object[,] arrData, WorksheetPart worksheetPart)
        {
            ModelTable.Range headerRange =  GetTableHeaderRange(positionNameColumn, arrData, worksheetPart);
            (int row, int column) firstCell = headerRange.Start;
            (int row, int column) lastCell = headerRange.End;
            Dictionary<string, (int row, int column)> titlePosition = new Dictionary<string, (int row, int column)>();
            for (int i = firstCell.Item2; i < lastCell.Item2 - firstCell.Item2 + 1 + firstCell.Item2; i++)
            {
                positionNameColumn = CheckRealPositionNameColumn(new (firstCell.row, i), arrData);
                if (positionNameColumn.row > 0 && positionNameColumn.column > 0)
                {
                    titlePosition.TryAdd(arrData[positionNameColumn.Item1, positionNameColumn.Item2].ToString(), positionNameColumn);//
                }               
            }
            return titlePosition;
        }
        private ModelTable.Range GetTableHeaderRange((int row, int column) positionNameColumn, object[,] arrData, WorksheetPart worksheetPart)
        {
            (int row, int column) firstCell = positionNameColumn;
            (int row, int column) lastCell = positionNameColumn;
            Worksheet worksheet = worksheetPart.Worksheet;
            MergeCells mergeCells = worksheet.Elements<MergeCells>().FirstOrDefault();
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            int rightBorderEnd;
            int leftBorderEnd;
            bool isLastCell = false;
            int rowIndex = positionNameColumn.Item1;
            int startColumn = positionNameColumn.Item2;
            OpenXml.Row row = sheetData.Elements<OpenXml.Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
            int maxRow = positionNameColumn.row;
            int minRow = positionNameColumn.row;
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
                    lastCell = new(rowIndex, rightBorderEnd);
                }
                else if (mergeCells != null)
                {
                    DocumentFormat.OpenXml.Spreadsheet.MergeCell mergeCell = GetCellInMergeCells(cell, mergeCells);
                    if (mergeCell != null)
                    {
                        (int minRow, int maxRow) rowsNumber = GetRowsInMergeCell(mergeCell);
                        if (rowsNumber.maxRow > maxRow)
                        {
                            maxRow = rowsNumber.maxRow;
                        }
                        if (rowsNumber.minRow < minRow)
                        {
                            minRow = rowsNumber.minRow;
                        }
                    }                   
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
                    firstCell = new (rowIndex, leftBorderEnd);
                }
                else if(mergeCells != null)
                {
                    DocumentFormat.OpenXml.Spreadsheet.MergeCell mergeCell = GetCellInMergeCells(cell, mergeCells);
                    if (mergeCell != null)
                    {
                        (int minRow, int maxRow) rowsNumber = GetRowsInMergeCell(mergeCell);
                        if (rowsNumber.maxRow > maxRow)
                        {
                            maxRow = rowsNumber.maxRow;
                        }
                        if (rowsNumber.minRow < minRow)
                        {
                            minRow = rowsNumber.minRow;
                        }
                    }
                }
                startColumn--;
            }
            firstCell.row = minRow;
            lastCell.row = maxRow;
            return new ModelTable.Range(firstCell, lastCell);
        }
        
        private object GetValueMergeCell(DocumentFormat.OpenXml.Spreadsheet.MergeCell mergeCell, object[,] arrData)
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
        private List<(int row, int column)> SearchPositionColumnName(object[,] dataInWorksheet, string nameColumnSearch)
        {           
            List<(int row, int column)> positions = new List<(int row, int column)>();
            List<string>  namesColumnSearch = settings.SearchingColumnNames.Find(item => item.Name == nameColumnSearch).Values;
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
                            positions.Add(new (row, column));
                        }
                    }
                }
            }
            return positions;
        }                                 

        private Border GetBorder(Cell cell, WorksheetPart worksheetPart) 
        {          
            if (cell == null)
            {
                return null;
            }           
            Border result = new Border();
            (string column, int row) = GetColumnAndRow(cell.CellReference);
            int cellPosition = currentCells.FindIndex(c => c.CellReference == column + row.ToString());
            Cell cellTop = null;
            for (int i = cellPosition; i > 0; i--)
            {
                if (currentCells[i].CellReference == column + (row - 1).ToString())
                {
                    cellTop = currentCells[i];
                    break;
                }
            }
            Cell cellBottom = null;
            for (int i = 0; i < currentCells.Length; i++)
            {
                if (currentCells[i].CellReference == column + (row + 1).ToString())
                {
                    cellBottom = currentCells[i];
                    break;
                }

            }
            Border cellTopBorder = GetImaginaryBorder(cellTop);
            Border cellBorder = GetImaginaryBorder(cell);           
            Border cellBottomBorder = GetImaginaryBorder(cellBottom);
            if (cellBorder.TopBorder != null)
            {
                result.TopBorder = new TopBorder();
            }
            else if(cellTopBorder != null)
            {
                if (cellBorder.TopBorder != null || cellTopBorder.BottomBorder != null)
                {
                    result.TopBorder = new TopBorder();
                }
            }
            if (cellBorder.BottomBorder != null)
            {
                result.BottomBorder = new BottomBorder();
            } 
            else if(cellBottomBorder != null)
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
        private int GetLastRowInTable(Tuple<int, int> startCell, object[,] arrData, WorksheetPart worksheetPart)// добавить предыдущую клетку 
        {
            var rows = arrData.GetLength(0);
            Cell cell = currentCells[0];
            SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
            string columnName;
            string cellReference;
            Border border = new Border();
            int counter = 0;
            for (int i = startCell.Item1; i <= rows; i++)
            {
                counter++;
                if (arrData[startCell.Item1, startCell.Item2] == null)
                {
                    columnName = ConvertToLetter(startCell.Item2);
                    cellReference = columnName + startCell.Item1.ToString();
                    cell = currentCells.FirstOrDefault(c => c.CellReference == cellReference);
                    if (cell != null)
                    {
                        border = GetBorder(cell, worksheetPart);
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
                return startCell.Item1 - 1 - settings.FooterLength;
            }
            return 1;
        }
        private int GetFirstRowInTable(Dictionary<string, (int row, int column)> titlePosition, WorksheetPart worksheetPart)
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
                    columnName = ConvertToLetter(titlePosition[title].column);
                    cellReference = columnName + titlePosition[title].row.ToString();
                    cell = currentCells.FirstOrDefault(c => c.CellReference == cellReference);                    
                    mergeCell = GetCellInMergeCells(cell, mergeCells);
                    if (mergeCell != null)
                    {
                        maxRowInMergeCell = GetRowsInMergeCell(mergeCell).maxRow;
                        if (maxRowInMergeCell > maxRow)
                        {
                            maxRow = maxRowInMergeCell;
                        }
                    }
                }                
                if (titlePosition[title].row > maxRow)
                {
                    maxRow = titlePosition[title].row;
                }                
            }
            return maxRow + 1;
        }
        private (string firstCell , string lastCell) GetReferencesMergeCell(DocumentFormat.OpenXml.Spreadsheet.MergeCell mergeCell)
        {
            (string firstCell, string lastCell) references;
            string input = mergeCell.Reference;
            string[] parts = input.Split(':');
            references.firstCell = parts[0];
            references.lastCell = parts[1];
            return references;
        }
        private (int minRow, int maxRow) GetRowsInMergeCell(DocumentFormat.OpenXml.Spreadsheet.MergeCell mergeCell)
        {
            (int minRow, int maxRow) result;
            (string firstCell, string lastCell) references = GetReferencesMergeCell(mergeCell);
            string firstCoordinate = Regex.Match(references.firstCell, @"\d+").Value;
            string secondCoordinate = Regex.Match(references.lastCell, @"\d+").Value;
            int firstNumber = int.Parse(firstCoordinate);
            int secondNumber = int.Parse(secondCoordinate);
            int maxNumber = Math.Max(firstNumber, secondNumber);
            if (maxNumber == firstNumber)
            {
                result = (secondNumber, maxNumber);
            }
            else
            {
                result = (firstNumber, maxNumber);
            }
            return result;
        }               
        private (int row, int column) CheckRealPositionNameColumn((int row, int column) nameColumnPosition, object[,] arrData)
        {
            var rows = arrData.GetLength(0);
            //var columns = arrData.GetLength(1);            
            (int row, int column) realPosition = nameColumnPosition;
            if (arrData[nameColumnPosition.Item1, nameColumnPosition.Item2] != null && CheckValueCellOfNameColumn(arrData[nameColumnPosition.Item1, nameColumnPosition.Item2].ToString()))
            {
                return realPosition;
            }
            if (nameColumnPosition.row - 1 > 0 && arrData[nameColumnPosition.Item1 - 1, nameColumnPosition.column] != null && CheckValueCellOfNameColumn(arrData[nameColumnPosition.row - 1, nameColumnPosition.Item2].ToString()) == true)
            {
                return new (nameColumnPosition.row - 1, nameColumnPosition.column);
            }
            if (nameColumnPosition.Item1 + 1 <= rows && arrData[nameColumnPosition.row + 1, nameColumnPosition.column] != null && CheckValueCellOfNameColumn(arrData[nameColumnPosition.row + 1, nameColumnPosition.Item2].ToString()) == true)
            {
                return new (nameColumnPosition.row + 1, nameColumnPosition.column);
            }
            return (-1, -1);
        }
        private bool CheckValueCellOfNameColumn(string valueInCell)
        {
            foreach (SearchingColumnName searchingColumnName in settings.SearchingColumnNames)
            {
                if (valueInCell == null)
                {
                    break;
                }
                if (searchingColumnName.Active == true && searchingColumnName.Values.Contains(valueInCell) == true)
                {
                    return true;
                }                                              
            }
            return false;
        }
        public static (string letters, int number) SplitString(string input)
        {
            Regex regex = new Regex(@"^(?<letters>[A-Za-z]+)(?<number>\d+)$");

            Match match = regex.Match(input);

            if (match.Success)
            {
                string letters = match.Groups["letters"].Value;
                int number = int.Parse(match.Groups["number"].Value);

                return (letters, number);
            }
            return new(null, 0);
        }
    }
}

