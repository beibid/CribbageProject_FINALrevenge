using System;

namespace CribbageRoyale.Exceptions
{
    public abstract class CribbageRoyaleException : Exception
    {
        protected CribbageRoyaleException()
        {
        }

        protected CribbageRoyaleException(string message) : base(message)
        {
        }

        protected CribbageRoyaleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}