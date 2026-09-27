using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Mail
{
    public class VerificationEmailSender
    {
        private const string HostSettingKey = "SmtpHost";
        private const string PortSettingKey = "SmtpPort";
        private const string AddressSettingKey = "SmtpAddress";
        private const string PasswordSettingKey = "SmtpPassword";
        private const string SenderNameSettingKey = "SmtpSenderName";

        public bool SendVerificationCode(string recipient, string code)
        {
            bool wasSent = false;

            if (IsConfigurationComplete())
            {
                wasSent = DeliverMessage(recipient, code);
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
                    out int port))
            {
                isComplete = true;
            }

            return isComplete;
        }

        private bool DeliverMessage(string recipient, string code)
        {
            bool wasSent = false;

            try
            {
                SendMessage(recipient, code);
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

        private void SendMessage(string recipient, string code)
        {
            string senderAddress = ConfigurationManager.AppSettings[AddressSettingKey];
            string senderPassword = ConfigurationManager.AppSettings[PasswordSettingKey];
            string host = ConfigurationManager.AppSettings[HostSettingKey];
            int port = int.Parse(
                ConfigurationManager.AppSettings[PortSettingKey],
                CultureInfo.InvariantCulture);

            using (MailMessage message = BuildMessage(senderAddress, recipient, code))
            {
                using (SmtpClient client = new SmtpClient(host, port))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(senderAddress, senderPassword);
                    client.Send(message);
                }
            }
        }

        private MailMessage BuildMessage(string senderAddress, string recipient, string code)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(
                senderAddress,
                ConfigurationManager.AppSettings[SenderNameSettingKey]);
            message.To.Add(recipient);
            message.Subject = Lang.Email_VerificationSubject;
            message.Body = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Email_VerificationBodyFormat,
                code);

            return message;
        }
    }
}