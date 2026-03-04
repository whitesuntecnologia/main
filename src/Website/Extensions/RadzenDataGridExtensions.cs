using CsvHelper;
using CsvHelper.Configuration;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.JSInterop;
using Radzen.Blazor;
using StaticClass.Extensions;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;

using static StaticClass.Constants;
using static Website.Extensions.ExportDataGridClassBuilder;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

namespace Website.Extensions
{
    public static class RadzenDataGridExtensions
    {
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
        private async static Task<IEnumerable<TItem>> GetAllDataAsync<TItem>(this RadzenDataGrid<TItem> grid, CancellationToken cancellationToken = default)
        {
            var currentGridPage = grid.CurrentPage;

            try
            {
                var originalPageSize = grid.PageSize;
                grid.Visible = false;
                List<TItem> data = new();

                grid.PageSize = int.MaxValue;

                if (!grid.AllowPaging)
                {
                    return grid.Data;
                }

                await grid.FirstPage(true);

                data.AddRange(grid.Data);

                grid.PageSize = originalPageSize;

                return data;
            }
            finally
            {
                grid.Visible = true;
                await grid.GoToPage(currentGridPage);
            }
        }
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.


        public async static Task ExportGridDataAsync<TItem>(
            this RadzenDataGrid<TItem> grid,
            IJSRuntime jSRuntime,
            ExportType exportType,
            string fileName,
            IEnumerable<string>? excludedColumns = default,
            CancellationToken cancellationToken = default)
        {
            switch (exportType)
            {
                case ExportType.Csv:
                    await ExportGridDataAsCsv(grid, jSRuntime, fileName, excludedColumns);
                    break;
                case ExportType.Excel:
                    if(excludedColumns!=null)
                        await ExportGridDataAsExcelAsync(grid, jSRuntime, fileName, excludedColumns , cancellationToken);
                    else
                        await ExportGridDataAsExcelAsync(grid, jSRuntime, fileName);
                    break;
                case ExportType.Pdf:
                    if (excludedColumns != null)
                        await ExportGridDataAsPdfAsync(grid, jSRuntime, fileName, excludedColumns, cancellationToken);
                    else
                        await ExportGridDataAsPdfAsync(grid, jSRuntime, fileName);
                    break;
            }
        }

        private static Type? BuildAndGetDataGridType(IEnumerable<ExportDataGridClassBuilderProperty> properties)
        {
            ExportDataGridClassBuilder classBuilder = new("ExportDataGridDynamicClass");
            var dataGridClass = classBuilder.CreateObject(properties);
            var dataGridClassType = dataGridClass?.GetType();
            return dataGridClassType;
        }

        private static IEnumerable<ExportDataGridClassBuilderProperty> GetDataGridProperties<TItem>(
            RadzenDataGrid<TItem> grid,
            IEnumerable<string>? excludedColumns = default)
        {
            excludedColumns ??= new List<string>();
            return grid.ColumnsCollection.Where(c => c.GetVisible() && c.Title.NotIn(excludedColumns.ToArray()))
                .Select(c => new ExportDataGridClassBuilderProperty
                {
                    Name = c.Property,
                    Header = c.Title,
                    Type = typeof(string),
                }).Where(x => x.Name is not null);
        }

