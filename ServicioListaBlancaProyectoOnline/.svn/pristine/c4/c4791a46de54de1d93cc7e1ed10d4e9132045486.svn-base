using Sibo.WhiteList.Service.BLL.Log;
using Sibo.WhiteList.Service.Entities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Sibo.WhiteList.Service.BLL.Helpers
{
    public class SendMail
    {
        public bool sendContract(string PDFPath, string receptor)
        {
            try
            {
                AccessControlSettingsBLL acsBLL = new AccessControlSettingsBLL();
                eConfiguration config = new eConfiguration();
                config = acsBLL.GetLocalAccessControlSettings();
                bool response = false;              

                if (config != null)
                {
                    string subject = "Contrato";
                    string template = string.Empty;
                    response = SendContractEmail(config.user, config.password, subject, template, receptor, PDFPath, config.SMTPServer, config.port);
                }

                deletePDF(PDFPath);
                return response;
            }
            catch (Exception ex)
            {
                ServiceLog log = new ServiceLog();
                log.WriteProcess("Ocurrió un error al enviar el email con el contrato adjunto. Por favor revisar el log de errores.");
                log.WriteError("Error realizando el envío el email con el contrato adjunto. Error - " + ex.Message + " - " + ex.StackTrace.ToString());
                return false;
            }
        }

        public bool SendContractEmail(string user, string password, string subject, string bodyMail, string receptorMail, string PDFPath, string SMTPServer, string port)
        {
            try
            {
                string ServidorSMTP = string.Empty, UsrEmail = string.Empty, passEmail = string.Empty, txtTextoEmail = string.Empty;;
                MailAddress from, to;
                SmtpClient server;

                using (MailMessage msg = new MailMessage())
                {
                    from = new MailAddress(user);
                    msg.From = from;
                    msg.IsBodyHtml = false;
                    msg.Subject = subject;
                    msg.Body = bodyMail;
                    server = new SmtpClient(SMTPServer);
                    server.EnableSsl = true;
                    server.Credentials = new NetworkCredential(user, password);
                    to = new MailAddress(receptorMail);
                    msg.Bcc.Add(to);
                    server.Port = Convert.ToInt32(port);
                    server.Send(msg);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ServiceLog log = new ServiceLog();
                log.WriteProcess("Ocurrió un error al enviar el email reportando el error de envío de factura. Por favor revisar el log de errores.");
                log.WriteError("Error realizando el envío el email reportando el error de envío de factura. Error - " + ex.Message + " - " + ex.StackTrace.ToString());
                return false;
            }
        }

        private void deletePDF(string PDFPath)
        {
            try
            {
                if (System.IO.File.Exists(PDFPath))
                {
                    System.IO.File.Delete(PDFPath);
                }
            }
            catch (Exception ex)
            {
                ServiceLog log = new ServiceLog();
                log.WriteProcess("Ocurrió un error al borrar el archivo del contrato. Por favor revisar el log de errores.");
                log.WriteError("Error borrando el archivo del contrato. Error - " + ex.Message + " - " + ex.StackTrace.ToString());
            }
        }
    }
}
