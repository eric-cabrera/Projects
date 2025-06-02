namespace Assurity.AgentPortal.Engine.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Contracts.CommissionsDebt.FileExport;
using Assurity.AgentPortal.Contracts.FileExportEngine;
using Assurity.AgentPortal.Contracts.ProductionCredit.FileExport;
using Assurity.AgentPortal.Engines;
using Assurity.AgentPortal.Engines.Tests;
using AutoBogus;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Xunit;

[ExcludeFromCodeCoverage]
public class FileExportEngineTests
{
    [Fact]
    public void CreateExcelDocument_ShouldReturnFullyPopulatedExcelDocument()
    {
        // Set the number of expected columns, rows, and total cells based on the generated data
        var expectedNumberOfColumns = typeof(Data).GetProperties().Length;
        var numberOfRowsOfData = 10;
        var expectedNumberOfRows = numberOfRowsOfData + 1;
        var expectedNumberOfCells = expectedNumberOfColumns * expectedNumberOfRows;

        // Create the fake data
        var headers = GenerateTestData.GenerateHeaders(expectedNumberOfColumns);
        var data = GenerateTestData.GenerateData(numberOfRowsOfData);

        // Create the fake document
        var fileExportEngine = new FileExportEngine();
        var excelDocument = fileExportEngine.CreateExcelDocument(headers, data, "Fake Data");
        Assert.NotNull(excelDocument);

        // Open the document so we can insepct the elements
        var memoryStream = new MemoryStream(excelDocument);
        var spreadsheetDocument = SpreadsheetDocument.Open(memoryStream, false);

        // Check that the document opens properly and has a WorkbookPart
        Assert.NotNull(spreadsheetDocument);
        var workbookPart = spreadsheetDocument.WorkbookPart;
        Assert.NotNull(workbookPart);

        // Check that a Workbook is associated with the WorkbookPart
        Assert.NotNull(workbookPart.Workbook);

        // Check that there is a WorksheetPart
        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
        Assert.NotNull(worksheetPart);

        // Check the Styles exist
        var workbookStylesPart = workbookPart.WorkbookStylesPart;
        Assert.NotNull(workbookStylesPart);
        var stylesheet = workbookStylesPart.Stylesheet;
        Assert.NotNull(stylesheet);

        // Check all of the styles are set up properly - this is subject to change if the Stylesheet is changed
        Assert.NotNull(stylesheet.Borders);
        Assert.Equal(3, stylesheet.Borders.Count());

        Assert.NotNull(stylesheet.CellFormats);
        Assert.Equal(5, stylesheet.CellFormats.Count());

        Assert.NotNull(stylesheet.CellStyleFormats);
        Assert.Single(stylesheet.CellStyleFormats);

        Assert.NotNull(stylesheet.CellStyles);
        Assert.Single(stylesheet.CellStyles);

        Assert.NotNull(stylesheet.DifferentialFormats);
        Assert.Empty(stylesheet.DifferentialFormats);

        Assert.NotNull(stylesheet.Fills);
        Assert.Equal(4, stylesheet.Fills.Count());

        // Numbering Formats
        Assert.NotNull(stylesheet.FirstChild);
        Assert.Single(stylesheet.FirstChild);

        Assert.NotNull(stylesheet.Fonts);
        Assert.Equal(2, stylesheet.Fonts.Count());

        Assert.NotNull(stylesheet.TableStyles);
        Assert.Empty(stylesheet.TableStyles);

        // Check that the WorksheetPart has a Worksheet
        Worksheet sheet = worksheetPart.Worksheet;
        Assert.NotNull(sheet);

        // Check the total number of cells
        var cells = sheet.Descendants<Cell>();
        Assert.NotNull(cells);
        Assert.Equal(expectedNumberOfCells, cells.Count());

        // Check the total number of rows
        var rows = sheet.Descendants<Row>();
        Assert.NotNull(rows);
        Assert.Equal(expectedNumberOfRows, rows.LongCount());
    }

