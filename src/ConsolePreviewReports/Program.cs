using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using System.Runtime.CompilerServices;

using Business;
using DataTransferObject;

var document = Document.Create(container =>
{

    string logoPath = Path.Combine("E:\\Programacion\\GitServer\\GLP_RELI\\Website\\wwwroot\\images", "logo-celeste.png");
    byte[] logoBytes = File.ReadAllBytes(logoPath);
    
    DateTime FechaDeVencimiento = new DateTime(2021,9,24);
    DateTime fechaBalance = DateTime.Now.Date;
    DateTime fechaPeriodoAnterior = new DateTime(2023,8,31);

    string Empresa = "empresa prueba SA";

    decimal IndiceUVIFechaBalance = 99.84m;
    decimal IndiceUVIFechaUltimoDiaMesAnterior = 270.8m;
    decimal IndiceUVICoeficiente = 2.712339744m;

    List<TramitesBalancesGeneralesEvaluarConstanciaDTO> lstConstancia = new List<TramitesBalancesGeneralesEvaluarConstanciaDTO>();
    
    foreach (var item in Enumerable.Range(1, 20))
    {
        lstConstancia.Add(new TramitesBalancesGeneralesEvaluarConstanciaDTO
        {
            DescripcionSeccion = Placeholders.Label(),
            CapacidadTecnica = (decimal)Placeholders.Random.NextDouble() * Placeholders.Random.Next(100000, 999999999),
            CapacidadContratacion = (decimal)Placeholders.Random.NextDouble() * Placeholders.Random.Next(100000, 999999999),
            EjecucaionAnual = (decimal)Placeholders.Random.NextDouble() * Placeholders.Random.Next(100000, 999999999)
        });
    }

    container.Page(page =>
    {
        
        page.Margin(20);

        // page header
        page.Header().Row(row =>
        {
            row.ConstantItem(120).Image(logoBytes);
            row.RelativeItem().Border(0)
            .PaddingTop(5)
            .Column(col =>
            {
                //col.Item().AlignCenter().MaxWidth(120).Image(logoBytes);
                col.Item().AlignCenter().Text("MINISTERIO DE OBRAS Y SERVICIOS PUBLICOS").FontSize(14).Bold();
                col.Item().AlignCenter().Text("Dirección General de Obras Públicas").FontSize(10);
         
            });
        });

        // page content
        page.Content().Column(col =>
        {
            col.Item().AlignCenter()
             .PaddingTop(15)
             .Text("CERTIFICADO DE ACTUALIZACION DE CAPACIDAD")
             .FontSize(14)
             .Bold()
             .Underline();
            
            col.Item().AlignCenter()
             .Text("RESOLUCION 129/2022 MOySP")
             .FontSize(14)
             .Bold()
             .Underline();

            col.Item().PaddingTop(15).PaddingBottom(10).Row(row =>
            {
                row.RelativeItem()
                        .Text(txt =>
                        {
                            txt.Span($"EMPRESA: ").FontSize(10);
                            txt.Span($"{Empresa.ToUpper()}").FontSize(10).Bold();
                        });

                row.RelativeItem()
                        .AlignRight()
                        .Text(txt =>
                        {
                            txt.Span($"Balance: ").FontSize(10);
                            txt.Span($"{fechaBalance.ToString("dd/MM/yyyy")}").FontSize(10).Bold();
                        });
            });

            col.Item().Table(tabla =>
            {

                tabla.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(60);
                    columns.ConstantColumn(60);
                    columns.RelativeColumn();
                    columns.RelativeColumn();

                });

                
                //Tabla indices 
                //Ranglon 1
                tabla.Cell()
                    .BorderColor("#DEDEDE")
                    .Padding(2)
                    .Text("Indice UVI al")
                    .FontSize(9);

                tabla.Cell()
                    .Border(1)
                    .BorderColor("#DEDEDE")
                    .AlignCenter()
                    .Padding(2)
                    .Text(fechaBalance.ToString("dd/MM/yyyy"))
                    .FontSize(9);


                tabla.Cell()
                    .Border(1)
                    .BorderColor("#DEDEDE")
                    .Padding(2)
                    .Text(string.Format("{0:N2}", IndiceUVIFechaBalance))
                    .FontSize(9);

                tabla.Cell()
                    .BorderTop(1)
                    .BorderRight(1)
                    .BorderColor("#DEDEDE")
                    .AlignCenter()
                    .Padding(2)
                    .Text("Coeficiente UVI")
                    .FontSize(9);

                //Renglon 2
                tabla.Cell()
                    .BorderColor("#DEDEDE")
                    .Padding(2)
                    .Text("Indice UVI al")
                    .FontSize(9);

                tabla.Cell()
                    .Border(1)
                    .BorderColor("#DEDEDE")
                    .AlignCenter()
                    .Padding(2)
                    .Text(fechaPeriodoAnterior.ToString("dd/MM/yyyy"))
                    .FontSize(9);


                tabla.Cell()
                    .Border(1)
                    .BorderColor("#DEDEDE")
                    .Padding(2)
                    .Text(string.Format("{0:N2}", IndiceUVIFechaUltimoDiaMesAnterior))
                    .FontSize(9);

                tabla.Cell()
                    .BorderBottom(1)
                    .BorderRight(1) 
                    .BorderColor("#DEDEDE")
                    .AlignCenter()
                    .Padding(2)
                    .Text(string.Format("{0:N9}", IndiceUVICoeficiente))
                    .FontSize(9);

            });


            col.Item().PaddingTop(20).Table(tabla =>
            {
                
                tabla.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.ConstantColumn(150);
                    columns.ConstantColumn(150);
                    
                });

                tabla.Header(header =>
                {
                    header.Cell()
                        .Background("#dedede")
                        .Padding(2)
                        .Text("ESPECIALIDAD")
                        .FontSize(9);

                    header.Cell()
                        .Background("#dedede")
                        .Padding(2)
                        .AlignRight()
                        .Text("CAP. TECNICA")
                        .FontSize(9)
                        ;

                    header.Cell()
                        .Background("#dedede")
                        .Padding(2)
                        .AlignRight()
                        .Text("CAP. CONTRATACION")
                        .FontSize(9);

                });

                foreach(var item in lstConstancia)
                {
                    
                    tabla.Cell()
                        .BorderBottom(1)
                        .BorderColor("#DEDEDE")
                        .Padding(2)
                        .Text(item.DescripcionSeccion)
                        .FontSize(9);

                    tabla.Cell()
                        .BorderBottom(1)
                        .BorderColor("#DEDEDE")
                        .AlignRight()
                        .Padding(2)
                        .Text(string.Format("{0:C}", item.CapacidadTecnica))
                        .FontSize(9);

                    tabla.Cell()
                        .BorderBottom(1)
                        .BorderColor("#DEDEDE")
                        .AlignRight()
                        .Padding(2)
                        .Text(string.Format("{0:C}", item.CapacidadContratacion))
                        .FontSize(9);

                }
            });

            col.Item()
             .PaddingTop(10)
             .Text($"FECHA: {DateTime.Now.ToString("dd/MM/yyyy")}.")
             .FontSize(10);


            col.Item()
               .PaddingTop(10)
               .Text(txt =>
               {
                   txt.Span("El presente certificado es complementario del ").Italic().FontSize(10);
                   txt.Span("CERTIFICADO DE HABILITACION PARA LICITACION").Italic().Bold().FontSize(10);
                   txt.Span(" y deberán presentarse en forma conjunta, en cumplimiento de la Resolición Nº 129/2022 MOySP.").Italic().FontSize(10);
               });


        });
        // page footer
        //page.Footer().Height(100).Background(Colors.Grey.Lighten2);

    });

    
});

// instead of the standard way of generating a PDF file
//document.GeneratePdf("hello1.pdf");

// use the following invocation
document.ShowInPreviewer();
var pdf = document.GeneratePdf();

// optionally, you can specify an HTTP port to communicate with the previewer host (default is 12500)
//document.ShowInPreviewer(12345);
 