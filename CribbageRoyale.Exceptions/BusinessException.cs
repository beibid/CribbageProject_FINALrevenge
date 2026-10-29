using System;

namespace CribbageRoyale.Exceptions
{
    public abstract class BusinessException : CribbageRoyaleException
    {
        protected BusinessException()
        {
        }

        protected BusinessException(string message) : base(message)
        {
        }

        protected BusinessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}