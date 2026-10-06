using System.Globalization;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Mail
{
    public class VerificationEmailSender : EmailSender
    {
        public bool SendVerificationCode(string recipient, string code)
        {
            string body = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Email_VerificationBodyFormat, code);

            return SendMessage(recipient, Lang.Email_VerificationSubject, body);
        }
    }
}