        private async static Task ExportGridDataAsCsv<TItem>(
            RadzenDataGrid<TItem> grid,
            IJSRuntime jSRuntime,
            string fileName,
            IEnumerable<string>? excludedColumns = default)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = true,
            });

            IEnumerable<ExportDataGridClassBuilderProperty> properties = null!;

            if (excludedColumns != null)
                properties = GetDataGridProperties(grid, excludedColumns);
            else
                properties = GetDataGridProperties(grid);

            var dataGridClassType = BuildAndGetDataGridType(properties);

            if (dataGridClassType != null)
            {
                var listType = typeof(List<>).MakeGenericType(dataGridClassType);
                var list = (IList?)Activator.CreateInstance(listType);

                var alldata = await grid.GetAllDataAsync();

                foreach (var idata in alldata)
                {
                    var instance = Activator.CreateInstance(dataGridClassType);

                    foreach (var property in properties)
                    {
                        dataGridClassType.GetProperty(property.Name ?? "")?
                            .SetValue(instance, grid.ColumnsCollection.First(c => c.Property == property.Name).GetValue(idata).ToString());
                    }

                    list?.Add(instance);
                }

                await csv.WriteRecordsAsync(list);
                await csv.FlushAsync();
            }

            var arr = memoryStream.ToArray();

            await jSRuntime.SaveAsAsync($"{fileName}.csv", arr);
        }

        private async static Task ExportGridDataAsExcelAsync<TItem>(
            RadzenDataGrid<TItem> grid,
            IJSRuntime jSRuntime,
            string fileName,
            IEnumerable<string>? excludedColumns = default,
            CancellationToken cancellationToken = default)
        {
            var properties = GetDataGridProperties(grid, excludedColumns);
            var dataGridClassType = BuildAndGetDataGridType(properties);

            if (dataGridClassType != null)
            {
                var listType = typeof(List<>).MakeGenericType(dataGridClassType);
                var list = (IList?)Activator.CreateInstance(listType);

                var alldata = await grid.GetAllDataAsync(cancellationToken);

                foreach (var idata in alldata)
                {
                    var instance = Activator.CreateInstance(dataGridClassType);

                    foreach (var property in properties)
                    {

                        dataGridClassType.GetProperty(property.Name ?? "")?
                            .SetValue(instance, grid.ColumnsCollection.First(c => c.Property == property.Name).GetValue(idata).ToString());
                    }

                    list?.Add(instance);
                }


                using MemoryStream ms = new();
                using var document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook);

                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new(new SheetData());

                var sheets = workbookPart.Workbook.AppendChild<Sheets>(new());

                Sheet sheet = new()
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Data",
                };

                sheets.Append(sheet);

                var worksheet = worksheetPart.Worksheet;
                var sheetData = worksheet.GetFirstChild<SheetData>();

                var props = dataGridClassType.GetProperties();

                //Fila de títulos
                {
                    Row row = new();

                    foreach (var property in props)
                    {
                        var descriptionAttribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(property, typeof(DescriptionAttribute));
                        if (descriptionAttribute != null)
                        {
                            Cell cell = new()
                            {
                                CellValue = new(descriptionAttribute.Description),
                                DataType = CellValues.String,
                            };
                            cell.AppendChild(new Column());
                            row.Append(cell);
                        }

                    }

                    sheetData?.Append(row);
                }

                //Filas de datos
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        Row row = new();

                        foreach (var property in props)
                        {
                            var value = property.GetValue(item);
                            if (value != null)
                            {
                                Cell cell = new()
                                {
                                    CellValue = new((string)value),
                                    DataType = CellValues.String,
                                };
                                row.Append(cell);
                            }
                        }

                        sheetData?.Append(row);
                    }
                }

                worksheetPart.Worksheet.Save();
                document.Dispose();

                await jSRuntime.SaveAsAsync($"{fileName}.xlsx", ms.ToArray());
            }
        }

        private async static Task ExportGridDataAsPdfAsync<TItem>(
            RadzenDataGrid<TItem> grid,
            IJSRuntime jSRuntime,
            string fileName,
            IEnumerable<string>? excludedColumns = default,
            CancellationToken cancellationToken = default)
        {
            var properties = GetDataGridProperties(grid, excludedColumns);
            var dataGridClassType = BuildAndGetDataGridType(properties);

            if (dataGridClassType != null)
            {
                var listType = typeof(List<>).MakeGenericType(dataGridClassType);
                var list = (IList?)Activator.CreateInstance(listType);

                var alldata = await grid.GetAllDataAsync(cancellationToken);

                foreach (var idata in alldata)
                {
                    var instance = Activator.CreateInstance(dataGridClassType);

                    foreach (var property in properties)
                    {

                        dataGridClassType.GetProperty(property.Name ?? "")?
                            .SetValue(instance, grid.ColumnsCollection.First(c => c.Property == property.Name).GetValue(idata).ToString());
                    }

                    list?.Add(instance);
                }

                var props = dataGridClassType.GetProperties();
                QuestPDF.Settings.License = LicenseType.Community;

                var document = Document.Create(container =>
                {

                    
                    container.Page(page =>
                    {
                        
                        page.Size(PageSizes.A4.Height, PageSizes.A4.Width);
                        page.DefaultTextStyle(x => x.FontSize(7));
                        page.Margin(10);

                        //// page header
                        //page.Header().Row(row =>
                        //{
                        //    row.ConstantItem(120).Image(logoBytes);
                        //    row.RelativeItem().Border(0)
                        //    .PaddingTop(5)
                        //    .Column(col =>
                        //    {
                        //        //col.Item().AlignCenter().MaxWidth(120).Image(logoBytes);
                        //        col.Item().AlignCenter().Text("MINISTERIO DE OBRAS Y SERVICIOS PUBLICOS").FontSize(14).Bold();
                        //        col.Item().AlignCenter().Text("Dirección General de Obras Públicas").FontSize(10);
                        //    });
                        //});

                        // page content
                        page.Content().Column(col =>
                        {
                            
                            col.Item().Table(tabla =>
                            {

                                tabla.ColumnsDefinition(columns =>
                                {
                                    //Cantidad de columnas
                                    foreach (var property in props)
                                    {
                                        columns.RelativeColumn();
                                    }

                                });

                                tabla.Header(header =>
                                {
                                    foreach (var property in props)
                                    {
                                        var descriptionAttribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(property, typeof(DescriptionAttribute));
                                        if (descriptionAttribute != null)
                                        {
                                            header.Cell()
                                                .Background("#dedede")
                                                .Padding(2)
                                                .Text(descriptionAttribute.Description);

                                        }

                                    }

                                });

                                //Filas de datos
                                if (list != null)
                                {
                                    foreach (var item in list)
                                    {
                                        foreach (var property in props)
                                        {
                                            var value = property.GetValue(item);
                                            if (value != null)
                                            {
                                                tabla.Cell()
                                                  .BorderBottom(1)
                                                  .BorderColor("#DEDEDE")
                                                  .Padding(2)
                                                  .Text((string)value);

                                            }
                                        }

                                    }
                                }

                            });

                        });

                    });


                });

                
                var pdf = document.GeneratePdf();

                using MemoryStream ms = new();
                await  ms.WriteAsync(pdf,0, pdf.Length);
                await jSRuntime.SaveAsAsync($"{fileName}.pdf", ms.ToArray());
            }
        }
    }
}
