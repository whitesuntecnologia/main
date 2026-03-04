using AutoMapper;
using BlazorPro.Spinkit;
using Business;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DataTransferObject.BLs;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using StaticClass;
using System.Data;
using Website.Extensions;
using Website.Models.Consultas;
using Website.Models.Formulario;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using static StaticClass.Constants;

namespace Website.Pages.Consultas
{
    public partial class CapacidadesxEmpresa: ComponentBase
    {
        private CapacidadesxEmpresaModel Model { get; set; } = new();
        [Inject] private DialogService DS { get; set; } = null!;
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private IWebHostEnvironment WebHostEnvironment { get; set; } = null!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private ICombosBL CombosBL { get; set; } = null!;
        [Inject] private IEmpresasBL EmpresasBL { get; set; } = null!;
        [Inject] private IReportesBL _reportesBL { get; set; } = null!;
        private IMapper _mapper { get; set; } = null!;
        private List<InformeCapacidadesxEmpresaDtoFlat> lstResultado = new();
        private List<EspecialidadInfo> lstEspecialidades = new();
        private RadzenDataGrid<InformeCapacidadesxEmpresaDtoFlat> grdResultado = null!;
        private bool IsBusyBuscar { get; set; } = false;
        
        protected override async Task OnInitializedAsync()
        {
            Model.Especialidades = (await CombosBL.GetEspecialidadesAsync()).ToList();
            Model.Especialidades.Insert(0, new GenericComboDTO { Id = 0, Descripcion = "(Todas)" });

            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CapacidadesxEmpresaModel, FiltroCapacidadesxEmpresaDto>();

            });
            _mapper = config.CreateMapper();
            //--
            await base.OnInitializedAsync();
        }


        protected async Task OnClickBuscar(EditContext ed)
        {
            if (IsBusyBuscar)
                return;

            IsBusyBuscar = true;
            if (ed.Validate())
            {
                try
                {
                    var filtroDto = _mapper.Map<FiltroCapacidadesxEmpresaDto>(Model);
                    var resultado = await EmpresasBL.GetCapacidadesxEmpresaFlatAsync(filtroDto);
                    lstResultado = resultado.datos;
                    lstEspecialidades = resultado.especialidades;
                }
                catch (Exception ex)
                {
                    IsBusyBuscar = false;
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                }
            }
            else
            {
                notificationService.Notify(NotificationSeverity.Error, "Aviso", "Revise las validaciones en pantalla", StaticClass.Constants.NotifDuration.Normal);
            }
            IsBusyBuscar = false;
        }

        private async Task LimpiarClick()
        {
            Model = new();
            lstResultado = new();
            grdResultado.Reset();
            await grdResultado.FirstPage(true);
        }

        private async Task ExportarExcel()
        {
            try
            {
                // Crear DataTable para exportar
                var dt = new DataTable("Capacidades por Empresa");

                // Agregar columnas fijas
                dt.Columns.Add("ID", typeof(int));
                dt.Columns.Add("CUIT", typeof(string));
                dt.Columns.Add("Razón Social", typeof(string));
                dt.Columns.Add("Domicilio", typeof(string));
                dt.Columns.Add("Fecha Inscripción", typeof(string));
                dt.Columns.Add("Vencimiento", typeof(string));

                // Agregar columnas dinámicas por especialidad
                foreach (var especialidad in lstEspecialidades)
                {
                    dt.Columns.Add($"{especialidad.Descripcion} - Cap. Técnica", typeof(string));
                    dt.Columns.Add($"{especialidad.Descripcion} - Cap. Técnica UVI", typeof(string));
                    dt.Columns.Add($"{especialidad.Descripcion} - Cap. Técnica UVI Actualizada", typeof(string));
                    dt.Columns.Add($"{especialidad.Descripcion} - Cap. Contratación", typeof(string));
                    dt.Columns.Add($"{especialidad.Descripcion} - Cap. Contratación UVI", typeof(string));
                    dt.Columns.Add($"{especialidad.Descripcion} - Cap. Contratación UVI Actualizada", typeof(string));
                    dt.Columns.Add($"{especialidad.Descripcion} - Cap. Ejecución", typeof(string));
                    dt.Columns.Add($"{especialidad.Descripcion} - Cap. Ejecución UVI", typeof(string));
                    dt.Columns.Add($"{especialidad.Descripcion} - Cap. Ejecución UVI Actualizada", typeof(string));
                }

                // Agregar filas con datos
                foreach (var empresa in lstResultado)
                {
                    var row = dt.NewRow();
                    row["ID"] = empresa.IdEmpresa;
                    row["CUIT"] = empresa.CuitEmpresa;
                    row["Razón Social"] = empresa.RazonSocial;
                    row["Domicilio"] = empresa.Domicilio;
                    row["Fecha Inscripción"] = empresa.FechaInscripcion.HasValue
                        ? empresa.FechaInscripcion.Value.ToString("dd/MM/yyyy")
                        : "-";
                    row["Vencimiento"] = empresa.Vencimiento.HasValue
                        ? empresa.Vencimiento.Value.ToString("dd/MM/yyyy")
                        : "-";

                    // Agregar datos de capacidades por especialidad
                    foreach (var especialidad in lstEspecialidades)
                    {
                        var ct = empresa.GetCapacidad($"{especialidad.IdSeccion}-{especialidad.Descripcion}", "CT");
                        var ctu = empresa.GetCapacidad($"{especialidad.IdSeccion}-{especialidad.Descripcion}", "CTU");
                        var ctua = empresa.GetCapacidad($"{especialidad.IdSeccion}-{especialidad.Descripcion}", "CTUA");
                        var cc = empresa.GetCapacidad($"{especialidad.IdSeccion}-{especialidad.Descripcion}", "CC");
                        var ccu = empresa.GetCapacidad($"{especialidad.IdSeccion}-{especialidad.Descripcion}", "CCU");
                        var ccua = empresa.GetCapacidad($"{especialidad.IdSeccion}-{especialidad.Descripcion}", "CCUA");
                        var ce = empresa.GetCapacidad($"{especialidad.IdSeccion}-{especialidad.Descripcion}", "CE");
                        var ceu = empresa.GetCapacidad($"{especialidad.IdSeccion}-{especialidad.Descripcion}", "CEU");
                        var ceua = empresa.GetCapacidad($"{especialidad.IdSeccion}-{especialidad.Descripcion}", "CEUA");

                        row[$"{especialidad.Descripcion} - Cap. Técnica"] = ct > 0 ? ct.ToString("N2") : "-";
                        row[$"{especialidad.Descripcion} - Cap. Técnica UVI"] = ctu > 0 ? ctu.ToString("N2") : "-";
                        row[$"{especialidad.Descripcion} - Cap. Técnica UVI Actualizada"] = ctua > 0 ? ctua.ToString("N2") : "-";
                        row[$"{especialidad.Descripcion} - Cap. Contratación"] = cc > 0 ? cc.ToString("N2") : "-";
                        row[$"{especialidad.Descripcion} - Cap. Contratación UVI"] = ccu > 0 ? ccu.ToString("N2") : "-";
                        row[$"{especialidad.Descripcion} - Cap. Contratación UVI Actualizada"] = ccua > 0 ? ccua.ToString("N2") : "-";
                        row[$"{especialidad.Descripcion} - Cap. Ejecución"] = ce > 0 ? ce.ToString("N2") : "-";
                        row[$"{especialidad.Descripcion} - Cap. Ejecución UVI"] = ceu > 0 ? ceu.ToString("N2") : "-";
                        row[$"{especialidad.Descripcion} - Cap. Ejecución UVI Actualizada"] = ceua > 0 ? ceua.ToString("N2") : "-";
                    }

                    dt.Rows.Add(row);
                }

                // Convertir DataTable a bytes de Excel
                var excelBytes = ConvertDataTableToExcel(dt);

                // Descargar archivo
                await JSRuntime.SaveAsAsync($"CapacidadesxEmpresas.xlsx", excelBytes);

            }
            catch (Exception ex)
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", $"Error al exportar: {Functions.GetErrorMessage(ex)}");
            }
        }

        private byte[] ConvertDataTableToExcel(DataTable dt)
        {
            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Capacidades");

                // Agregar encabezados manualmente
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var cell = worksheet.Cell(1, i + 1);
                    cell.Value = dt.Columns[i].ColumnName;
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.FontColor = ClosedXML.Excel.XLColor.Black;
                    cell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                    cell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                    cell.Style.Alignment.Vertical = ClosedXML.Excel.XLAlignmentVerticalValues.Center;
                    cell.Style.Alignment.WrapText = true;
                }

                // Agregar datos
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        var cell = worksheet.Cell(i + 2, j + 1);
                        var valor = dt.Rows[i][j].ToString();

                        // Columnas 7+ (capacidades): formato numérico
                        if (j >= 6) // Columna 7 en adelante (índice 6+)
                        {
                            // Intentar parsear como decimal
                            if (decimal.TryParse(valor, out decimal numeroDecimal))
                            {
                                cell.Value = numeroDecimal;
                                cell.Style.NumberFormat.Format = "#,##0.00";
                                cell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Right;
                            }
                            else
                            {
                                cell.Value = valor; // "-" u otro valor no numérico
                                cell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                            }
                        }
                        else
                        {
                            cell.Value = valor;
                        }
                    }
                }

                // Ajustar ancho de columnas
                // Primeras 6 columnas: ajuste automático
                for (int i = 1; i <= 6 && i <= dt.Columns.Count; i++)
                {
                    worksheet.Column(i).AdjustToContents();
                }

                // Columnas de especialidades (después de Vencimiento): ancho fijo de 25 caracteres (≈200px)
                for (int i = 7; i <= dt.Columns.Count; i++)
                {
                    worksheet.Column(i).Width = 25;
                }


                // Congelar primera fila
                worksheet.SheetView.FreezeRows(1);

                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        private async Task DescargarCertificadoClick(InformeCapacidadesxEmpresaDtoFlat row)
        {
            string wwwrootPath = WebHostEnvironment.WebRootPath;
            var pdf = await _reportesBL.GetPdfReliActualizacionFromEmpresa(row.IdEmpresa, wwwrootPath,false);
            string fileName = $"Certificado-Actualizacion-RELI-Empresa-{row.CuitEmpresa}.pdf";
            string contentType = "application/octet-stream";
            await JSRuntime.InvokeVoidAsync("BlazorDownloadFile", fileName, contentType, pdf);
        }

    }
}
