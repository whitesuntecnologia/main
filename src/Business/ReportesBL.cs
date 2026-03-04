using AutoMapper;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using Microsoft.EntityFrameworkCore;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Repository;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnitOfWork;

namespace Business
{
    public class ReportesBL : IReportesBL
    {
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IMapper _mapper;
        private readonly IUsuariosBL _usuarioBL;
        private readonly ITramitesBL _tramitesBL;
        private readonly IIndiceUnidadViviendaBL _indiceUVIBL;

        public ReportesBL(IUnitOfWorkFactory uowFactory, ITramitesBL tramitesBL, IUsuariosBL usuariosBL, IIndiceUnidadViviendaBL indiceUVIBL)
        {
            _tramitesBL = tramitesBL;
            _usuarioBL = usuariosBL;
            _indiceUVIBL = indiceUVIBL;
            _uowFactory = uowFactory;
            QuestPDF.Settings.License = LicenseType.Community;


            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TramitesBalancesGeneralesEvaluarConstanciaCap, TramitesBalancesGeneralesEvaluarConstanciaDTO>().ReverseMap();
                cfg.CreateMap<TramitesRepresentantesTecnicos, TramitesRepresentanteTecnicoDTO>().ReverseMap();
                cfg.CreateMap<TramitesRepresentantesTecnicosJurisdicciones, TramitesRepresentanteTecnicoJurisdiccionDTO>()
                    .ForMember(dest => dest.DescripcionProvincia, opt => opt.MapFrom(src => src.IdProvinciaNavigation.Descripcion))
                ;
                cfg.CreateMap<TramitesRepresentantesTecnicosEspecialidades, TramitesRepresentanteTecnicoEspecilidadDTO>()
                   .ForMember(dest => dest.NombreEspecialidad, opt => opt.MapFrom(src => src.IdTramiteEspecialidadNavigation.IdEspecialidadNavigation.NombreEspecialidad))
                ;

            });
            _mapper = config.CreateMapper();


        }
        public async Task<byte[]> GetPdfReliInscripcion(int IdTramite, string wwwrootPath, bool previsualizar = true)
        {
            byte[] pdf = null;

            // Seccion de Carga de Datos

            string logoPath = Path.Combine(wwwrootPath, "images", "logo-celeste.png");
            byte[] logoBytes = await File.ReadAllBytesAsync(logoPath);

            DateTime fechaCertificado = DateTime.Now.Date;
            DateTime? fechaBalance = null;
            DateTime? fechaActualizacionUVI = null;

            var uow = _uowFactory.GetUnitOfWork();
            var repoTramites = new TramitesRepository(uow);
            var repoRepresentantes = new TramitesRepresentantesTecnicosRepository(uow);
            var repoBalance = new TramitesBalancesGeneralesRepository(uow);
            var repoBalanceEvaluar = new TramitesBalancesGeneralesEvaluarRepository(uow);
            var repoConstancia = new TramitesBalancesGeneralesEvaluarConstanciaCapRepository(uow);

            var tramiteEntity = await repoTramites.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);
            var balanceEvaluarEntity = await repoBalanceEvaluar.Where(x => x.IdTramiteBalanceGeneralNavigation.IdTramite == IdTramite)
                                                               .OrderByDescending(o=> o.IdTramiteBalanceGeneralEvaluar)
                                                               .FirstOrDefaultAsync();
            var lstConstanciaEntity = await repoConstancia.Where(x => x.IdTramiteBalanceGeneralEvaluarNavigation.IdTramiteBalanceGeneralEvaluar == balanceEvaluarEntity.IdTramiteBalanceGeneralEvaluar).ToListAsync();
            var lstRepresentantesEntity = await repoRepresentantes.Where(x => x.IdTramite == IdTramite).ToListAsync();
            var ultimoBalanceEntity = await repoBalance.Where(x => x.IdTramite == IdTramite).OrderByDescending(o => o.FechaBalance).FirstOrDefaultAsync();


            int EstadoDeudaBCRA = balanceEvaluarEntity.EstadoDeudaBcra;
            int EmpresaId = tramiteEntity.IdEmpresaNavigation.IdEmpresa;
            string Empresa = tramiteEntity.IdEmpresaNavigation.RazonSocial;

            if (ultimoBalanceEntity != null)
            {
                fechaBalance = ultimoBalanceEntity.FechaBalance;
            }
            //Ultimo día del mes anterior a la fecha de creación del trámite.
            fechaActualizacionUVI = new DateTime(tramiteEntity.CreateDate.Year, tramiteEntity.CreateDate.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(-1);

            List<TramitesBalancesGeneralesEvaluarConstanciaDTO> lstConstancia = _mapper.Map<List<TramitesBalancesGeneralesEvaluarConstanciaDTO>>(lstConstanciaEntity);

            List<string> lstRepresentantesTecnicos = new List<string>();

            foreach (var item in lstRepresentantesEntity)
            {
                var arrRamas = item.TramitesRepresentantesTecnicosEspecialidades.Select(s => s.IdTramiteEspecialidadNavigation.IdEspecialidadNavigation.Rama).Distinct().ToArray();
                string rama = "(" + string.Join(", ", arrRamas) + ")";
                string representanteTecnico = $"{rama} - {item.Apellido}, {item.Nombres} MAT. {item.Matricula} - Vig:{item.FechaVencimientoContrato.ToString("dd/MM/yyyy")}";
                lstRepresentantesTecnicos.Add(representanteTecnico);
            }
            var representantesTecnicos = string.Join(" - ", lstRepresentantesTecnicos);
            //-- fin de Carga de datos

            //Seccion de Impresion del PDF

            var document = Document.Create(container =>
            {


                container.Page(page =>
                {

                    page.Margin(20);

                    // page header
                    page.Header().Column(col =>
                    {
                        col.Item().Width(120).Image(logoBytes);
                        col.Item().AlignCenter().Text("MINISTERIO DE OBRAS Y SERVICIOS PUBLICOS").FontSize(14).Bold();
                        col.Item().AlignCenter().Text("DIRECCION DEL REGISTRO DE LICITADORES").FontSize(10);
                    });


                    // page content con marca de agua
                    page.Content().Layers(layers =>
                    {
                        // Capa principal con el contenido
                        layers.PrimaryLayer().Column(col =>
                        {
                            col.Item().PaddingTop(15).Row(row =>
                            {
                                row.RelativeItem()
                                        .Text("NO VALIDA PARA LICITACION")
                                        .FontSize(13)
                                        .Bold();


                                row.RelativeItem()
                                        .AlignRight()
                                        .Text("Form. 20")
                                        .FontSize(13)
                                        .Bold();
                            });

                            col.Item()
                                .AlignCenter()
                                .Text("CONSTANCIA DE CAPACIDAD")
                                .FontSize(13)
                                .Bold()
                                .Underline();

                            col.Item()
                                .PaddingTop(10)
                                .Text($"Certifico que la empresa {Empresa} se halla inscripta en el registro bajo el número {EmpresaId} y habilitada de la siguiente manera, con fecha de vencimiento el {tramiteEntity.FechaVencimiento?.ToString("d 'de' MMMM 'de' yyyy", new CultureInfo("es-AR"))}.")
                                .FontSize(10);

                            col.Item()
                                .PaddingTop(10)
                                .Text($"Representantes Técnicos")
                                .FontSize(13)
                                .Bold()
                                .Underline();


                            col.Item()
                                .PaddingTop(10)
                                .Text(representantesTecnicos)
                                .FontSize(9)
                                ;
                            col.Item()
                              .PaddingTop(10)
                              .Text($"Referencia")
                              .FontSize(9)
                              .Bold()
                              .Underline();

                            col.Item()
                                .Text($"Fecha de balance: {fechaBalance?.ToString("dd/MM/yyyy")}")
                                .FontSize(9);

                            col.Item()
                                .Text($"Fecha Actualización UVI: {fechaActualizacionUVI?.ToString("dd/MM/yyyy")}")
                                .FontSize(9);

                            col.Item()
                                .PaddingTop(10)
                                .Background("#dedede")
                                .AlignCenter()
                                .Text("CAPACIDAD")
                                .FontSize(10)
                                ;
                            col.Item().Table(tabla =>
                            {

                                tabla.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                tabla.Header(header =>
                                {
                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .Text("ESPECIALIDAD")
                                        .FontSize(8);

                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .AlignRight()
                                        .PaddingRight(10)
                                        .Text("TECNICA")
                                        .FontSize(8)
                                        ;

                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .AlignRight()
                                        .PaddingRight(12)
                                        .Text("ACT. UVI")
                                        .FontSize(8)
                                        ;

                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .AlignRight()
                                        .Text("CONTRATACION")
                                        .FontSize(8);

                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .AlignRight()
                                        .PaddingRight(15)
                                        .Text("ACT. UVI")
                                        .FontSize(8)
                                        ;

                                    header.Cell()
                                        .Background("#dedede")
                                        .PaddingLeft(2)
                                        .PaddingTop(2)
                                        .PaddingBottom(2)
                                        .AlignRight()
                                        .Text("EJECUCION ANUAL")
                                        .FontSize(8);

                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .AlignRight()
                                        .PaddingRight(15)
                                        .Text("ACT. UVI")
                                        .FontSize(8)
                                        ;
                                });

                                foreach (var item in lstConstancia)
                                {

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .Padding(2)
                                        .Text(item.DescripcionSeccion)
                                        .FontSize(8);

                                    tabla.Cell()
                                      .BorderBottom(1)
                                      .BorderColor("#DEDEDE")
                                      .AlignRight()
                                      .Padding(2)
                                      .Text(string.Format("{0:C}", item.CapacidadTecnica))
                                      .FontSize(8);

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .AlignRight()
                                        .Padding(2)
                                        .Text(string.Format("{0:C}", item.CapacidadTecnicaUvi))
                                        .FontSize(8);

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .AlignRight()
                                        .Padding(2)
                                        .Text(string.Format("{0:C}", item.CapacidadContratacion))
                                        .FontSize(8);

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .AlignRight()
                                        .Padding(2)
                                        .Text(string.Format("{0:C}", item.CapacidadContratacionUvi))
                                        .FontSize(8);

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .AlignRight()
                                        .Padding(2)
                                        .PaddingRight(4)
                                        .Text(string.Format("{0:C}", item.EjecucaionAnual))
                                        .FontSize(8);

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .AlignRight()
                                        .Padding(2)
                                        .Text(string.Format("{0:C}", item.EjecucaionAnualUvi))
                                        .FontSize(8);
                                }
                            });

                            col.Item()
                             .PaddingTop(10)
                             .Text($"SITUACION DE SISTEMA FINANCIERO: {EstadoDeudaBCRA}")
                             .FontSize(10)
                             .Bold()
                             ;

                            col.Item()
                             .Text($"Se extiende el presente certificado en la ciudad de Santa Rosa el {fechaCertificado.ToString("dd/MM/yyyy")} " +
                                   $"a solicitud del interesado/a y al solo efecto de: INFORMAR CAPACIDAD OTORGADA.")
                             .FontSize(10)
                             ;
                        });
                        // Capa de marca de agua
                        if (previsualizar)
                        {
                            layers.Layer()
                                .Extend() // ocupa toda la hoja    
                                .PaddingTop(50) // agrandás el bounding box
                                .PaddingLeft(80) // agrandás el bounding box
                                .PaddingRight(-150) // agrandás el bounding box
                                .AlignCenter()
                                .Rotate(55)
                                .Text("PREVISUALIZACIÓN")
                                .FontSize(60)
                                .FontColor(Colors.Black.WithAlpha(100));
                        }  
                    });
                    // page footer
                    //page.Footer().Height(100).Background(Colors.Grey.Lighten2);

                });


            });

            // instead of the standard way of generating a PDF file
            //document.GeneratePdf("hello1.pdf");

            // use the following invocation
            //document.ShowInPreviewer();
            pdf = document.GeneratePdf();

            // optionally, you can specify an HTTP port to communicate with the previewer host (default is 12500)
            //document.ShowInPreviewer(12345);


            return pdf;
        }
        public async Task<byte[]> GetPdfReliLicitar(int IdTramite, string wwwrootPath,bool previsualizar = true)
        {
            byte[] pdf = null;

            // Seccion de Carga de Datos

            string logoPath = Path.Combine(wwwrootPath, "images", "logo-celeste.png");
            byte[] logoBytes = await File.ReadAllBytesAsync(logoPath);

            DateTime? fechaBalance = null;
            DateTime? fechaActualizacionUVI = null;

            var uow = _uowFactory.GetUnitOfWork();
            var repoTramites = new TramitesRepository(uow);
            var repoRepresentantes = new TramitesRepresentantesTecnicosRepository(uow);
            var repoBalanceEvaluar = new TramitesBalancesGeneralesEvaluarRepository(uow);
            var repoBalance = new TramitesBalancesGeneralesRepository(uow);
            var repoConstancia = new TramitesBalancesGeneralesEvaluarConstanciaCapRepository(uow);

            var tramiteEntity = await repoTramites.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);
            var balanceEvaluarEntity = await repoBalanceEvaluar.Where(x => x.IdTramiteBalanceGeneralNavigation.IdTramite == IdTramite)
                                                               .OrderByDescending(o => o.IdTramiteBalanceGeneralEvaluar)
                                                               .FirstOrDefaultAsync();
            var lstConstanciaEntity = await repoConstancia.Where(x => x.IdTramiteBalanceGeneralEvaluarNavigation.IdTramiteBalanceGeneralEvaluar == balanceEvaluarEntity.IdTramiteBalanceGeneralEvaluar).ToListAsync();
            var lstRepresentantesEntity = await repoRepresentantes.Where(x => x.IdTramite == IdTramite).ToListAsync();
            var ultimoBalanceEntity = await repoBalance.Where(x => x.IdTramite == IdTramite).OrderByDescending(o => o.FechaBalance).FirstOrDefaultAsync();

            if (ultimoBalanceEntity != null)
            {
                fechaBalance = ultimoBalanceEntity.FechaBalance;
            }
            //Ultimo día del mes anterior a la fecha de creación del trámite.
            fechaActualizacionUVI = new DateTime(tramiteEntity.CreateDate.Year, tramiteEntity.CreateDate.Month, 1).AddDays(-1);

            string Empresa = tramiteEntity.IdEmpresaNavigation.RazonSocial;
            decimal CuitEmpresa = tramiteEntity.IdEmpresaNavigation.CuitEmpresa;
            var usuarioApoderado = await _usuarioBL.GetUserByIdAsync(tramiteEntity.IdEmpresaNavigation.UseridRepresentante);

            string Obra = tramiteEntity.IdObraPciaLpNavigation.ObraNombre;
            string Licitante = tramiteEntity.IdObraPciaLpNavigation.Licitante ?? string.Empty;

            string Sanciones = "";
            string Observaciones = "";
            decimal coeficienteConceptual = balanceEvaluarEntity.CoeficienteConceptual;
            string Concepto = Functions.GetConceptoFromCoef(coeficienteConceptual);

            //Observaciones
            if (tramiteEntity.SePresentaEnUte)
                Observaciones = $"SE PRESENTA EN UTE. PORCENTAJE DE PARTICIPACION {tramiteEntity.PorcParticipUte.GetValueOrDefault()}%";
            else
                Observaciones = "SE PRESENTA SOLA.";
            //--

            //Verifica si la empresa tiene sanciones vigentes
            if (tramiteEntity.IdEmpresaNavigation.EmpresasSanciones.Any(x=>  DateTime.Today >=  x.FechaDesdeSancion && (!x.FechaHastaSancion.HasValue || DateTime.Today <= x.FechaHastaSancion.Value))) 
                Sanciones = "SI";
            else
                Sanciones = "NO";
            //--



            List<TramitesBalancesGeneralesEvaluarConstanciaDTO> lstConstancia = _mapper.Map<List<TramitesBalancesGeneralesEvaluarConstanciaDTO>>(lstConstanciaEntity);
            List<string> lstRepresentantesTecnicos = new List<string>();

            foreach (var item in lstRepresentantesEntity)
            {
                var arrRamas = item.TramitesRepresentantesTecnicosEspecialidades.Select(s => s.IdTramiteEspecialidadNavigation.IdEspecialidadNavigation.Rama).Distinct().ToArray();
                string rama = "(" + string.Join(", ", arrRamas) + ")";
                string representanteTecnico = $"{rama} - {item.Apellido}, {item.Nombres} MAT. {item.Matricula} - Vig:{item.FechaVencimientoContrato.ToString("dd/MM/yyyy")}";
                lstRepresentantesTecnicos.Add(representanteTecnico);
            }
            var representantesTecnicos = string.Join(" - ", lstRepresentantesTecnicos);
            //-- fin de Carga de datos

            //Seccion de Impresion del PDF

            var document = Document.Create(container =>
            {


                container.Page(page =>
                {

                    page.Margin(20);

                    // page header
                    page.Header().Column(col =>
                    {
                        col.Item().Width(120).Image(logoBytes);
                        col.Item().AlignCenter().Text("MINISTERIO DE OBRAS Y SERVICIOS PUBLICOS").FontSize(14).Bold();
                        col.Item().AlignCenter().Text("DIRECCION DEL REGISTRO DE LICITADORES").FontSize(10);
                    });



                    // page content con marca de agua
                    page.Content().Layers(layers =>
                    {
                        // Capa principal con el contenido
                        layers.PrimaryLayer().Column(col =>
                        {
                            col.Item().AlignCenter()
                             .PaddingTop(10)
                             .Text("CERTIFICADO DE HABILITACION PARA LICITACION")
                             .FontSize(14)
                             .Bold()
                             .Underline();

                            //Nro de Inscripcion, EMPRESA y CUIT
                            col.Item().PaddingTop(10).Row(row =>
                            {

                                row.AutoItem().PaddingRight(20).Column(col =>
                                {
                                    col.Item().Text(txt =>
                                    {
                                        txt.Span("Nro. Inscrip.: ").FontSize(10);
                                        txt.Span($"{IdTramite}").Bold().FontSize(10);
                                    });

                                });

                                row.RelativeItem().PaddingLeft(20).Column(col =>
                                {
                                    col.Item().Text(txt =>
                                    {
                                        txt.Span($"EMPRESA: ").FontSize(10);
                                        txt.Span($"{Empresa.ToUpper()}").FontSize(10).Bold();
                                    });
                                    col.Item().Text(txt =>
                                    {
                                        txt.Span($"C.U.I.T.: ").FontSize(10);
                                        txt.Span($"{CuitEmpresa.ToString()}").FontSize(10).Bold();
                                    });
                                });
                            });

                            //Apoderado
                            col.Item().PaddingTop(10).Text(txt =>
                            {
                                txt.Span("APODERADO: ").FontSize(10);
                                txt.Span($"{usuarioApoderado.NombreyApellido}").FontSize(10).Bold();
                            });


                            col.Item()
                                .PaddingTop(10)
                                .Text($"Representantes Técnicos")
                                .FontSize(13)
                                .Bold()
                                .Underline();

                            col.Item()
                                .PaddingTop(10)
                                .Text(representantesTecnicos)
                                .FontSize(9);

                            col.Item()
                                .PaddingTop(10)
                                .Text($"Referencia")
                                .FontSize(9)
                                .Bold()
                                .Underline();

                            col.Item()
                                .Text($"Fecha de balance: {fechaBalance?.ToString("dd/MM/yyyy")}")
                                .FontSize(9);

                            col.Item()
                                .Text($"Fecha Actualización UVI: {fechaActualizacionUVI?.ToString("dd/MM/yyyy")}")
                                .FontSize(9);

                            col.Item()
                                .PaddingTop(10)
                                .Background("#dedede")
                                .AlignCenter()
                                .Text("CAPACIDAD")
                                .FontSize(10);

                            col.Item().Table(tabla =>
                            {

                                tabla.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                tabla.Header(header =>
                                {
                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .Text("ESPECIALIDAD")
                                        .FontSize(8);

                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .AlignRight()
                                        .PaddingRight(10)
                                        .Text("TECNICA")
                                        .FontSize(8)
                                        ;

                                    header.Cell()
                                       .Background("#dedede")
                                       .Padding(2)
                                       .AlignRight()
                                       .PaddingRight(12)
                                       .Text("ACT. UVI")
                                       .FontSize(8)
                                       ;

                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .AlignRight()
                                        .Text("CONTRATACION")
                                        .FontSize(8);

                                    header.Cell()
                                       .Background("#dedede")
                                       .Padding(2)
                                       .AlignRight()
                                       .PaddingRight(15)
                                       .Text("ACT. UVI")
                                       .FontSize(8)
                                       ;

                                });

                                foreach (var item in lstConstancia)
                                {

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .Padding(2)
                                        .Text(item.DescripcionSeccion)
                                        .FontSize(8);

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .AlignRight()
                                        .Padding(2)
                                        .Text(string.Format("{0:C}", item.CapacidadTecnica))
                                        .FontSize(8);

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .AlignRight()
                                        .Padding(2)
                                        .Text(string.Format("{0:C}", item.CapacidadTecnicaUvi))
                                        .FontSize(8);

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .AlignRight()
                                        .Padding(2)
                                        .Text(string.Format("{0:C}", item.CapacidadContratacion))
                                        .FontSize(8);

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .AlignRight()
                                        .Padding(2)
                                        .Text(string.Format("{0:C}", item.CapacidadContratacionUvi))
                                        .FontSize(8);

                                }
                            });

                            col.Item()
                             .PaddingTop(10)
                             .Text($"Habilitación según lo establecido en el Decreto 2546/93, art. 15.")
                             .FontSize(10);

                            col.Item()
                             .Text($"Se extiende el presente certificado el día {DateTime.Now.ToString("dd/MM/yyyy")} para ser presentado ante quien corresponda.")
                             .FontSize(10);

                            col.Item()
                             .PaddingTop(10)
                             .Text($"OBRA A LICITAR: {Obra.ToUpper()}.")
                             .FontSize(10);

                            col.Item()
                                .PaddingTop(2)
                                .Text(txt =>
                                {
                                    txt.Span($"LICITANTE DE LA OBRA:  ").FontSize(10);
                                    txt.Span(Licitante.ToUpper()).Bold().FontSize(10);
                                });

                            col.Item()
                                 .PaddingTop(2)
                                 .Text(txt =>
                                 {
                                     txt.Span($"CONCEPTO:  ").FontSize(10);
                                     txt.Span(Concepto).Bold().FontSize(10);
                                 });

                            col.Item()
                                 .PaddingTop(2)
                                 .Text(txt =>
                                 {
                                     txt.Span($"SANCIONES:  ").FontSize(10);
                                     txt.Span(Sanciones).Bold().FontSize(10);
                                 });

                            col.Item()
                                 .PaddingTop(2)
                                 .Text(txt =>
                                 {
                                     txt.Span($"OBSERVACIONES:  ").FontSize(10);
                                     txt.Span(Observaciones).Bold().FontSize(10);
                                 });
                        });


                        // Capa de marca de agua
                        if (previsualizar)
                        {
                            layers.Layer()
                                .Extend() // ocupa toda la hoja    
                                .PaddingTop(50) // agrandás el bounding box
                                .PaddingLeft(80) // agrandás el bounding box
                                .PaddingRight(-150) // agrandás el bounding box
                                .AlignCenter()
                                .Rotate(55)
                                .Text("PREVISUALIZACIÓN")
                                .FontSize(60)
                                .FontColor(Colors.Black.WithAlpha(100));
                        }
                    });
                    // page footer
                    //page.Footer().Height(100).Background(Colors.Grey.Lighten2);

                });


            });

            pdf = document.GeneratePdf();

            return pdf;
        }
        public async Task<byte[]> GetPdfReliActualizacion(int IdTramite, string wwwrootPath, bool previsualizar = true)
        {
            byte[] pdf = null;

            // Seccion de Carga de Datos

            string logoPath = Path.Combine(wwwrootPath, "images", "logo-celeste.png");
            byte[] logoBytes = await File.ReadAllBytesAsync(logoPath);

            DateTime fechaBalance = DateTime.Now.Date;
            DateTime fechaPeriodoAnterior = DateTime.Now.Date;
            decimal IndiceUVIFechaBalance = 0m;
            decimal IndiceUVIFechaUltimoDiaMesAnterior = 0m;
            decimal IndiceUVICoeficiente = 0m;


            var uow = _uowFactory.GetUnitOfWork();
            var repoTramites = new TramitesRepository(uow);
            var repoRepresentantes = new TramitesRepresentantesTecnicosRepository(uow);
            var repoBalanceEvaluar = new TramitesBalancesGeneralesEvaluarRepository(uow);
            var repoBalance = new TramitesBalancesGeneralesRepository(uow);
            var repoConstancia = new TramitesBalancesGeneralesEvaluarConstanciaCapRepository(uow);

            var tramiteEntity = await repoTramites.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);
            var balanceEvaluarEntity = await repoBalanceEvaluar.Where(x => x.IdTramiteBalanceGeneralNavigation.IdTramite == IdTramite)
                                                               .OrderByDescending(o => o.IdTramiteBalanceGeneralEvaluar)
                                                               .FirstOrDefaultAsync();
            var lstConstanciaEntity = await repoConstancia.Where(x => x.IdTramiteBalanceGeneralEvaluarNavigation.IdTramiteBalanceGeneralEvaluar == balanceEvaluarEntity.IdTramiteBalanceGeneralEvaluar).ToListAsync();
            var lstRepresentantesEntity = await repoRepresentantes.Where(x => x.IdTramite == IdTramite).ToListAsync();
            var ultimoBalanceEntity = await repoBalance.Where(x => x.IdTramite == IdTramite).OrderByDescending(o => o.FechaBalance).FirstOrDefaultAsync();

            if (ultimoBalanceEntity != null)
            {
                fechaBalance = ultimoBalanceEntity.FechaBalance;
                fechaPeriodoAnterior = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                fechaPeriodoAnterior = fechaPeriodoAnterior.AddDays(-1);
            }
            else
                throw new ArgumentException("No se encontró la fecha del último balance.");

            string Obra = tramiteEntity.IdObraPciaLpNavigation.ObraNombre;
            string Empresa = tramiteEntity.IdEmpresaNavigation.RazonSocial;

            var indiceFechaAnteriorDTO = await _indiceUVIBL.GetIndiceAsync(fechaPeriodoAnterior.Month, fechaPeriodoAnterior.Year);
            var indiceFechaBalanceDTO = await _indiceUVIBL.GetIndiceAsync(fechaBalance.Month, fechaBalance.Year);

            if (indiceFechaAnteriorDTO != null && indiceFechaBalanceDTO != null)
            {
                IndiceUVIFechaBalance = indiceFechaBalanceDTO.Valor;
                IndiceUVIFechaUltimoDiaMesAnterior = indiceFechaAnteriorDTO.Valor;
                if (IndiceUVIFechaBalance > 0)
                    IndiceUVICoeficiente = IndiceUVIFechaUltimoDiaMesAnterior / IndiceUVIFechaBalance;
            }
            else if (indiceFechaBalanceDTO == null)
                throw new ArgumentException($"No se encontró el UVI de la fecha de balance: {fechaBalance.ToString("dd/MM/yyyy")}.");
            else if (indiceFechaAnteriorDTO == null)
                throw new ArgumentException($"No se encontró el UVI correspondiente a la fecha del período anterior: {fechaPeriodoAnterior.ToString("dd/MM/yyyy")}.");

            List<TramitesBalancesGeneralesEvaluarConstanciaDTO> lstConstancia = _mapper.Map<List<TramitesBalancesGeneralesEvaluarConstanciaDTO>>(lstConstanciaEntity);
            List<string> lstRepresentantesTecnicos = new List<string>();

            foreach (var item in lstRepresentantesEntity)
            {
                var arrRamas = item.TramitesRepresentantesTecnicosEspecialidades.Select(s => s.IdTramiteEspecialidadNavigation.IdEspecialidadNavigation.Rama).Distinct().ToArray();
                string rama = "(" + string.Join(", ", arrRamas) + ")";
                string representanteTecnico = $"{rama} - {item.Apellido}, {item.Nombres} MAT. {item.Matricula} - Vig:{item.FechaVencimientoContrato.ToString("dd/MM/yyyy")}";
                lstRepresentantesTecnicos.Add(representanteTecnico);
            }
            //-- fin de Carga de datos

            //Seccion de Impresion del PDF

            var document = Document.Create(container =>
            {

                container.Page(page =>
                {

                    page.Margin(20);

                    // page header
                    page.Header().Column(col =>
                    {
                        col.Item().Width(120).Image(logoBytes);
                        col.Item().AlignCenter().Text("MINISTERIO DE OBRAS Y SERVICIOS PUBLICOS").FontSize(14).Bold();
                        col.Item().AlignCenter().Text("DIRECCION DEL REGISTRO DE LICITADORES").FontSize(10);
                    });



                    // page content con marca de agua
                    page.Content().Layers(layers =>
                    {
                        // Capa principal con el contenido
                        layers.PrimaryLayer().Column(col =>
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



                            //col.Item()
                            //    .PaddingTop(10)
                            //    .Background("#dedede")
                            //    .AlignCenter()
                            //    .Text("CAPACIDAD")
                            //    .FontSize(10)
                            //    ;
                            col.Item().PaddingTop(20).Table(tabla =>
                            {

                                tabla.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                tabla.Header(header =>
                                {
                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .Text("ESPECIALIDAD")
                                        .FontSize(8);

                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .AlignRight()
                                        .Text("CAP. TECNICA")
                                        .FontSize(8)
                                        ;
                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .AlignRight()
                                        .Text("ACT. UVI")
                                        .FontSize(8)
                                        ;

                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .AlignRight()
                                        .Text("CAP. CONTRATACION")
                                        .FontSize(8);

                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .AlignRight()
                                        .Text("ACT. UVI")
                                        .FontSize(8)
                                        ;

                                });

                                foreach (var item in lstConstancia)
                                {

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .Padding(2)
                                        .Text(item.DescripcionSeccion)
                                        .FontSize(8);

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .AlignRight()
                                        .Padding(2)
                                        .Text(string.Format("{0:C}", item.CapacidadTecnica))
                                        .FontSize(8);

                                    tabla.Cell()
                                       .BorderBottom(1)
                                       .BorderColor("#DEDEDE")
                                       .AlignRight()
                                       .Padding(2)
                                       .Text(string.Format("{0:C}", item.CapacidadTecnica * IndiceUVICoeficiente))
                                       .FontSize(8);

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .AlignRight()
                                        .Padding(2)
                                        .Text(string.Format("{0:C}", item.CapacidadContratacion))
                                        .FontSize(8);

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .AlignRight()
                                        .Padding(2)
                                        .Text(string.Format("{0:C}", item.CapacidadContratacion * IndiceUVICoeficiente))
                                        .FontSize(8);

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
                                   txt.Span("Obra:").Italic().FontSize(10);
                                   txt.Span($"{Obra}.").Italic().Bold().FontSize(10);
                               });


                            col.Item()
                               .PaddingTop(10)
                               .Text(txt =>
                               {
                                   txt.Span("El presente certificado es complementario del ").Italic().FontSize(10);
                                   txt.Span("CERTIFICADO DE HABILITACION PARA LICITACION").Italic().Bold().FontSize(10);
                                   txt.Span(" y deberán presentarse en forma conjunta, en cumplimiento de la Resolición Nº 129/2022 MOySP.").Italic().FontSize(10);
                               });
                        });

                        // Capa de marca de agua
                        if (previsualizar)
                        {
                            layers.Layer()
                                .Extend() // ocupa toda la hoja    
                                .PaddingTop(50) // agrandás el bounding box
                                .PaddingLeft(80) // agrandás el bounding box
                                .PaddingRight(-150) // agrandás el bounding box
                                .AlignCenter()
                                .Rotate(55)
                                .Text("PREVISUALIZACIÓN")
                                .FontSize(60)
                                .FontColor(Colors.Black.WithAlpha(100));
                        }
                    });


                });


            });

            pdf = document.GeneratePdf();

            return pdf;
        }
        public async Task<byte[]> GetPdfRecoInscripcion(int IdTramite, string wwwrootPath, bool previsualizar = true)
        {
            byte[] pdf = null;

            // Seccion de Carga de Datos

            string logoPath = Path.Combine(wwwrootPath, "images", "logo-celeste.png");
            byte[] logoBytes = await File.ReadAllBytesAsync(logoPath);

            DateTime fechaCertificado = DateTime.Now.Date;
            DateTime? fechaBalance = null;
            DateTime? fechaActualizacionUVI = null;

            var uow = _uowFactory.GetUnitOfWork();
            var repoTramites = new TramitesRepository(uow);
            var repoRepresentantes = new TramitesRepresentantesTecnicosRepository(uow);
            var repoBalance = new TramitesBalancesGeneralesRepository(uow);
            var repoBalanceEvaluar = new TramitesBalancesGeneralesEvaluarRepository(uow);


            var tramiteEntity = await repoTramites.FirstOrDefaultAsync(x => x.IdTramite == IdTramite);
            var balanceEvaluarEntity = await repoBalanceEvaluar.Where(x => x.IdTramiteBalanceGeneralNavigation.IdTramite == IdTramite)
                                                               .OrderByDescending(o => o.IdTramiteBalanceGeneralEvaluar)
                                                               .FirstOrDefaultAsync();
            var lstRepresentantesEntity = await repoRepresentantes.Where(x => x.IdTramite == IdTramite).ToListAsync();
            var ultimoBalanceEntity = await repoBalance.Where(x => x.IdTramite == IdTramite).OrderByDescending(o => o.FechaBalance).FirstOrDefaultAsync();


            int EstadoDeudaBCRA = balanceEvaluarEntity.EstadoDeudaBcra;
            string Empresa = tramiteEntity.IdEmpresaNavigation.RazonSocial;

            if (ultimoBalanceEntity != null)
            {
                fechaBalance = ultimoBalanceEntity.FechaBalance;
            }
            List<string> lstRepresentantesTecnicos = new List<string>();

            foreach (var item in lstRepresentantesEntity)
            {
                var arrRamas = item.TramitesRepresentantesTecnicosEspecialidades.Select(s => s.IdTramiteEspecialidadNavigation.IdEspecialidadNavigation.Rama).Distinct().ToArray();
                string rama = "(" + string.Join(", ", arrRamas) + ")";
                string representanteTecnico = $"{rama} - {item.Apellido}, {item.Nombres} MAT. {item.Matricula} - Vig:{item.FechaVencimientoContrato.ToString("dd/MM/yyyy")}";
                lstRepresentantesTecnicos.Add(representanteTecnico);
            }
            var representantesTecnicos = string.Join(" - ", lstRepresentantesTecnicos);
            var lstEspecialidades = tramiteEntity.TramitesEspecialidades
                                            .SelectMany(s => s.TramitesEspecialidadesSecciones)
                                            .Select(s => $"Obras Comp. " + s.IdSeccionNavigation.DescripcionSeccion).Distinct().ToList();

            //-- fin de Carga de datos

            //Seccion de Impresion del PDF

            var document = Document.Create(container =>
            {


                container.Page(page =>
                {

                    page.Margin(20);

                    // page header
                    page.Header().Column(col =>
                    {
                        col.Item().Width(120).PaddingBottom(10).Image(logoBytes);
                        col.Item().AlignCenter().Text("MINISTERIO DE OBRAS Y SERVICIOS PUBLICOS").FontSize(14).Bold();
                        col.Item().AlignCenter().Text("Dirección General de Obras Públicas").FontSize(10);
                        col.Item().AlignCenter().Text("Registro Permanente de Consultores de Obra Públicas").FontSize(10);
                    });


                    // page content con marca de agua
                    page.Content().Layers(layers =>
                    {
                        // Capa principal con el contenido
                        layers.PrimaryLayer().Column(col =>
                        {
                            col.Item()
                                .PaddingTop(15)
                                .AlignCenter()
                                .Text("CONSTANCIA DE INSCRIPCION")
                                .FontSize(13)
                                .Bold()
                                .Underline();

                            col.Item()
                                .PaddingTop(10)
                                .Text($"Certifico que la firma: {Empresa}, ha dado cumplimiento a lo establecido en la Resolución Nº 269/2010 del Ministerio de Obras y Servicios Públicos y se halla inscripta en el Registro de Licitadores y Consultores bajo el Número {tramiteEntity.IdTramite} que la habilita de la siguiente manera, con fecha de vencimiento el {tramiteEntity.FechaVencimiento?.ToString("dd/MM/yyyy", new CultureInfo("es-AR"))}.")
                                .FontSize(10);


                            //Tabla de Representantes Técnicos
                            col.Item().PaddingTop(15).Table(tabla =>
                            {

                                tabla.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                });

                                tabla.Header(header =>
                                {
                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .Text("Representantes Técnicos")
                                        .FontSize(8);
                                });

                                foreach (var item in lstRepresentantesTecnicos)
                                {

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .Padding(2)
                                        .Text(item)
                                        .FontSize(8);
                                }
                            });

                            //Tabla de Especialidades
                            col.Item().PaddingTop(15).Table(tabla =>
                            {

                                tabla.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                });

                                tabla.Header(header =>
                                {
                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .Text("Especialidad")
                                        .FontSize(8);
                                });

                                foreach (var item in lstEspecialidades)
                                {

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .Padding(2)
                                        .Text(item)
                                        .FontSize(8);
                                }
                            });

                            col.Item()
                             .PaddingTop(15)
                             .Text($"SITUACION DE SISTEMA FINANCIERO: {EstadoDeudaBCRA}")
                             .FontSize(10)
                             .Bold()
                             ;

                            col.Item()
                             .Text($"Se extiende el presente certificado en la ciudad de Santa Rosa el {fechaCertificado.ToString("dd/MM/yyyy")} " +
                                   $"a solicitud del interesado/a.")
                             .FontSize(10)
                             ;
                        });

                        // Capa de marca de agua
                        if (previsualizar)
                        {
                            layers.Layer()
                                .Extend() // ocupa toda la hoja    
                                .PaddingTop(50) // agrandás el bounding box
                                .PaddingLeft(80) // agrandás el bounding box
                                .PaddingRight(-150) // agrandás el bounding box
                                .AlignCenter()
                                .Rotate(55)
                                .Text("PREVISUALIZACIÓN")
                                .FontSize(60)
                                .FontColor(Colors.Black.WithAlpha(100));
                        }

                    });
                    // page footer
                    //page.Footer().Height(100).Background(Colors.Grey.Lighten2);

                });


            });

            // instead of the standard way of generating a PDF file
            //document.GeneratePdf("hello1.pdf");

            // use the following invocation
            //document.ShowInPreviewer();
            pdf = document.GeneratePdf();

            // optionally, you can specify an HTTP port to communicate with the previewer host (default is 12500)
            //document.ShowInPreviewer(12345);


            return pdf;
        }
        public async Task<byte[]> GetPdfReliActualizacionFromEmpresa(int IdEmpresa, string wwwrootPath, bool previsualizar = true)
        {
            byte[] pdf = null;

            // Seccion de Carga de Datos

            string logoPath = Path.Combine(wwwrootPath, "images", "logo-celeste.png");
            byte[] logoBytes = await File.ReadAllBytesAsync(logoPath);

            DateTime fechaCertificado = DateTime.Now.Date;
            DateTime fechaBalance;
            DateTime fechaActualizacionUVI = DateTime.Now.Date;
            decimal IndiceUVIFechaBalance = 0m;
            decimal IndiceUVIFechaUltimoDiaMesAnterior = 0m;
            decimal IndiceUVICoeficiente = 0m;

            var uow = _uowFactory.GetUnitOfWork();
            var repoEmpresas = new EmpresasRepository(uow);
            var repoRepresentantes = new EmpresasRepresentantesTecnicosRepository(uow);
            var repoBalance = new EmpresasBalancesGeneralesRepository(uow);
            var repoBalanceEvaluar = new EmpresasBalancesGeneralesEvaluarRepository(uow);
            var repoConstancia = new EmpresasConstanciaCapRepository(uow);

            var EmpresaEntity = await repoEmpresas.FirstOrDefaultAsync(x => x.IdEmpresa == IdEmpresa);
            var balanceEvaluarEntity = await repoBalanceEvaluar.Where(x => x.IdEmpresa == IdEmpresa)
                                                               .OrderByDescending(o => o.IdEmpresaBalanceGeneralEvaluar)
                                                               .FirstOrDefaultAsync();
            var lstConstanciaEntity = await repoConstancia.Where(x => x.IdEmpresa == IdEmpresa).ToListAsync();
            var lstRepresentantesEntity = await repoRepresentantes.Where(x => x.IdEmpresa == IdEmpresa).ToListAsync();
            var ultimoBalanceEntity = await repoBalance.Where(x => x.IdEmpresa == IdEmpresa).OrderByDescending(o => o.FechaBalance).FirstOrDefaultAsync();


            int EstadoDeudaBCRA = balanceEvaluarEntity.EstadoDeudaBcra;
            int EmpresaId = EmpresaEntity.IdEmpresa;
            string Empresa = EmpresaEntity.RazonSocial;

            if (ultimoBalanceEntity != null)
            {
                fechaBalance = ultimoBalanceEntity.FechaBalance;
                //Ultimo día del mes anterior a la fecha actual.
                fechaActualizacionUVI = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                fechaActualizacionUVI = fechaActualizacionUVI.AddDays(-1);
            }
            else
                throw new ArgumentException("No se encontró la fecha del último balance.");
            

            var indiceFechaActualizacionUvi = await _indiceUVIBL.GetIndiceAsync(fechaActualizacionUVI.Month, fechaActualizacionUVI.Year);
            var indiceFechaBalance = await _indiceUVIBL.GetIndiceAsync(fechaBalance.Month, fechaBalance.Year);

            if (indiceFechaActualizacionUvi != null && indiceFechaBalance != null)
            {
                IndiceUVIFechaBalance = indiceFechaBalance.Valor;
                IndiceUVIFechaUltimoDiaMesAnterior = indiceFechaActualizacionUvi.Valor;
                if (IndiceUVIFechaBalance > 0)
                    IndiceUVICoeficiente = IndiceUVIFechaUltimoDiaMesAnterior / IndiceUVIFechaBalance;
            }
            else if (indiceFechaBalance == null)
                throw new ArgumentException($"No se encontró el UVI de la fecha de balance: {fechaBalance.ToString("dd/MM/yyyy")}.");
            else if (indiceFechaActualizacionUvi == null)
                throw new ArgumentException($"No se encontró el UVI correspondiente a la fecha del período anterior: {fechaActualizacionUVI.ToString("dd/MM/yyyy")}.");


            List<string> lstRepresentantesTecnicos = new List<string>();

            foreach (var item in lstRepresentantesEntity)
            {
                var arrRamas = item.EmpresasRepresentantesTecnicosEspecialidades.Select(s => s.IdEmpresaEspecialidadNavigation.IdEspecialidadNavigation.Rama).Distinct().ToArray();
                string rama = "(" + string.Join(", ", arrRamas) + ")";
                string representanteTecnico = $"{rama} - {item.Apellido}, {item.Nombres} MAT. {item.Matricula} - Vig:{item.FechaVencimientoContrato.ToString("dd/MM/yyyy")}";
                lstRepresentantesTecnicos.Add(representanteTecnico);
            }
            var representantesTecnicos = string.Join(" - ", lstRepresentantesTecnicos);
            //-- fin de Carga de datos

            //Seccion de Impresion del PDF

            var document = Document.Create(container =>
            {


                container.Page(page =>
                {

                    page.Margin(20);

                    // page header
                    page.Header().Column(col =>
                    {
                        col.Item().Width(120).Image(logoBytes);
                        col.Item().AlignCenter().Text("MINISTERIO DE OBRAS Y SERVICIOS PUBLICOS").FontSize(14).Bold();
                        col.Item().AlignCenter().Text("DIRECCION DEL REGISTRO DE LICITADORES").FontSize(10);
                    });


                    // page content con marca de agua
                    page.Content().Layers(layers =>
                    {
                        // Capa principal con el contenido
                        layers.PrimaryLayer().Column(col =>
                        {
                            col.Item().PaddingTop(15).Row(row =>
                            {
                                row.RelativeItem()
                                        .Text("NO VALIDA PARA LICITACION")
                                        .FontSize(13)
                                        .Bold();


                                row.RelativeItem()
                                        .AlignRight()
                                        .Text("Form. 20")
                                        .FontSize(13)
                                        .Bold();
                            });

                            col.Item()
                                .AlignCenter()
                                .Text("CONSTANCIA DE CAPACIDAD")
                                .FontSize(13)
                                .Bold()
                                .Underline();

                            col.Item()
                                .PaddingTop(10)
                                .Text($"Certifico que la empresa {Empresa} se halla inscripta en el registro bajo el número {EmpresaId} y habilitada.")
                                .FontSize(10);

                            col.Item()
                                .PaddingTop(10)
                                .Text($"Representantes Técnicos")
                                .FontSize(13)
                                .Bold()
                                .Underline();


                            col.Item()
                                .PaddingTop(10)
                                .Text(representantesTecnicos)
                                .FontSize(9)
                                ;
                            col.Item()
                              .PaddingTop(10)
                              .Text($"Referencia")
                              .FontSize(9)
                              .Bold()
                              .Underline();

                            col.Item()
                                .Text($"Fecha de balance: {fechaBalance.ToString("dd/MM/yyyy")}")
                                .FontSize(9);

                            col.Item()
                                .Text($"Fecha Actualización UVI: {fechaActualizacionUVI.ToString("dd/MM/yyyy")}")
                                .FontSize(9);

                            col.Item()
                                .PaddingTop(10)
                                .Background("#dedede")
                                .AlignCenter()
                                .Text("CAPACIDAD")
                                .FontSize(10)
                                ;
                            col.Item().Table(tabla =>
                            {

                                tabla.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                tabla.Header(header =>
                                {
                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .Text("ESPECIALIDAD")
                                        .FontSize(8);

                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .AlignRight()
                                        .PaddingRight(10)
                                        .Text("TECNICA")
                                        .FontSize(8)
                                        ;

                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .AlignRight()
                                        .PaddingRight(12)
                                        .Text("ACT. UVI")
                                        .FontSize(8)
                                        ;

                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .AlignRight()
                                        .Text("CONTRATACION")
                                        .FontSize(8);

                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .AlignRight()
                                        .PaddingRight(15)
                                        .Text("ACT. UVI")
                                        .FontSize(8)
                                        ;

                                    header.Cell()
                                        .Background("#dedede")
                                        .PaddingLeft(2)
                                        .PaddingTop(2)
                                        .PaddingBottom(2)
                                        .AlignRight()
                                        .Text("EJECUCION ANUAL")
                                        .FontSize(8);

                                    header.Cell()
                                        .Background("#dedede")
                                        .Padding(2)
                                        .AlignRight()
                                        .PaddingRight(15)
                                        .Text("ACT. UVI")
                                        .FontSize(8)
                                        ;
                                });

                                foreach (var item in lstConstanciaEntity)
                                {

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .Padding(2)
                                        .Text(item.DescripcionSeccion)
                                        .FontSize(8);

                                    tabla.Cell()
                                      .BorderBottom(1)
                                      .BorderColor("#DEDEDE")
                                      .AlignRight()
                                      .Padding(2)
                                      .Text(string.Format("{0:C}", item.CapacidadTecnica))
                                      .FontSize(8);

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .AlignRight()
                                        .Padding(2)
                                        .Text(string.Format("{0:C}", item.CapacidadTecnica * IndiceUVICoeficiente))
                                        .FontSize(8);

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .AlignRight()
                                        .Padding(2)
                                        .Text(string.Format("{0:C}", item.CapacidadContratacion))
                                        .FontSize(8);

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .AlignRight()
                                        .Padding(2)
                                        .Text(string.Format("{0:C}", item.CapacidadContratacion * IndiceUVICoeficiente))
                                        .FontSize(8);

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .AlignRight()
                                        .Padding(2)
                                        .PaddingRight(4)
                                        .Text(string.Format("{0:C}", item.EjecucaionAnual))
                                        .FontSize(8);

                                    tabla.Cell()
                                        .BorderBottom(1)
                                        .BorderColor("#DEDEDE")
                                        .AlignRight()
                                        .Padding(2)
                                        .Text(string.Format("{0:C}", item.EjecucaionAnual * IndiceUVICoeficiente))
                                        .FontSize(8);
                                }
                            });

                            col.Item()
                             .PaddingTop(10)
                             .Text($"SITUACION DE SISTEMA FINANCIERO: {EstadoDeudaBCRA}")
                             .FontSize(10)
                             .Bold()
                             ;

                            col.Item()
                             .Text($"Se extiende el presente certificado en la ciudad de Santa Rosa el {fechaCertificado.ToString("dd/MM/yyyy")} " +
                                   $"a solicitud del interesado/a y al solo efecto de: INFORMAR CAPACIDAD ACTUALIZADA.")
                             .FontSize(10)
                             ;
                        });
                        // Capa de marca de agua
                        if (previsualizar)
                        {
                            layers.Layer()
                                .Extend() // ocupa toda la hoja    
                                .PaddingTop(50) // agrandás el bounding box
                                .PaddingLeft(80) // agrandás el bounding box
                                .PaddingRight(-150) // agrandás el bounding box
                                .AlignCenter()
                                .Rotate(55)
                                .Text("PREVISUALIZACIÓN")
                                .FontSize(60)
                                .FontColor(Colors.Black.WithAlpha(100));
                        }
                    });
                    // page footer
                    //page.Footer().Height(100).Background(Colors.Grey.Lighten2);

                });


            });

            // instead of the standard way of generating a PDF file
            //document.GeneratePdf("hello1.pdf");

            // use the following invocation
            //document.ShowInPreviewer();
            pdf = document.GeneratePdf();

            // optionally, you can specify an HTTP port to communicate with the previewer host (default is 12500)
            //document.ShowInPreviewer(12345);


            return pdf;
        }
    }
}
