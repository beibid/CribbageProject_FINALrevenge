using System;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Exceptions
{
    public class InvalidSessionException : BusinessException
    {
        private readonly SessionFailure _failure;

        public InvalidSessionException(SessionFailure failure)
        {
            _failure = failure;
        }

        public InvalidSessionException(SessionFailure failure, string message) : base(message)
        {
            _failure = failure;
        }

        public InvalidSessionException(SessionFailure failure, string message, Exception innerException)
            : base(message, innerException)
        {
            _failure = failure;
        }

        public SessionFailure Failure
        {
            get { return _failure; }
        }
    }
}