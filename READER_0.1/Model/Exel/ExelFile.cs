using System;
using System.Collections.Generic;
using System.Text;
using READER_0._1.Model.Settings;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace READER_0._1.Model.Excel
{
    public class ExcelFile : File
    {
        public  List<Page> ExcelPages { get; private set; }       

        public ExcelFile(string path, string name, string format) : base(path, name, format)
        {
            ExcelPages = new List<Page>();
            Readed = false;
        }
        /// <summary>
        /// if createNewFile true create new file in Path
        /// </summary>
        public ExcelFile(string path, bool createNewFile)
        {
            Path = path;
            if (createNewFile == true)
            {
                CeateFile();
            }
            Format =  System.IO.Path.GetExtension(path);
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            ExcelPages = new List<Page>();           
        }

        public ExcelFile(ExcelFile excelFile)
        {
            Name = excelFile.Name;
            Path = excelFile.Path;
            Readed = excelFile.Readed;
            Format = excelFile.Format;
            ExcelPages = excelFile.ExcelPages;            
        }       
        public void AddPage(Page page)
        {
            if (page != null)
            {
                ExcelPages.Add(page);
            }           
        }
        public void AddPage(List<Page> page)
        {
            if (page != null)
            {
                ExcelPages.AddRange(page);
            }           
        }
        private void CeateFile()
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(Path, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();           
                worksheetPart.Worksheet = new Worksheet(new Columns(), new SheetData());
                Sheets sheets = document.WorkbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Лист1" };                
                sheets.Append(sheet);
                WorkbookStylesPart workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                workbookStylesPart.Stylesheet = CreateStyleSheet();
               
            }
        }
        private Stylesheet CreateStyleSheet()
        {
            Stylesheet stylesheet = new Stylesheet();
            #region Number format
            uint DATETIME_FORMAT = 164;
            uint DIGITS4_FORMAT = 165;
            var numberingFormats = new NumberingFormats();
            numberingFormats.Append(new NumberingFormat // Datetime format
            {
                NumberFormatId = UInt32Value.FromUInt32(DATETIME_FORMAT),
                FormatCode = StringValue.FromString("dd/mm/yyyy hh:mm:ss")
            });
            
            numberingFormats.Append(new NumberingFormat // four digits format
            {
                NumberFormatId = UInt32Value.FromUInt32(DIGITS4_FORMAT),
                FormatCode = StringValue.FromString("0000")
            });
            
            numberingFormats.Count = UInt32Value.FromUInt32((uint)numberingFormats.ChildElements.Count);
            #endregion

            #region Fonts
            var fonts = new Fonts();
            fonts.Append(new DocumentFormat.OpenXml.Spreadsheet.Font()  // Font index 0 - default
            {
                FontName = new FontName { Val = StringValue.FromString("Times New Roman") },
                FontSize = new FontSize { Val = DoubleValue.FromDouble(11) }
            });
            fonts.Append(new DocumentFormat.OpenXml.Spreadsheet.Font()  // Font index 1
            {
                FontName = new FontName { Val = StringValue.FromString("Times New Roman") },
                FontSize = new FontSize { Val = DoubleValue.FromDouble(11) },
                Bold = new Bold()
            });
            fonts.Append(new DocumentFormat.OpenXml.Spreadsheet.Font()  // Font index 2
            {
                FontName = new FontName { Val = StringValue.FromString("Arial") },
                FontSize = new FontSize { Val = DoubleValue.FromDouble(11) },
                Bold = new Bold()
            });
            fonts.Count = UInt32Value.FromUInt32((uint)fonts.ChildElements.Count);
            #endregion

            #region Fills
            var fills = new Fills();
            fills.Append(new Fill() // Fill index 0
            {
                PatternFill = new PatternFill { PatternType = PatternValues.None }
            });
            fills.Append(new Fill() // Fill index 1
            {
                PatternFill = new PatternFill { PatternType = PatternValues.Gray125 }
            });
            fills.Append(new Fill() // Fill index 2
            {
                PatternFill = new PatternFill
                {
                    PatternType = PatternValues.Solid,
                    ForegroundColor = TranslateForeground(System.Drawing.Color.LightBlue),
                    BackgroundColor = new BackgroundColor { Rgb = TranslateForeground(System.Drawing.Color.LightBlue).Rgb }
                }
            });
            fills.Append(new Fill() // Fill index 3
            {
                PatternFill = new PatternFill
                {
                    PatternType = PatternValues.Solid,
                    ForegroundColor = TranslateForeground(System.Drawing.Color.LightSkyBlue),
                    BackgroundColor = new BackgroundColor { Rgb = TranslateForeground(System.Drawing.Color.LightBlue).Rgb }
                }
            });
            fills.Count = UInt32Value.FromUInt32((uint)fills.ChildElements.Count);
            #endregion

            #region Borders
            var borders = new Borders();
            borders.Append(new Border   // Border index 0: no border
            {
                LeftBorder = new LeftBorder(),
                RightBorder = new RightBorder(),
                TopBorder = new TopBorder(),
                BottomBorder = new BottomBorder(),
                DiagonalBorder = new DiagonalBorder()
            });
            borders.Append(new Border    //Boarder Index 1: All
            {
                LeftBorder = new LeftBorder { Style = BorderStyleValues.Thin },
                RightBorder = new RightBorder { Style = BorderStyleValues.Thin },
                TopBorder = new TopBorder { Style = BorderStyleValues.Thin },
                BottomBorder = new BottomBorder { Style = BorderStyleValues.Thin },
                DiagonalBorder = new DiagonalBorder()
            });
            borders.Append(new Border   // Boarder Index 2: Top and Bottom
            {
                LeftBorder = new LeftBorder(),
                RightBorder = new RightBorder(),
                TopBorder = new TopBorder { Style = BorderStyleValues.Thin },
                BottomBorder = new BottomBorder { Style = BorderStyleValues.Thin },
                DiagonalBorder = new DiagonalBorder()
            });
            borders.Append(new Border   // Boarder Index 3: Top and Bottom and Left
            {
                LeftBorder = new LeftBorder() { Style = BorderStyleValues.Thin },
                RightBorder = new RightBorder(),
                TopBorder = new TopBorder { Style = BorderStyleValues.Thin },
                BottomBorder = new BottomBorder { Style = BorderStyleValues.Thin },
                DiagonalBorder = new DiagonalBorder()
            });
            borders.Append(new Border   // Boarder Index 4: Top and Bottom and Right
            {
                LeftBorder = new LeftBorder(),
                RightBorder = new RightBorder() { Style = BorderStyleValues.Thin },
                TopBorder = new TopBorder { Style = BorderStyleValues.Thin },
                BottomBorder = new BottomBorder { Style = BorderStyleValues.Thin },
                DiagonalBorder = new DiagonalBorder()
            });
            borders.Count = UInt32Value.FromUInt32((uint)borders.ChildElements.Count);
            #endregion

            #region Cell Style Format
            var cellStyleFormats = new CellStyleFormats();
            cellStyleFormats.Append(new CellFormat  // Cell style format index 0: no format
            {
                NumberFormatId = 0,
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                FormatId = 0
            });
            cellStyleFormats.Count = UInt32Value.FromUInt32((uint)cellStyleFormats.ChildElements.Count);
            #endregion

            #region Cell format
            var cellFormats = new CellFormats();
            cellFormats.Append(new CellFormat());    // Cell format index 0
            cellFormats.Append(new CellFormat   // CellFormat index 1
            {
                NumberFormatId = 14,        // 14 = 'mm-dd-yy'. Standard Date format;
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            });
            cellFormats.Append(new CellFormat   // Cell format index 2: Standard Number format with 2 decimal placing
            {
                NumberFormatId = 4,        // 4 = '#,##0.00';
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            });
            cellFormats.Append(new CellFormat   // Cell formt index 3
            {
                NumberFormatId = DATETIME_FORMAT,        // 164 = 'dd/mm/yyyy hh:mm:ss'. Standard Datetime format;
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            });
            cellFormats.Append(new CellFormat   // Cell format index 4
            {
                NumberFormatId = 3, // 3   #,##0
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            });
            cellFormats.Append(new CellFormat    // Cell format index 5
            {
                NumberFormatId = 4, // 4   #,##0.00
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            });
            cellFormats.Append(new CellFormat   // Cell format index 6
            {
                NumberFormatId = 10,    // 10  0.00 %,
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            });
            cellFormats.Append(new CellFormat   // Cell format index 7
            {
                NumberFormatId = DIGITS4_FORMAT,    // Format cellas 4 digits. If less than 4 digits, prepend 0 in front
                FontId = 0,
                FillId = 0,
                BorderId = 0,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            });
            cellFormats.Append(new CellFormat   // Cell format index 8
            {
                NumberFormatId = 49,   //NumberFormatId = 28,
                FontId = 0,
                FillId = 0,
                BorderId = 1,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Center,Vertical = VerticalAlignmentValues.Center, WrapText = BooleanValue.FromBoolean(true) }
            });
            cellFormats.Append(new CellFormat   // Cell format index 9
            {
                NumberFormatId = 28,  
                FontId = 0,
                FillId = 0,
                BorderId = 1,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true)
            });
            cellFormats.Append(new CellFormat   // Cell format index 10: Cell header
            {
                NumberFormatId = 43,               
                FontId = 1,
                FillId = 0,
                BorderId = 1,
                FormatId = 1,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }
            });
            cellFormats.Append(new CellFormat   // Cell format index 11, оформления для дабла
            {
                NumberFormatId = 43,
                FontId = 0,
                FillId = 0,
                BorderId = 1,
                FormatId = 0,
                ApplyNumberFormat = BooleanValue.FromBoolean(true),
                Alignment = new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }
            });
            cellFormats.Count = UInt32Value.FromUInt32((uint)cellFormats.ChildElements.Count);
            #endregion

            stylesheet.Append(numberingFormats);
            stylesheet.Append(fonts);
            stylesheet.Append(fills);
            stylesheet.Append(borders);
            stylesheet.Append(cellStyleFormats);
            stylesheet.Append(cellFormats);

            #region Cell styles
            var css = new CellStyles();
            css.Append(new CellStyle
            {
                Name = StringValue.FromString("Normal"),
                FormatId = 0,
                BuiltinId = 0
            });
            css.Count = UInt32Value.FromUInt32((uint)css.ChildElements.Count);
            stylesheet.Append(css);
            #endregion

            var dfs = new DifferentialFormats { Count = 0 };
            stylesheet.Append(dfs);
            var tss = new TableStyles
            {
                Count = 0,
                DefaultTableStyle = StringValue.FromString("TableStyleMedium9"),
                DefaultPivotStyle = StringValue.FromString("PivotStyleLight16")
            };
            stylesheet.Append(tss);

            return stylesheet;
        }
        private ForegroundColor TranslateForeground(System.Drawing.Color fillColor)
        {
            return new ForegroundColor()
            {
                Rgb = new HexBinaryValue()
                {
                    Value =
                              System.Drawing.ColorTranslator.ToHtml(
                              System.Drawing.Color.FromArgb(
                                  fillColor.A,
                                  fillColor.R,
                                  fillColor.G,
                                  fillColor.B)).Replace("#", "")
                }
            };
        }
    }
}