    [Fact]
    public void CreateExcelDocument_ShouldReturnFullyPopulatedExcelDocument_ShouldFormatPercentageCorrectly()
    {
        // Set the number of expected columns, rows, and total cells based on the generated data
        var data = new AutoFaker<PolicyDetailsExport>()
            .RuleFor(x => x.CommissionRate, f => new ExcelDataCell { Format = ExcelFormat.Fraction, Value = f.Random.Decimal() })
            .Generate(100);
        var expectedNumberOfColumns = typeof(PolicyDetailsExport).GetProperties().Length;
        var expectedNumberOfCells = (data.Count + 1) * expectedNumberOfColumns;

        // Create the fake document
        var fileExportEngine = new FileExportEngine();
        var headers = fileExportEngine.CreateHeaders<PolicyDetailsExport>();
        var excelDocument = fileExportEngine.CreateExcelDocument(headers, data, "Fake Data");
        Assert.NotNull(excelDocument);

        // Open the document so we can insepct the elements
        var memoryStream = new MemoryStream(excelDocument);
        var spreadsheetDocument = SpreadsheetDocument.Open(memoryStream, false);

        // Check that the document opens properly and has a WorkbookPart
        Assert.NotNull(spreadsheetDocument);
        var workbookPart = spreadsheetDocument.WorkbookPart;
        Assert.NotNull(workbookPart);

        // Check that a Workbook is associated with the WorkbookPart
        Assert.NotNull(workbookPart.Workbook);

        // Check that there is a WorksheetPart
        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
        Assert.NotNull(worksheetPart);

        // Check the Styles exist
        var workbookStylesPart = workbookPart.WorkbookStylesPart;
        Assert.NotNull(workbookStylesPart);
        var stylesheet = workbookStylesPart.Stylesheet;
        Assert.NotNull(stylesheet);

        // Check all of the styles are set up properly - this is subject to change if the Stylesheet is changed
        Assert.NotNull(stylesheet.Borders);
        Assert.Equal(3, stylesheet.Borders.Count());

        Assert.NotNull(stylesheet.CellFormats);
        Assert.Equal(5, stylesheet.CellFormats.Count());

        Assert.NotNull(stylesheet.CellStyleFormats);
        Assert.Single(stylesheet.CellStyleFormats);

        Assert.NotNull(stylesheet.CellStyles);
        Assert.Single(stylesheet.CellStyles);

        Assert.NotNull(stylesheet.DifferentialFormats);
        Assert.Empty(stylesheet.DifferentialFormats);

        Assert.NotNull(stylesheet.Fills);
        Assert.Equal(4, stylesheet.Fills.Count());

        // Numbering Formats
        Assert.NotNull(stylesheet.FirstChild);
        Assert.Single(stylesheet.FirstChild);

        Assert.NotNull(stylesheet.Fonts);
        Assert.Equal(2, stylesheet.Fonts.Count());

        Assert.NotNull(stylesheet.TableStyles);
        Assert.Empty(stylesheet.TableStyles);

        // Check that the WorksheetPart has a Worksheet
        Worksheet sheet = worksheetPart.Worksheet;
        Assert.NotNull(sheet);

        // Check the total number of cells
        var cells = sheet.Descendants<Cell>();
        Assert.NotNull(cells);
        Assert.Equal(expectedNumberOfCells, cells.Count());

        // Check the total number of rows
        var rows = sheet.Descendants<Row>();
        Assert.NotNull(rows);
        Assert.Equal(data.Count + 1, rows.LongCount());

        // Verify fraction cell is set correctly
        var firstFractionCell = cells.First(cell => cell.StyleIndex != null && cell.StyleIndex == (uint)ExcelFormat.Fraction);
        Assert.Equal(data.First().CommissionRate.Value.ToString(), firstFractionCell.InnerText);
    }

