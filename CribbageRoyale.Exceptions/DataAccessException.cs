using System;

namespace CribbageRoyale.Exceptions
{
    public class DataAccessException : TechnicalException
    {
        public DataAccessException()
        {
        }

        public DataAccessException(string message) : base(message)
        {
        }

        public DataAccessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}