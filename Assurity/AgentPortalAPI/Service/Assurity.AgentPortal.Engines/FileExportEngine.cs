namespace Assurity.AgentPortal.Engines;

using System.ComponentModel;
using System.Reflection;
using Assurity.AgentPortal.Contracts.FileExportEngine;
using Assurity.AgentPortal.Engines;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

public class FileExportEngine : IFileExportEngine
{
    private const uint CURRENCYFORMAT = 164;

    public byte[] CreateExcelDocument<T>(List<string> headers, List<T> data, string sheetName = "data")
    {
        // If data is null or contains only null elements, create an empty sheet with just headers
        if (data == null || data.All(item => item == null))
        {
            // Create an empty list of T if all elements are null
            data = new List<T>();
        }

        // Helpful article explaining a lot of openxml functionality: https://jason-ge.medium.com/create-excel-using-openxml-in-net-6-3b601ddf48f7
        var memoryStream = new MemoryStream();
        var spreadsheetDocument = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook);

        // Add a WorkbookPart to the document.
        var workbookpart = spreadsheetDocument.AddWorkbookPart();
        workbookpart.Workbook = new Workbook();

        var stylesPart = workbookpart.AddNewPart<WorkbookStylesPart>();
        stylesPart.Stylesheet = CreateStylesheet();
        stylesPart.Stylesheet.Save();

        // Add a WorksheetPart to the WorkbookPart and generate a new Worksheet.
        var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
        worksheetPart.Worksheet = new Worksheet();

        // Generate SheetData.
        var sheetData = InsertSheetData(headers, data);

        // Adjust the column width and add SheetData to the WorksheetPart.
        var columns2 = AutoSizeCells(sheetData);
        worksheetPart.Worksheet.Append(columns2);
        worksheetPart.Worksheet.Append(sheetData);

        // Add Sheets to the Workbook, append a new worksheet, and associate it with the workbook.
        var sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
        var sheet = new Sheet()
        {
            Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
            SheetId = 1,
            Name = sheetName
        };
        sheets.Append(sheet);

        // Save the workbook.
        workbookpart.Workbook.Save();

