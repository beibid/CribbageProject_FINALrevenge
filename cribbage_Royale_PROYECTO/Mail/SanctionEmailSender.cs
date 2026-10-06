using System;
using System.Globalization;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Mail
{
    public class SanctionEmailSender : EmailSender
    {
        public bool SendSanctionNotification(
            string recipient, string reason, DateTime suspensionEndDate)
        {
            string body = string.Format(
                CultureInfo.CurrentCulture,
                Lang.SanctionEmail_TemporaryBodyFormat,
                reason,
                suspensionEndDate);

            return SendMessage(recipient, Lang.SanctionEmail_Subject, body);
        }
    }
}