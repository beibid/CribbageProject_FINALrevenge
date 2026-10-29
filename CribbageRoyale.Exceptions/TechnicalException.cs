using System;

namespace CribbageRoyale.Exceptions
{
    public abstract class TechnicalException : CribbageRoyaleException
    {
        protected TechnicalException()
        {
        }

        protected TechnicalException(string message) : base(message)
        {
        }

        protected TechnicalException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}