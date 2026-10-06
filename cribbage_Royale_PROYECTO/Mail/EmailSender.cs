using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Mail;

namespace cribbage_Royale_PROYECTO.Mail
{
    public abstract class EmailSender
    {
        private const string HostSettingKey = "SmtpHost";
        private const string PortSettingKey = "SmtpPort";
        private const string AddressSettingKey = "SmtpAddress";
        private const string PasswordSettingKey = "SmtpPassword";
        private const string SenderNameSettingKey = "SmtpSenderName";

        protected bool SendMessage(string recipient, string subject, string body)
        {
            bool wasSent = false;

            if (IsConfigurationComplete())
            {
                wasSent = DeliverMessage(recipient, subject, body);
            }

            return wasSent;
        }

        private bool IsConfigurationComplete()
        {
            bool isComplete = false;

            if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[HostSettingKey])
                && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[AddressSettingKey])
                && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[PasswordSettingKey])
                && int.TryParse(
                    ConfigurationManager.AppSettings[PortSettingKey],
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out int smtpPort))
            {
                isComplete = true;
            }

            return isComplete;
        }

        private bool DeliverMessage(string recipient, string subject, string body)
        {
            bool wasSent = false;

            try
            {
                Deliver(recipient, subject, body);
                wasSent = true;
            }
            catch (SmtpException smtpException)
            {
                Trace.WriteLine(smtpException.Message);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                Trace.WriteLine(invalidOperationException.Message);
            }
            catch (FormatException formatException)
            {
                Trace.WriteLine(formatException.Message);
            }

            return wasSent;
        }

        private void Deliver(string recipient, string subject, string body)
        {
            string senderAddress = ConfigurationManager.AppSettings[AddressSettingKey];
            string senderPassword = ConfigurationManager.AppSettings[PasswordSettingKey];
            string host = ConfigurationManager.AppSettings[HostSettingKey];

            int port = int.Parse(
                ConfigurationManager.AppSettings[PortSettingKey],
                CultureInfo.InvariantCulture);

            using (MailMessage message = BuildMessage(senderAddress, recipient, subject, body))
            {
                using (SmtpClient client = new SmtpClient(host, port))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(senderAddress, senderPassword);
                    client.Send(message);
                }
            }
        }

        private MailMessage BuildMessage(
            string senderAddress, string recipient, string subject, string body)
        {
            MailMessage message = new MailMessage();

            message.From = new MailAddress(
                senderAddress,
                ConfigurationManager.AppSettings[SenderNameSettingKey]);

            message.To.Add(recipient);
            message.Subject = subject;
            message.Body = body;

            return message;
        }
    }
}