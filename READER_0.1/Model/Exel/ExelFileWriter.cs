using DevExpress.Data.Async;
using DevExpress.Data.Linq.Helpers;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using READER_0._1.Model.Excel.TableData;
using READER_0._1.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OpenXml = DocumentFormat.OpenXml.Spreadsheet;
using ModelTable = READER_0._1.Model.Excel.TableData;
using Range = READER_0._1.Model.Excel.TableData.Range;

namespace READER_0._1.Model.Excel
{
    public class ExcelFileWriter
    {
        public ExcelFile ExcelFile { get; private set; }
        private bool active;
        public bool Active
        {
            get
            {
                return active;
            }
            set
            {                
                if (value == true && active == true)
                {
                    throw new Exception("Класс не предназначен для работы в многопоточном режиме");
                }
                active = value;
            }
        }
        private SpreadsheetDocument usedDocument;

        public ExcelFileWriter(ExcelFile excelFile)
        {
            ExcelFile = excelFile;

        }
        public void Open()
        {
            usedDocument = SpreadsheetDocument.Open(ExcelFile.TempCopyPath, true);
            active = false;
        }
        public void Close()
        {
            currentWorksheetPart = null;
            usedDocument.Save();
            usedDocument.Dispose();
        }         
        private WorksheetPart currentWorksheetPart;        
        public void TablesWrite(ModelTable.Table table,int pageNumber, string startCell)// сделать сохдание страницы если ее нет 
        {                
            Active = true;
            currentWorksheetPart = usedDocument.WorkbookPart.WorksheetParts.ToList()[pageNumber];
            (string letters, int number) coordinatesExcelSplit = SplitString(startCell);
            string startColumn = new string(coordinatesExcelSplit.letters);
            int startColumnNumber = ConvertToNumber(startColumn);
            int startRow = coordinatesExcelSplit.number;
            string currentCell = new string(startCell);            
            foreach (string key in table.TableColumns.Keys)
            {
                Cell cellTemp = GetCell(currentCell);
                cellTemp.CellValue = new CellValue(key);                
                cellTemp.StyleIndex = 10;
                coordinatesExcelSplit = SplitString(currentCell);
                string newLetters = ConvertToLetter(ConvertToNumber(coordinatesExcelSplit.letters) + 1);
                currentCell = newLetters + coordinatesExcelSplit.number;
            }
            coordinatesExcelSplit = SplitString(startCell);
            currentCell = coordinatesExcelSplit.letters + (coordinatesExcelSplit.number + 1);                  
            for (int row = 0; row < table.Rows.Count; row++)
            {                
                for (int cell = 0; cell < table.Rows[row].RowData.Count; cell++)
                {
                    Cell cellTemp = GetCell(currentCell);
                    object value = table.Rows[row].RowData[cell]?.Data;                    
                    cellTemp.CellValue = CreateCellValue(value, out CellValues cellValuesType, out Type valueType); //сделать нули 
                    if (valueType == typeof(double) || valueType == typeof(float))
                    {
                        cellTemp.StyleIndex = 11;
                    }
                    else
                    {                        
                        cellTemp.StyleIndex = 8;
                    }
                    
                    cellTemp.DataType = new EnumValue<CellValues>(cellValuesType);
                    coordinatesExcelSplit = SplitString(currentCell);
                    string newLetters = ConvertToLetter(ConvertToNumber(coordinatesExcelSplit.letters) + 1);            
                    currentCell = newLetters + coordinatesExcelSplit.number;
                }
                coordinatesExcelSplit = SplitString(currentCell);
                int newNumber = coordinatesExcelSplit.number + 1;
                currentCell = startColumn + newNumber;
            }
            table.SetRangeBody(new Range((startRow + 1, startColumnNumber), 
                (coordinatesExcelSplit.number, startColumnNumber + table.TableColumns.Count)));           
            SheetData sheetData = currentWorksheetPart.Worksheet.Elements<SheetData>().First(); ////            
            DocumentFormat.OpenXml.Spreadsheet.Columns columns = currentWorksheetPart.Worksheet.Elements<DocumentFormat.OpenXml.Spreadsheet.Columns>().First();
            foreach (DocumentFormat.OpenXml.Spreadsheet.Column column in AutoSize(sheetData))
            {
                columns.AppendChild(column);
            }            
            CreateFooter(table, new string[] { "Стоимость", "НДС" }, "Итого:");
            Active = false;
        }
        private void CreateFooter(ModelTable.Table table, string[] columnNamesSum, string text)
        {
            int min = columnNamesSum.Select(key => table.TableColumns[key]).Min();
            int max = columnNamesSum.Select(key => table.TableColumns[key]).Max();
            MergeCells mergeCells = GetOrCreateMergeCells();
            List<string> mergeCellsReference = new List<string>();            
            foreach (string name in table.TableColumns.Keys)
            {
                if (columnNamesSum.Contains(name) == true)
                {
                    SumColumn(table, name);
                }
                else
                {
                    Cell cell = GetCell(ConvertToLetter(table.RangeBody.Start.column + table.TableColumns[name])
                            + (table.RangeBody.End.row + 1).ToString());
                    if (table.TableColumns[name] < min)
                    {
                        mergeCellsReference.Add(cell.CellReference);
                    }
                    cell.StyleIndex = 8;
                }
            }
            if (mergeCellsReference.Count > 0)
            {
                Cell cell = GetCell(mergeCellsReference.First());
                cell.CellValue = new CellValue(text);                
                DocumentFormat.OpenXml.Spreadsheet.MergeCell mergeCell = new DocumentFormat.OpenXml.Spreadsheet.MergeCell()
                {
                    Reference = $"{mergeCellsReference.First()}:{mergeCellsReference.Last()}",                                        
                };
                mergeCells.AppendChild(mergeCell);
            }
        }
        #region Formulas 
        #region Sum