    [Fact]
    public void CreateExcelDocument_ShouldReturnFullyPopulatedProductionCreditExcelDocument_ShouldFormatPolicyCountCorrectly()
    {
        // Set the number of expected columns, rows, and total cells based on the generated data
        var data = new AutoFaker<ProductionCreditExport>()
            .RuleFor(x => x.PolicyCount, f => new ExcelDataCell { Format = ExcelFormat.NoFormat, Value = f.Random.Decimal() })
            .Generate(100);
        var expectedNumberOfColumns = typeof(ProductionCreditExport).GetProperties().Length;
        var expectedNumberOfCells = (data.Count + 1) * expectedNumberOfColumns;

        // Create the fake document
        var fileExportEngine = new FileExportEngine();
        var headers = fileExportEngine.CreateHeaders<ProductionCreditExport>();
        var excelDocument = fileExportEngine.CreateExcelDocument(headers, data, "Fake Data");
        Assert.NotNull(excelDocument);

        // Open the document so we can insepct the elements
        var memoryStream = new MemoryStream(excelDocument);
        var spreadsheetDocument = SpreadsheetDocument.Open(memoryStream, false);

        // Check that the document opens properly and has a WorkbookPart
        Assert.NotNull(spreadsheetDocument);
        var workbookPart = spreadsheetDocument.WorkbookPart;
        Assert.NotNull(workbookPart);

        // Check that a Workbook is associated with the WorkbookPart
        Assert.NotNull(workbookPart.Workbook);

        // Check that there is a WorksheetPart
        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
        Assert.NotNull(worksheetPart);

        // Check the Styles exist
        var workbookStylesPart = workbookPart.WorkbookStylesPart;
        Assert.NotNull(workbookStylesPart);
        var stylesheet = workbookStylesPart.Stylesheet;
        Assert.NotNull(stylesheet);

        // Check all of the styles are set up properly - this is subject to change if the Stylesheet is changed
        Assert.NotNull(stylesheet.Borders);
        Assert.Equal(3, stylesheet.Borders.Count());

        Assert.NotNull(stylesheet.CellFormats);
        Assert.Equal(5, stylesheet.CellFormats.Count());

        Assert.NotNull(stylesheet.CellStyleFormats);
        Assert.Single(stylesheet.CellStyleFormats);

        Assert.NotNull(stylesheet.CellStyles);
        Assert.Single(stylesheet.CellStyles);

        Assert.NotNull(stylesheet.DifferentialFormats);
        Assert.Empty(stylesheet.DifferentialFormats);

        Assert.NotNull(stylesheet.Fills);
        Assert.Equal(4, stylesheet.Fills.Count());

        // Numbering Formats
        Assert.NotNull(stylesheet.FirstChild);
        Assert.Single(stylesheet.FirstChild);

        Assert.NotNull(stylesheet.Fonts);
        Assert.Equal(2, stylesheet.Fonts.Count());

        Assert.NotNull(stylesheet.TableStyles);
        Assert.Empty(stylesheet.TableStyles);

        // Check that the WorksheetPart has a Worksheet
        Worksheet sheet = worksheetPart.Worksheet;
        Assert.NotNull(sheet);

        // Check the total number of cells
        var cells = sheet.Descendants<Cell>();
        Assert.NotNull(cells);
        Assert.Equal(expectedNumberOfCells, cells.Count());

        // Check the total number of rows
        var rows = sheet.Descendants<Row>();
        Assert.NotNull(rows);
        Assert.Equal(data.Count + 1, rows.LongCount());

        // Verify fraction cell is set correctly
        var firstFractionCell = cells.First(cell => cell.StyleIndex != null && cell.StyleIndex == (uint)ExcelFormat.NoFormat);
        Assert.Equal(data.First().PolicyCount.Value.ToString(), firstFractionCell.InnerText);
    }

