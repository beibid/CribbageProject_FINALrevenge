using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Mail
{
    public class SanctionEmailSender
    {
        public void SendSanctionNotification(string recipientEmail, string reason, DateTime suspensionEndDate)
        {
            string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            int smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
            string smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
            string smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
            string messageBody = string.Format(Lang.SanctionEmail_TemporaryBodyFormat, reason, suspensionEndDate);
            using (SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = true;
                MailMessage mailMessage = new MailMessage(smtpUsername, recipientEmail);
                mailMessage.Subject = Lang.SanctionEmail_Subject;
                mailMessage.Body = messageBody;
                smtpClient.Send(mailMessage);
            }
        }
    }
}