        private void SumColumn(ModelTable.Table table, string columnName)
        {
            table.TableColumns.TryGetValue(columnName, out int column);
            string startCell = ConvertToLetter(table.RangeBody.Start.column + column) + table.RangeBody.Start.row.ToString();
            string endCell = ConvertToLetter(table.RangeBody.Start.column + column) + table.RangeBody.End.row.ToString();
            SumRange(startCell, endCell, GetCell(ConvertToLetter(table.RangeBody.Start.column + column) + (table.RangeBody.End.row + 1).ToString()));

        }
        private void SumRange(string startCell, string endCell, Cell resultCell)
        {
            resultCell.CellFormula = new CellFormula($"SUM({startCell}:{endCell})");
            resultCell.DataType = CellValues.Number;
            resultCell.StyleIndex = 11;
        }
        #endregion
        #endregion
        #region AutoSize
        private List<DocumentFormat.OpenXml.Spreadsheet.Column> AutoSize(SheetData sheetData)
        {
            Dictionary<int, int> maxColWidth = GetMaxCharacterWidth(sheetData);
            List<DocumentFormat.OpenXml.Spreadsheet.Column> columns = new List<DocumentFormat.OpenXml.Spreadsheet.Column>();
            //this is the width of my font - yours may be different
            double maxWidth = 7;
            foreach (var item in maxColWidth)
            {
                //width = Truncate([{Number of Characters} * {Maximum Digit Width} + {5 pixel padding}]/{Maximum Digit Width}*256)/256
                double width = Math.Truncate((item.Value * maxWidth + 5) / maxWidth * 256) / 256;

                //pixels=Truncate(((256 * {width} + Truncate(128/{Maximum Digit Width}))/256)*{Maximum Digit Width})
                double pixels = Math.Truncate(((256 * width + Math.Truncate(128 / maxWidth)) / 256) * maxWidth);

                //character width=Truncate(({pixels}-5)/{Maximum Digit Width} * 100+0.5)/100
                double charWidth = Math.Truncate((pixels - 5) / maxWidth * 100 + 0.5) / 100;               
                DocumentFormat.OpenXml.Spreadsheet.Column col = new DocumentFormat.OpenXml.Spreadsheet.Column() { BestFit = true, Min = (UInt32)(item.Key), Max = (UInt32)(item.Key), CustomWidth = true, Width = (DoubleValue)width };
                columns.Add(col);
            }
             
            return columns;
        }
        private Dictionary<int, int> GetMaxCharacterWidth(SheetData sheetData) // следует использовать только для файлов созданных программно 
        {
            //iterate over all cells getting a max char value for each column
            Dictionary<int, int> maxColWidth = new Dictionary<int, int>();
            var rows = sheetData.Elements<OpenXml.Row>();
            UInt32[] numberStyles = new UInt32[] { 11 }; //styles that will add extra chars
            UInt32[] boldStyles = new UInt32[] { 10 }; //styles that will bold
            foreach (var r in rows)
            {
                var cells = r.Elements<Cell>().ToArray();

                //using cell index as my column
                for (int i = 0; i < cells.Length; i++)
                {
                    Cell cell = cells[i];
                    string cellValue = cell.CellValue == null ? string.Empty : cell.CellValue.InnerText;
                    int cellTextLength = 0;
                    if (string.IsNullOrEmpty(cellValue) == false && cellValue.Contains("\r\n") == true)
                    {
                        string[] parts = cellValue.Split("\r\n");
                        cellTextLength = parts.OrderByDescending(s => s.Length).FirstOrDefault().Length;
                    }
                    else
                    {
                        cellTextLength = cellValue.Length;
                    }                  
                    if (cell.StyleIndex != null && numberStyles.Contains(cell.StyleIndex))
                    {
                        int thousandCount = (int)Math.Truncate((double)cellTextLength / 4);

                        //add 3 for '.00' 
                        cellTextLength += (3 + thousandCount);
                    }

                    if (cell.StyleIndex != null && boldStyles.Contains(cell.StyleIndex))
                    {
                        //add an extra char for bold - not 100% acurate but good enough for what i need.
                        cellTextLength += 3;
                    }
                    int column = ConvertToNumber(SplitString(cell.CellReference).letters);
                    if (maxColWidth.ContainsKey(column))
                    {
                        var current = maxColWidth[column];
                        if (cellTextLength > current)
                        {
                            maxColWidth[column] = cellTextLength;
                        }
                    }
                    else
                    {                                           
                        maxColWidth.Add(column, cellTextLength);
                    }
                }
            }

            return maxColWidth;
        }
        #endregion
        private CellValue CreateCellValue(object data, out CellValues cellValuesType, out Type type)
        {
            cellValuesType = CellValues.String;
            CellValue result = new CellValue("");
            type = data?.GetType();
            if (type == typeof(string))
            {
                result = new CellValue(data.ToString());
            }   
            else if(type == typeof(double) || type == typeof(float))
            {                
                double.TryParse(data.ToString(), out double doubleNumber);
                result = new CellValue(doubleNumber);
                cellValuesType = CellValues.Number;                
            }
            else if(type == typeof(int) || type == typeof(long))
            {
                long.TryParse(data.ToString(), out long longNumber);
                result = new CellValue((int)longNumber);
                cellValuesType = CellValues.Number;
            }           
            return result;
        }
        private MergeCells GetOrCreateMergeCells()
        {
            IEnumerable<MergeCells> mergeCellsList = currentWorksheetPart.Worksheet.Elements<MergeCells>();
            MergeCells mergeCells = new MergeCells();
            if (mergeCellsList.Count() == 0)
            {              
                SheetData sheetData = currentWorksheetPart.Worksheet.Elements<SheetData>().First();
                currentWorksheetPart.Worksheet.InsertAfter(mergeCells, sheetData);
            }
            else
            {
                mergeCells = mergeCellsList.First();
            }
            return mergeCells;
        }
        public void RepaintTable(ModelTable.Table table,List<ModelTable.Row> rows, int pageNumber, string hexColor)
        {
            Active = true;
            uint idColor = GetFillId(hexColor);
            (int row, int column) rangeBodyStart = table.RangeBody.Start;
            (int row, int column) rangeBodyEnd = table.RangeBody.End;
            int rowsCount = 0;
            currentWorksheetPart = usedDocument.WorkbookPart.WorksheetParts.ToList()[pageNumber];
            Worksheet worksheet = currentWorksheetPart.Worksheet;
            List<Cell> cellsAll = worksheet.Descendants<Cell>().ToList();
            for (int row = 0; row < table.Rows.Count; row++)
            {
                if (table.Rows[row].Number == rows[rowsCount].Number)
                {
                    (int row, int column) startCell = (table.Rows[row].Number + rangeBodyStart.row, rangeBodyStart.column);
                    (int row, int column) lastCell = (table.Rows[row].Number + rangeBodyStart.row, rangeBodyEnd.column);
                    RepaintRange(startCell, lastCell, idColor, cellsAll);
                    rowsCount++;
                }               
            }
            Active = false;
        }
        #region RepaintRange
        private void RepaintRange((int row, int column) startCell, (int row, int column) endCell, string hexColor)
        {
            int startRow = Math.Min(startCell.row, endCell.row);
            int endRow = Math.Max(startCell.row, endCell.row);
            int startCol = Math.Min(startCell.column, endCell.column);
            int endCol = Math.Max(startCell.column, endCell.column);
            uint id = (uint)GetFillId(hexColor);
            for (int row = startRow; row <= endRow; row++)
            {
                for (int column = startCol; column <= endCol; column++)
                {
                    RepaintCell((row, column), id);
                }
            }
        }
        private void RepaintRange((int row, int column) startCell, (int row, int column) endCell, uint idColor)
        {     
            int startRow = Math.Min(startCell.row, endCell.row);
            int endRow = Math.Max(startCell.row, endCell.row);
            int startCol = Math.Min(startCell.column, endCell.column);
            int endCol = Math.Max(startCell.column, endCell.column);           
            for (int row = startRow; row <= endRow; row++)
            {                
                for (int column = startCol; column <= endCol; column++)
                {
                    RepaintCell((row, column), idColor);
                }        
            }                                  
        }
        private void RepaintRange((int row, int column) startCell, (int row, int column) endCell, uint idColor, List<Cell> allCell)
        {
            int startRow = Math.Min(startCell.row, endCell.row);
            int endRow = Math.Max(startCell.row, endCell.row);
            int startCol = Math.Min(startCell.column, endCell.column);
            int endCol = Math.Max(startCell.column, endCell.column);
            for (int row = startRow; row <= endRow; row++)
            {
                for (int column = startCol; column <= endCol; column++)
                {
                    RepaintCell((row, column), idColor, allCell);
                }
            }
        }
        #endregion
        #region RepaintCell
        private void RepaintCell((int row, int column) position, string hexColor)
        {           
            Worksheet worksheet = currentWorksheetPart.Worksheet;
            string columnName = ConvertToLetter(position.column);
            string cellReference = columnName + position.row.ToString();
            Cell cell = GetCell(cellReference);
            CellFormat originalCellFormat = usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.Cast<CellFormat>().ToList()[(int)cell.StyleIndex.Value];
            CellFormat cellFormat = (CellFormat)originalCellFormat.CloneNode(true);
            cellFormat.FillId.Value = GetFillId(hexColor);
            cell.StyleIndex = (uint)GetStyleIndex(cellFormat);
        }