    [Fact]
    public void CreateExcelDocument_ShouldReturnFullyPopulatedProductionCreditWorksiteExcelDocument_ShouldFormatPolicyCountCorrectly()
    {
        // Set the number of expected columns, rows, and total cells based on the generated data
        var data = new AutoFaker<ProductionCreditWorksiteExport>()
            .RuleFor(x => x.PolicyCount, f => new ExcelDataCell { Format = ExcelFormat.NoFormat, Value = f.Random.Decimal() })
            .Generate(100);
        var expectedNumberOfColumns = typeof(ProductionCreditWorksiteExport).GetProperties().Length;
        var expectedNumberOfCells = (data.Count + 1) * expectedNumberOfColumns;

        // Create the fake document
        var fileExportEngine = new FileExportEngine();
        var headers = fileExportEngine.CreateHeaders<ProductionCreditWorksiteExport>();
        var excelDocument = fileExportEngine.CreateExcelDocument(headers, data, "Fake Data");
        Assert.NotNull(excelDocument);

        // Open the document so we can insepct the elements
        var memoryStream = new MemoryStream(excelDocument);
        var spreadsheetDocument = SpreadsheetDocument.Open(memoryStream, false);

        // Check that the document opens properly and has a WorkbookPart
        Assert.NotNull(spreadsheetDocument);
        var workbookPart = spreadsheetDocument.WorkbookPart;
        Assert.NotNull(workbookPart);

        // Check that a Workbook is associated with the WorkbookPart
        Assert.NotNull(workbookPart.Workbook);

        // Check that there is a WorksheetPart
        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
        Assert.NotNull(worksheetPart);

        // Check the Styles exist
        var workbookStylesPart = workbookPart.WorkbookStylesPart;
        Assert.NotNull(workbookStylesPart);
        var stylesheet = workbookStylesPart.Stylesheet;
        Assert.NotNull(stylesheet);

        // Check all of the styles are set up properly - this is subject to change if the Stylesheet is changed
        Assert.NotNull(stylesheet.Borders);
        Assert.Equal(3, stylesheet.Borders.Count());

        Assert.NotNull(stylesheet.CellFormats);
        Assert.Equal(5, stylesheet.CellFormats.Count());

        Assert.NotNull(stylesheet.CellStyleFormats);
        Assert.Single(stylesheet.CellStyleFormats);

        Assert.NotNull(stylesheet.CellStyles);
        Assert.Single(stylesheet.CellStyles);

        Assert.NotNull(stylesheet.DifferentialFormats);
        Assert.Empty(stylesheet.DifferentialFormats);

        Assert.NotNull(stylesheet.Fills);
        Assert.Equal(4, stylesheet.Fills.Count());

        // Numbering Formats
        Assert.NotNull(stylesheet.FirstChild);
        Assert.Single(stylesheet.FirstChild);

        Assert.NotNull(stylesheet.Fonts);
        Assert.Equal(2, stylesheet.Fonts.Count());

        Assert.NotNull(stylesheet.TableStyles);
        Assert.Empty(stylesheet.TableStyles);

        // Check that the WorksheetPart has a Worksheet
        Worksheet sheet = worksheetPart.Worksheet;
        Assert.NotNull(sheet);

        // Check the total number of cells
        var cells = sheet.Descendants<Cell>();
        Assert.NotNull(cells);
        Assert.Equal(expectedNumberOfCells, cells.Count());

        // Check the total number of rows
        var rows = sheet.Descendants<Row>();
        Assert.NotNull(rows);
        Assert.Equal(data.Count + 1, rows.LongCount());

        // Verify fraction cell is set correctly
        var firstFractionCell = cells.First(cell => cell.StyleIndex != null && cell.StyleIndex == (uint)ExcelFormat.NoFormat);
        Assert.Equal(data.First().PolicyCount.Value.ToString(), firstFractionCell.InnerText);
    }
}
