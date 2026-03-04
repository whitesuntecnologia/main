using Business;
using Business.Interfaces;
using DataTransferObject;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Radzen;
using StaticClass;
using System;
using System.Net.Mail;
using System.Text;
using Website.Models.Account;
using Website.Pages.Shared.Components;

namespace Website.Pages.Account
{
    public partial class SolicitudUsuario: ComponentBase
    {
        [Inject] private NotificationService notificationService { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        [Inject] private IEnvioMailBL _envioMailBL { get; set; } = null!;
        [Inject] private IParametrosBL _paramBL { get; set; } = null!;
        private AppSettings? _appSettings { get; set; }

        private SolicitudUsuarioModel Model { get; set; } = new();
        private CustomFileUpload? upload1;
        private List<FileDTO> lstFiles1 = new();
        private CustomFileUpload? upload2;
        private List<FileDTO> lstFiles2 = new();
        private CustomFileUpload? upload3;
        private List<FileDTO> lstFiles3 = new();

        private bool isBusyAceptar { get; set; }
        private bool solicitudEnviada { get; set; }
        private string? mailto { get; set; }
        private EditContext? context;
        

        public SolicitudUsuario()
        {

            var builder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();
            _appSettings = config.GetSection("AppSettings").Get<AppSettings>();
            
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
        protected void CancelarClick()
        {
            navigationManager.NavigateTo($"/Account/Login", true);
        }
        private async Task FileUploaded1(FileDTO fileModel)
        {

            if (upload1?.Accept?.Length > 0 && upload1.Accept != fileModel.ContentType)
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                Model.File1 = fileModel;
            }
            await ReValidate("File1");
        }
        private void FileDeletionRequested1(FileDTO fileModel)
        {
            Model.File1 = null;
            lstFiles1 = lstFiles1.Where(x => x != fileModel).ToList();
        }
        private async Task FileUploaded2(FileDTO fileModel)
        {

            if (upload2?.Accept?.Length > 0 && upload2.Accept != fileModel.ContentType)
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                Model.File2 = fileModel;
            }
            await ReValidate("File2");
        }
        private void FileDeletionRequested2(FileDTO fileModel)
        {
            Model.File2 = null;
            lstFiles2 = lstFiles2.Where(x => x != fileModel).ToList();
        }
        private async Task FileUploaded3(FileDTO fileModel)
        {

            if (upload3?.Accept?.Length > 0 && upload3.Accept != fileModel.ContentType)
            {
                notificationService.Notify(NotificationSeverity.Error, "Error", "El archivo seleccionado no posee el formato permitido", StaticClass.Constants.NotifDuration.Normal);
            }
            else
            {
                Model.File3 = fileModel;
            }
            await ReValidate("File3");
        }
        private void FileDeletionRequested3(FileDTO fileModel)
        {
            Model.File3 = null;
            lstFiles3 = lstFiles3.Where(x => x != fileModel).ToList();
        }
        private void FileError(string message)
        {
            notificationService.Notify(NotificationSeverity.Error, "Error", message, StaticClass.Constants.NotifDuration.Normal);
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
                    if(_appSettings?.isDesarrollo ?? false)
                    {
                        if (string.IsNullOrWhiteSpace(mailto))
                            throw new Exception("Debe ingresar la dirección de mail donde se recibirá el pedido.");
                        
                        mail.To.Add(mailto);
                    }
                    else
                    {
                        mailto = await _paramBL.GetParametroCharAsync("SolicitudUsuario.MailDestino");
                        if (string.IsNullOrWhiteSpace(mailto))
                            throw new Exception("No se encontró el parámetro con la dirección de destino del mail.");

                    }


                    mail.Subject = "RegLic - Pedido de usuario";
                    mail.IsBodyHtml = true;

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<h3 >Datos de la solicitud de usuario</h3>");
                    sb.AppendLine("<div>");
                    sb.AppendLine("<div>");
                    sb.AppendLine($"	<label>Apellido y Nombre:</label><strong>{Model.ApellidoyNombresSolicitante}</strong>");
                    sb.AppendLine("</div>");
                    sb.AppendLine("<div style=\"margin-top: 10px\">");
                    sb.AppendLine($"	<label>C.U.I.T. del solicitante:</label><strong>{Model.CuitSolicitante}</strong>");
                    sb.AppendLine("</div>");
                    sb.AppendLine("<div style=\"margin-top: 10px\">");
                    sb.AppendLine($"	<label>Razon Social:</label><strong>{Model.RazonSocial}</strong>");
                    sb.AppendLine("</div>");
                    sb.AppendLine("<div style=\"margin-top: 10px\">");
                    sb.AppendLine($"	<label>C.U.I.T. de la empresa:</label><strong>{Model.CuitEmpresa}</strong>");
                    sb.AppendLine("</div>");
                    sb.AppendLine("</div>");
                    mail.Body = sb.ToString();
                    List<Attachment> lstAdjuntos = new List<Attachment>();

                    if(Model.File1 != null)
                        mail.Attachments.Add(new Attachment(Model.File1.FilePath));
                    if (Model.File2 != null)
                        mail.Attachments.Add(new Attachment(Model.File2.FilePath));
                    if (Model.File3 != null)
                        mail.Attachments.Add(new Attachment(Model.File3.FilePath));

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