        private void RepaintCell((int row, int column) position, uint idColor)
        {           
            Worksheet worksheet = currentWorksheetPart.Worksheet;
            string columnName = ConvertToLetter(position.column);
            string cellReference = columnName + position.row.ToString();
            Cell cell = GetCell(cellReference);
            CellFormat originalCellFormat = usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.Cast<CellFormat>().ToList()[(int)cell.StyleIndex.Value];
            CellFormat cellFormat = new CellFormat(new string(originalCellFormat.OuterXml));
            cellFormat.FillId.Value = idColor;
            int index = GetStyleIndex(cellFormat, presumptiveStyleIndex);
            cell.StyleIndex = (uint)index;        
        }
        private int presumptiveStyleIndex = 0;
        private void RepaintCell((int row, int column) position, uint idColor, List<Cell> allCell)
        { 
            Worksheet worksheet = currentWorksheetPart.Worksheet;
            string columnName = ConvertToLetter(position.column);
            string cellReference = columnName + position.row.ToString();
            Cell cell = GetCell(cellReference, allCell);
            CellFormat originalCellFormat = usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.Cast<CellFormat>().ToList()[(int)cell.StyleIndex.Value];
            CellFormat cellFormat = new CellFormat(new string(originalCellFormat.OuterXml));
            cellFormat.FillId.Value = idColor;
            int index = GetStyleIndex(cellFormat, presumptiveStyleIndex);
            cell.StyleIndex = (uint)index;
            presumptiveStyleIndex = index;
        }
        #endregion       