        // Close the document.
        spreadsheetDocument.Dispose();
        return memoryStream.ToArray();
    }

    public List<string> CreateHeaders<T>()
    {
        return typeof(T).GetProperties().Select(property => GetPropertyDisplayName(property)).ToList();
    }

    private static string GetPropertyDisplayName(PropertyInfo property)
    {
        var atts = property.GetCustomAttributes(typeof(DisplayNameAttribute), true);
        if (atts.Length == 0)
        {
            return string.Empty;
        }

        return (atts[0] as DisplayNameAttribute).DisplayName;
    }

    private static SheetData InsertSheetData<T>(List<string> headers, List<T> data)
    {
        var sheetData = new SheetData();

        // Add headers to the SheetData
        var rowId = 0;
        var row = new Row();

        for (int i = 0; i < headers.Count; i++)
        {
            row.InsertAt(
                new Cell()
                {
                    DataType = CellValues.InlineString,
                    InlineString = new InlineString()
                    {
                        Text = new Text(headers[i])
                    },
                    StyleIndex = 3
                },
                i);
        }

        sheetData.InsertAt(row, rowId++);

        // Add data rows to the SheetData
        for (int i = 0; i < data.Count; i++)
        {
            row = new Row();
            var properties = data[i].GetType().GetProperties();

            for (int j = 0; j < properties.Length; j++)
            {
                object? value = properties[j].GetValue(data[i]);
                var cell = GetCell(value, properties[j].PropertyType);

                row.InsertAt(cell, j);
            }

            sheetData.InsertAt(row, rowId++);
        }

        return sheetData;
    }

    private static Cell GetCell(object? value, Type type)
    {
        var cell = new Cell();

        TypeCode typeCode;

        // If type is nullable, get the underlying type.
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            typeCode = Type.GetTypeCode(Nullable.GetUnderlyingType(type));
        }
        else
        {
            typeCode = Type.GetTypeCode(type);
        }

        switch (typeCode)
        {
            case TypeCode.DateTime:
                cell.DataType = CellValues.Date;
                cell.CellValue = value == null ? null : new CellValue(((DateTime)value).Date);
                cell.StyleIndex = 1; // Style Index 1 = simple date time
                break;
            case TypeCode.Int32:
                cell.DataType = CellValues.Number;
                cell.CellValue = value == null ? null : new CellValue((int)value);
                break;
            case TypeCode.Decimal:
                cell.DataType = CellValues.Number;
                cell.CellValue = value == null ? null : new CellValue((decimal)value);
                cell.StyleIndex = 2; // Style Index 2 = currency
                break;
            case TypeCode.Object:
                if (value is ExcelDataCell excelDataCell)
                {
                    cell.DataType = CellValues.Number;
                    cell.CellValue = excelDataCell.Value == null ? null : new CellValue((decimal)excelDataCell.Value);
                    cell.StyleIndex = (uint)excelDataCell.Format;
                }
                else
                {
                    cell.DataType = CellValues.InlineString; // Default to string
                    cell.InlineString = value == null || (string)value == string.Empty ? null : new InlineString()
                    {
                        Text = new Text((string)value)
                    };
                }

                break;
            default:
                cell.DataType = CellValues.InlineString; // Default to string
                cell.InlineString = value == null || (string)value == string.Empty ? null : new InlineString()
                {
                    Text = new Text((string)value)
                };
                break;
        }

        return cell;
    }

    private static NumberingFormats CreateNumberingFormats()
    {
        // NumberingFormatId Default List: https://learn.microsoft.com/en-us/dotnet/api/documentformat.openxml.spreadsheet.numberingformat?view=openxml-2.8.1
        var numberingFormats = new NumberingFormats();

        // Currency format -- in excel, the NumberFormatId is 169 and the FormatCode is "\"$\"#,##0.00" but setting the NumberFormatId to 169 doesn't set the type to currency
        // Every id >= 164 is a custom numberingformat
        numberingFormats.Append(new NumberingFormat
        {
            NumberFormatId = CURRENCYFORMAT,
            FormatCode = "\"$\"#,##0.00"
        });

        numberingFormats.Count = UInt32Value.FromUInt32((uint)numberingFormats.ChildElements.Count);
        return numberingFormats;
    }

    private static Fonts CreateFonts()
    {
        var fonts = new Fonts();

        // Font index 0 is default
        fonts.Append(new Font
        {
            FontName = new FontName { Val = StringValue.FromString("Calibri") },
            FontSize = new FontSize { Val = DoubleValue.FromDouble(11) }
        });

        // Font index 1
        fonts.Append(new Font
        {
            FontName = new FontName { Val = StringValue.FromString("Arial") },
            FontSize = new FontSize { Val = DoubleValue.FromDouble(11) },
            Bold = new Bold()
        });

        fonts.Count = UInt32Value.FromUInt32((uint)fonts.ChildElements.Count);
        return fonts;
    }

    private static Fills CreateFills()
    {
        var fills = new Fills(); // Fills 0 and 1 are reserved in Excel with the following default fills

        // Fill index 0 -- default Excel Fill of no fill
        fills.Append(new Fill
        {
            PatternFill = new PatternFill { PatternType = PatternValues.None }
        });

        // Fill index 1 -- default Excel Fill of grey dotted background
        fills.Append(new Fill
        {
            PatternFill = new PatternFill { PatternType = PatternValues.Gray125 }
        });

        // Fill index 2
        fills.Append(new Fill
        {
            PatternFill = new PatternFill
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor { Rgb = new HexBinaryValue("ADD8E6") }, // Blue
                BackgroundColor = new BackgroundColor { Rgb = new HexBinaryValue("ADD8E6") },
            }
        });

        // Fill index 3
        fills.Append(new Fill
        {
            PatternFill = new PatternFill
            {
                PatternType = PatternValues.Solid,
                ForegroundColor = new ForegroundColor { Rgb = new HexBinaryValue("C5E4DB") }, // Teal
                BackgroundColor = new BackgroundColor { Rgb = new HexBinaryValue("C5E4DB") },
            }
        });

        fills.Count = UInt32Value.FromUInt32((uint)fills.ChildElements.Count);
        return fills;
    }

    private static Borders CreateBorders()
    {
        var borders = new Borders();

        // Border index 0: no border
        borders.Append(new Border
        {
            LeftBorder = new LeftBorder(),
            RightBorder = new RightBorder(),
            TopBorder = new TopBorder(),
            BottomBorder = new BottomBorder(),
            DiagonalBorder = new DiagonalBorder()
        });

        // Border Index 1: All borders
        borders.Append(new Border
        {
            LeftBorder = new LeftBorder { Style = BorderStyleValues.Thin },
            RightBorder = new RightBorder { Style = BorderStyleValues.Thin },
            TopBorder = new TopBorder { Style = BorderStyleValues.Thin },
            BottomBorder = new BottomBorder { Style = BorderStyleValues.Thin },
            DiagonalBorder = new DiagonalBorder()
        });

        // Border Index 2: Top and Bottom borders
        borders.Append(new Border
        {
            LeftBorder = new LeftBorder(),
            RightBorder = new RightBorder(),
            TopBorder = new TopBorder { Style = BorderStyleValues.Thin },
            BottomBorder = new BottomBorder { Style = BorderStyleValues.Thin },
            DiagonalBorder = new DiagonalBorder()
        });

        borders.Count = UInt32Value.FromUInt32((uint)borders.ChildElements.Count);
        return borders;
    }

    private static CellStyleFormats CreateCellStyleFormats()
    {
        var cellStyleFormats = new CellStyleFormats();

        // Cell style format index 0: no format
        cellStyleFormats.Append(new CellFormat
        {
            NumberFormatId = 0,
            FontId = 0,
            FillId = 0,
            BorderId = 0,
            FormatId = 0
        });

        cellStyleFormats.Count = UInt32Value.FromUInt32((uint)cellStyleFormats.ChildElements.Count);
        return cellStyleFormats;
    }

    private static CellFormats CreateCellFormats()
    {
        var cellFormats = new CellFormats();

        // Cell format index 0 -- no formatting except for text wrapping in the cell
        cellFormats.Append(new CellFormat
        {
            Alignment = new Alignment
            {
                WrapText = true,
                Vertical = VerticalAlignmentValues.Center
            }
        });

        // CellFormat index 1: Standard Date format | 14 = 'mm-dd-yy'
        cellFormats.Append(new CellFormat
        {
            NumberFormatId = 14,
            FontId = 0,
            FillId = 0,
            BorderId = 0,
            FormatId = 0,
            ApplyNumberFormat = BooleanValue.FromBoolean(true),
            Alignment = new Alignment
            {
                WrapText = true,
                Vertical = VerticalAlignmentValues.Center
            }
        });

        // Cell format index 2: Currency | CURRENCYFORMAT = "$"#,##0.00
        cellFormats.Append(new CellFormat
        {
            NumberFormatId = CURRENCYFORMAT,
            FontId = 0,
            FillId = 0,
            BorderId = 0,
            FormatId = 0,
            ApplyNumberFormat = BooleanValue.FromBoolean(true),
            Alignment = new Alignment
            {
                WrapText = true,
                Vertical = VerticalAlignmentValues.Center
            }
        });

        // Cell format index 3: Cell header | @ (literal text) formatting, Arial font, Teal fill, All borders
        cellFormats.Append(new CellFormat
        {
            NumberFormatId = 49,
            FontId = 1,
            FillId = 3,
            BorderId = 1,
            FormatId = 0,
            ApplyNumberFormat = BooleanValue.FromBoolean(true),
            Alignment = new Alignment
            {
                WrapText = true,
                Horizontal = HorizontalAlignmentValues.Center,
                Vertical = VerticalAlignmentValues.Center
            }
        });

        // Cell format index 4: Fraction | Number with Fraction Symbol = 0%
        cellFormats.Append(new CellFormat
        {
            NumberFormatId = 10,
            FontId = 0,
            FillId = 0,
            BorderId = 0,
            FormatId = 0,
            ApplyNumberFormat = BooleanValue.FromBoolean(true),
            Alignment = new Alignment
            {
                WrapText = true,
                Vertical = VerticalAlignmentValues.Center
            }
        });

        cellFormats.Count = UInt32Value.FromUInt32((uint)cellFormats.ChildElements.Count);
        return cellFormats;
    }

    private static CellStyles CreateCellStyles()
    {
        var css = new CellStyles();
        css.Append(new CellStyle
        {
            Name = StringValue.FromString("Normal"),
            FormatId = 0,
            BuiltinId = 0
        });

        css.Count = UInt32Value.FromUInt32((uint)css.ChildElements.Count);
        return css;
    }

    private static DifferentialFormats CreateDifferentialFormats()
    {
        return new DifferentialFormats { Count = 0 };
    }

    private static TableStyles CreateTableStyles()
    {
        return new TableStyles
        {
            Count = 0,
            DefaultTableStyle = StringValue.FromString("TableStyleMedium9"),
            DefaultPivotStyle = StringValue.FromString("PivotStyleLight16")
        };
    }

    private static Stylesheet CreateStylesheet()
    {
        // Stylesheets must be formatted in the right order or you will have a corrupted excel spreadsheet
        // If you specify one field for a cell format, you must ensure *ALL* fields are specified for that section
        var stylesheet = new Stylesheet();

        // NumberingFormats speficy datetime or currency formating
        var numberingFormats = CreateNumberingFormats();
        stylesheet.Append(numberingFormats);

        // Fonts of the cells
        var fonts = CreateFonts();
        stylesheet.Append(fonts);

        // Fills the cell with color
        var fills = CreateFills();
        stylesheet.Append(fills);

        // Borders of the cells
        var borders = CreateBorders();
        stylesheet.Append(borders);

        // Cell Style Formats
        var cellStyleFormats = CreateCellStyleFormats();
        stylesheet.Append(cellStyleFormats);

        // Cell Formats are the reference for your cell's StyleIndex
        var cellFormats = CreateCellFormats();
        stylesheet.Append(cellFormats);

        // Cell Styles
        var css = CreateCellStyles();
        stylesheet.Append(css);

        // Differential Formats
        var dfs = CreateDifferentialFormats();
        stylesheet.Append(dfs);

        // Table Styles
        var tss = CreateTableStyles();
        stylesheet.Append(tss);

        return stylesheet;
    }

    private static Dictionary<int, int> GetMaxCharacterWidth(SheetData sheetData)
    {
        // Iterate over all of the cells getting a max char value for each column to calculate the column width
        var maxColWidth = new Dictionary<int, int>();
        var rows = sheetData.Elements<Row>();
        foreach (var r in rows)
        {
            var cells = r.Elements<Cell>().ToArray();

            // Using cell index as my column
            for (int cellIndex = 0; cellIndex < cells.Length; cellIndex++)
            {
                var cell = cells[cellIndex];
                var cellValue = cell.CellValue == null ? cell.InnerText : cell.CellValue.InnerText;
                var cellTextLength = cellValue.Length;

                // 3 is the style index for the bold header and arial font which takes up more space
                if (cell.StyleIndex != null && cell.StyleIndex == 3)
                {
                    // Add an extra few characters to account for the bold font - not 100% acurate but good enough
                    cellTextLength += 5;
                }

                if (maxColWidth.ContainsKey(cellIndex))
                {
                    var current = maxColWidth[cellIndex];
                    if (cellTextLength > current)
                    {
                        maxColWidth[cellIndex] = cellTextLength;
                    }
                }
                else
                {
                    maxColWidth.Add(cellIndex, cellTextLength);
                }
            }
        }

        return maxColWidth;
    }

    private static Columns AutoSizeCells(SheetData sheetData)
    {
        // Column Documentation: https://learn.microsoft.com/en-us/dotnet/api/documentformat.openxml.spreadsheet.column?view=openxml-2.8.1
        var maxColWidth = 100;
        var maxColWidthPerCellData = GetMaxCharacterWidth(sheetData);
        var columns = new Columns();

        // Set the max width of the font characters
        // Using the Calibri font as an example, the maximum digit width of 11 point font size is 7 pixels (at 96 dpi)
        var maxCharacterWidth = 7;
        foreach (var cell in maxColWidthPerCellData)
        {
            // colWidth = Truncate([{Number of Characters} * {Maximum Digit Width} + {50 pixel padding}]/{Maximum Digit Width}*256)/256
            var colWidth = Math.Truncate((double)((cell.Value * maxCharacterWidth) + 50) / maxCharacterWidth * 256) / 256;

            // Set a maximum width for the column
            var width = colWidth > maxColWidth ? maxColWidth : colWidth;

            // Column Min == First column affected by this 'column info' record
            // Column Max == Last column affected by this 'column info' record
            var column = new Column
            {
                BestFit = true,
                Min = (uint)(cell.Key + 1),
                Max = (uint)(cell.Key + 1),
                CustomWidth = true,
                Width = (DoubleValue)width
            };
            columns.Append(column);
        }

        return columns;
    }
}
