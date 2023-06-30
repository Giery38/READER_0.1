using DevExpress.Data.Async;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using READER_0._1.Model.Exel.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Color = DocumentFormat.OpenXml.Spreadsheet.Color;

namespace READER_0._1.Model.Exel
{
    public class ExelFileWriter
    {
        public ExelFile ExelFile { get; private set; }
        private SpreadsheetDocument usedDocument;

        public ExelFileWriter(ExelFile exelFile)
        {
            ExelFile = exelFile;
            
        } 
        public void Open()
        {
            usedDocument = SpreadsheetDocument.Open(ExelFile.TempCopyPath, true);
        }
        public void Close()
        {         
            usedDocument.Save();
            usedDocument.Dispose();
        }
        public void RepaintRange((int row, int column) startCell, (int row, int column) endCell, string hexColor)
        {
            int startRow = Math.Min(startCell.row, endCell.row);
            int endRow = Math.Max(startCell.row, endCell.row);
            int startCol = Math.Min(startCell.column, endCell.column);
            int endCol = Math.Max(startCell.column, endCell.column);

            for (int row = startRow; row <= endRow; row++)
            {
                for (int column = startCol; column <= endCol; column++)
                {
                    RepaintCell((row, column), hexColor);
                }
            }
        }
        public void RepaintCell((int row, int column) position, string hexColor)
        {            
            WorksheetPart worksheetPart = usedDocument.WorkbookPart.WorksheetParts.First();
            Worksheet worksheet = worksheetPart.Worksheet;
            string columnName = ConvertToLetter(position.column);
            string cellReference = columnName + position.row.ToString();                                                      
            Cell cell = GetCell(cellReference);
            CellFormat originalCellFormat = usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.Cast<CellFormat>().ToList()[(int)cell.StyleIndex.Value];
            CellFormat cellFormat = (CellFormat)originalCellFormat.CloneNode(true);
            cellFormat.FillId.Value = (uint)GetFillId(hexColor); 
            cell.StyleIndex = (uint)GetStyleIndex(cellFormat);          
        }

        private int GetStyleIndex(CellFormat cellFormat)
        {
            List<CellFormat> cellFormats = usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.Cast<CellFormat>().ToList();            
            if (cellFormats.Contains(cellFormat) == false)
            {                
                usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.AppendChild(cellFormat);
                usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.Count =
    (uint)usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ChildElements.Count;
            }
            return usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ToList().IndexOf(cellFormat);
        }

        private int GetFillId(string hexColor)
        {
            List<Fill> fills = usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.Fills.Cast<Fill>().ToList();
            Fill fill = fills.FirstOrDefault(item => item.PatternFill?.BackgroundColor?.Rgb == hexColor);            
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
            return usedDocument.WorkbookPart.WorkbookStylesPart.Stylesheet.Fills.ToList().IndexOf(fill);
        }
        private Cell GetCell(string cellReference)
        {
            WorksheetPart worksheetPart = usedDocument.WorkbookPart.WorksheetParts.First();
            Worksheet worksheet = worksheetPart.Worksheet;
            Cell cell = worksheet.Descendants<Cell>().FirstOrDefault(c => c.CellReference == cellReference);           
            if (cell == null)
            {
                cell = new Cell()
                {
                    CellReference = cellReference,
                    DataType = CellValues.String,
                    StyleIndex = 0

                };
                string numbersOnly = Regex.Replace(cellReference, "[^0-9]", "");
                int rowNumber = int.Parse(numbersOnly);
                Row row = GetRow(rowNumber);
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
            }
            if (cell.StyleIndex == null)
            {
                UInt32Value uInt32Value = new UInt32Value((uint)0);
                cell.StyleIndex = uInt32Value;
            }
            return cell;
        }
        private Row GetRow(int rowIndex)
        {         
            WorksheetPart worksheetPart = usedDocument.WorkbookPart.WorksheetParts.First();
            SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
            Row row = worksheetPart.Worksheet.Descendants<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
            Row afterRow = null;
            int index = -1;
            if (row == null)
            {
                row = new Row() { RowIndex = Convert.ToUInt32(rowIndex) };
                List<Row> rows = worksheetPart.Worksheet.Descendants<Row>().ToList();                
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
    }
}