        private int GetStyleIndex(CellFormat cellFormat)
        {
            List<CellFormat> cellFormats = usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.Cast<CellFormat>().ToList();
            bool contains = false;
            for (int i = 0; i < cellFormats.Count; i++)
            {
                if (cellFormats[i].BorderId.Value == cellFormat.BorderId.Value &&
                    cellFormats[i].FillId.Value == cellFormat.FillId.Value &&
                    cellFormats[i].FontId.Value == cellFormat.FontId.Value &&
                    cellFormats[i].FormatId.Value == cellFormat.FormatId.Value &&
                    cellFormats[i].NumberFormatId.Value == cellFormat.NumberFormatId.Value)
                {
                    contains = true;
                    cellFormat = cellFormats[i];
                    break;
                }                
            }
            if (contains == false)
            {                
                usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.AppendChild(cellFormat);
                usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.Count =
    (uint)usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ChildElements.Count;
            }
            return usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ToList().IndexOf(cellFormat);
        }
        private int GetStyleIndex(CellFormat cellFormat, int presumptiveStyleIndex)
        {
            List<CellFormat> cellFormats = usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.Cast<CellFormat>().ToList();
            if (cellFormats[presumptiveStyleIndex].BorderId.Value == cellFormat.BorderId.Value &&
                   cellFormats[presumptiveStyleIndex].FillId.Value == cellFormat.FillId.Value &&
                   cellFormats[presumptiveStyleIndex].FontId.Value == cellFormat.FontId.Value &&
                   cellFormats[presumptiveStyleIndex].FormatId.Value == cellFormat.FormatId.Value &&
                   cellFormats[presumptiveStyleIndex].NumberFormatId.Value == cellFormat.NumberFormatId.Value)
            {
                cellFormats = null;
                return presumptiveStyleIndex;
            }
            for (int i = 0; i < cellFormats.Count; i++)
            {
                if (cellFormats[i].BorderId.Value == cellFormat.BorderId.Value &&
                   cellFormats[i].FillId.Value == cellFormat.FillId.Value &&
                   cellFormats[i].FontId.Value == cellFormat.FontId.Value &&
                   cellFormats[i].FormatId.Value == cellFormat.FormatId.Value &&
                   cellFormats[i].NumberFormatId.Value == cellFormat.NumberFormatId.Value)
                {
                    cellFormats = null;
                    return i;
                }
            }
            usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.AppendChild(cellFormat);
            usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.Count =
(uint)usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ChildElements.Count;
            cellFormats = null;
            return usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ToList().IndexOf(cellFormat);
        }

