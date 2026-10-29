using System;

namespace CribbageRoyale.Exceptions
{
    public class MailDeliveryException : TechnicalException
    {
        public MailDeliveryException()
        {
        }

        public MailDeliveryException(string message) : base(message)
        {
        }

        public MailDeliveryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}