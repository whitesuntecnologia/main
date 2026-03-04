using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using System.Net.Mail;
using System.Text;
using Website.Models.Formulario;
using Website.Pages.Shared.Components;

namespace Website.Pages.Tramites
{
    public partial class SolicitudObraParaLicitar: ComponentBase
    {

        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private IEnvioMailBL _envioMailBL { get; set; } = null!;
        [Inject] private IParametrosBL _paramBL { get; set; } = null!;
        

        private AppSettings? _appSettings { get; set; } = null!;
        private SolicitudObraParaLicitarModel Model { get; set; } = new();
        private CustomFileUpload? upload1;
        private List<FileDTO> lstFiles1 = new();
        private CustomFileUpload? upload2;

        private bool isBusyAceptar;
        private EditContext context = null!;
        private bool solicitudEnviada { get; set; }
        private string? mailto { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var builder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();
            _appSettings = config.GetSection("AppSettings").Get<AppSettings>();

            context = new EditContext(Model);
            await base.OnInitializedAsync();
        }
        private Task ReValidate(string fieldName)
        {
            if (context != null)
            {
                FieldIdentifier field = context.Field(fieldName);
                context?.NotifyFieldChanged(field);
            }
            return Task.CompletedTask;
        }
        private async Task FileUploaded1(FileDTO fileModel)
        {

            if (upload1?.Accept?.Length > 0 && upload1.Accept != fileModel.ContentType)
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                Model.File = fileModel;
            }
            await ReValidate("File1");
        }
        private void FileDeletionRequested1(FileDTO fileModel)
        {
            Model.File = null;
            lstFiles1 = lstFiles1.Where(x => x != fileModel).ToList();
        }
        private void FileError(string message)
        {
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
        }
        protected void CancelarClick()
        {
            navigationManager.NavigateTo($"/Tramites/IniciarCertificadoLicitar", true);
        }
        protected async Task OnClickAceptar(EditContext ed)
        {

            if (isBusyAceptar)
                return;

            context = ed;
            isBusyAceptar = true;

            if (ed.Validate())
            {
                try
                {

                    MailMessage mail = new MailMessage();
                    if (_appSettings?.isDesarrollo ?? false)
                    {
                        if (string.IsNullOrWhiteSpace(mailto))
                            throw new Exception("Debe ingresar la dirección de mail donde se recibirá el pedido.");

                        mail.To.Add(mailto);
                    }
                    else
                    {
                        mailto = await _paramBL.GetParametroCharAsync("Mail.Administrador");
                        if (string.IsNullOrWhiteSpace(mailto))
                            throw new Exception("No se encontró el parámetro con la dirección de destino del mail.Parametro Mail.Administrador");

                    }


                    mail.Subject = "RegLic - Pedido de usuario";
                    mail.IsBodyHtml = true;

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<h3 >Datos de la solicitud de Obra para Licitar</h3>");
                    sb.AppendLine("<div>");
                    sb.AppendLine("<div>");
                    sb.AppendLine($"	<label>Denominación de la Obra:</label><strong>{Model.NombreObra}</strong>");
                    sb.AppendLine("</div>");
                    
                    if (Model.FechaLicitacion.HasValue)
                    {
                        sb.AppendLine("<div style=\"margin-top: 10px\">");
                        sb.AppendLine($"	<label>Fecha de Licitación:</label><strong>{Model.FechaLicitacion.Value.ToString("dd/MM/yyyy")}</strong>");
                        sb.AppendLine("</div>");
                    }
                    
                    sb.AppendLine("<div style=\"margin-top: 10px\">");
                    sb.AppendLine($"	<label>Expediente:</label><strong>{Model.NroExpediente}</strong>");
                    sb.AppendLine("</div>");
                    sb.AppendLine("</div>");
                    mail.Body = sb.ToString();
                    List<Attachment> lstAdjuntos = new List<Attachment>();

                    if (Model.File != null)
                        mail.Attachments.Add(new Attachment(Model.File.FilePath));

                    await _envioMailBL.EnviarMail(mail);
                    solicitudEnviada = true;
                }
                catch (Exception ex)
                {
                    notificationService.Notify(NotificationSeverity.Error, "Aviso", Functions.GetErrorMessage(ex), StaticClass.Constants.NotifDuration.Normal);
                }
            }

            isBusyAceptar = false;
        }
       
    }
}