        private uint GetFillId(string hexColor)
        {
            List<Fill> fills = usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.Fills.Cast<Fill>().ToList();
            Fill fill = fills.FirstOrDefault(item => item.PatternFill?.ForegroundColor?.Rgb == hexColor);            
            if (fill == null)
            {
                fill = new Fill();
                PatternFill patternFill = new PatternFill();
                patternFill.PatternType = PatternValues.Solid;            
                ForegroundColor foregroundColor = new ForegroundColor() { Rgb = new HexBinaryValue() { Value = hexColor } };
                patternFill.ForegroundColor = foregroundColor;                
                fill.AppendChild(patternFill);
                usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.Fills.AppendChild(fill);
                usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.Fills.Count = 
                    (uint)usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.Fills.ChildElements.Count;
            }          
            return (uint)usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.Fills.ToList().IndexOf(fill);
        }
        #region GetCell
        private int presumptiveCellIndex = 0;
        private Cell GetCell(string cellReference, List<Cell> cellsAll)
        {         
            Worksheet worksheet = currentWorksheetPart.Worksheet;
            Cell cell = null;
            if (cellsAll[presumptiveCellIndex + 1].CellReference == cellReference)
            {
                presumptiveCellIndex = presumptiveCellIndex + 1;
                cell = cellsAll[presumptiveCellIndex];
            }
            if (cell == null)
            {
                for (int i = 0; i < cellsAll.Count(); i++)
                {
                    if (cellsAll[i].CellReference == cellReference)
                    {
                        presumptiveCellIndex = i;
                        cell = cellsAll[i];
                    }
                }
            }                           
            if (cell == null)
            {
                cell = new Cell()
                {
                    CellReference = new string(cellReference),
                    DataType = CellValues.String,
                    StyleIndex = 0

                };              
                string numbersOnly = Regex.Replace(cellReference, "[^0-9]", "");
                int rowNumber = int.Parse(numbersOnly);
                OpenXml.Row row = GetRow(rowNumber);
                if (row.Count() > 0)
                {
                    string lettersOnly = Regex.Replace(cell.CellReference.Value, "[^A-Za-z]", "");
                    int column = ConvertToNumber(lettersOnly);
                    List<Cell> cells = row.Descendants<Cell>().ToList();;
                    Cell afterCell = null;
                    for (int i = 0; i < cells.Count; i++)
                    {
                        lettersOnly = Regex.Replace(cells[i].CellReference.Value, "[^A-Za-z]", "");
                        int columnAfter = ConvertToNumber(lettersOnly);
                        if (columnAfter > column)
                        {
                            afterCell = cells[i];
                            break;
                        }
                    }
                    if (afterCell != null)
                    {
                        row.InsertBefore(cell, afterCell);
                        return cell;
                    }
                }               
                row.AppendChild(cell);
                cellsAll.Add(cell);
            }
            if (cell.StyleIndex == null)
            {
                UInt32Value uInt32Value = new UInt32Value((uint)0);
                cell.StyleIndex = uInt32Value;
            }          
            return cell;
        }     
        private Cell GetCell(string cellReference)
        {
            Worksheet worksheet = currentWorksheetPart.Worksheet;           
            Cell cell = worksheet.Descendants<Cell>().FirstOrDefault(c => c.CellReference == cellReference);           
            if (cell == null)
            {
                cell = new Cell()
                {
                    CellReference = new string(cellReference),
                    DataType = CellValues.String,
                    StyleIndex = 0

                };
                string numbersOnly = Regex.Replace(cellReference, "[^0-9]", "");
                int rowNumber = int.Parse(numbersOnly);
                OpenXml.Row row = GetRow(rowNumber);
                if (row.Count() > 0)
                {
                    string lettersOnly = Regex.Replace(cell.CellReference.Value, "[^A-Za-z]", "");
                    int column = ConvertToNumber(lettersOnly);
                    List<Cell> cells = row.Descendants<Cell>().ToList(); ;
                    Cell afterCell = null;
                    for (int i = 0; i < cells.Count; i++)
                    {
                        lettersOnly = Regex.Replace(cells[i].CellReference.Value, "[^A-Za-z]", "");
                        int columnAfter = ConvertToNumber(lettersOnly);
                        if (columnAfter > column)
                        {
                            afterCell = cells[i];
                            break;
                        }
                    }
                    if (afterCell != null)
                    {
                        row.InsertBefore(cell, afterCell);
                        return cell;
                    }
                }
                row.AppendChild(cell);
            }
            if (cell.StyleIndex == null)
            {
                UInt32Value uInt32Value = new UInt32Value((uint)0);
                cell.StyleIndex = uInt32Value;
            }
            return cell;
        }
        #endregion
        private OpenXml.Row GetRow(int rowIndex)
        {                    
            SheetData sheetData = currentWorksheetPart.Worksheet.Elements<SheetData>().First();
            OpenXml.Row row = currentWorksheetPart.Worksheet.Descendants<OpenXml.Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
            OpenXml.Row afterRow = null;
            int index = -1;
            if (row == null)
            {
                row = new OpenXml.Row() { RowIndex = Convert.ToUInt32(rowIndex) };
                List<OpenXml.Row> rows = currentWorksheetPart.Worksheet.Descendants<OpenXml.Row>().ToList();                
                int left = 0;
                int right = rows.Count - 1;             

                while (left <= right)
                {
                    int mid = (left + right) / 2;

                    if (rows[mid].RowIndex > rowIndex)
                    {
                        afterRow = rows[mid];
                        index = (int)rows[mid].RowIndex.Value;
                        right = mid - 1;
                    }
                    else
                    {
                        left = mid + 1;
                    }
                }
            }
            else
            {
                return row;
            }
            if (index > 0)
            {
                sheetData.InsertBefore(row, afterRow);
            }
            else
            {
                sheetData.AppendChild(row);
            }                    
            return row;
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
            return new (null, 0); 
        }
    }
}
