
using Business.Interfaces;
using DataTransferObject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace Business
{
    public class EnvioMailBL: IEnvioMailBL
    {
        private readonly MailSettings _mailSettings;
        public EnvioMailBL(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task<bool> EnviarMail(MailMessage message)
        {
            bool result = false;

            try
            {
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress("GLP - Sistema de Registro", _mailSettings.MailAdressFrom));

                foreach (var address in message.To)
                {
                    mimeMessage.To.Add(MailboxAddress.Parse(address.Address));
                }

                if (message.CC?.Count > 0)
                {
                    foreach (var address in message.CC)
                    {
                        mimeMessage.Cc.Add(MailboxAddress.Parse(address.Address));
                    }
                }

                if (message.Bcc?.Count > 0)
                {
                    foreach (var address in message.Bcc)
                    {
                        mimeMessage.Bcc.Add(MailboxAddress.Parse(address.Address));
                    }
                }

                mimeMessage.Subject = message.Subject;

                var bodyBuilder = new BodyBuilder();
                if (message.IsBodyHtml)
                {
                    bodyBuilder.HtmlBody = message.Body;
                }
                else
                {
                    bodyBuilder.TextBody = message.Body;
                }

                if (message.Attachments?.Count > 0)
                {
                    foreach (var attachment in message.Attachments)
                    {
                        bodyBuilder.Attachments.Add(attachment.Name, attachment.ContentStream);
                    }
                }

                mimeMessage.Body = bodyBuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                    await client.ConnectAsync(_mailSettings.ServidorSMTP, _mailSettings.Port, 
                        _mailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                    if (!string.IsNullOrEmpty(_mailSettings.User) && !string.IsNullOrEmpty(_mailSettings.Password))
                    {
                        await client.AuthenticateAsync(_mailSettings.User, _mailSettings.Password);
                    }

                    await client.SendAsync(mimeMessage);
                    await client.DisconnectAsync(true);

                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("No se ha podido enviar el mail", ex);
            }

            return result;
        }
    }
}
