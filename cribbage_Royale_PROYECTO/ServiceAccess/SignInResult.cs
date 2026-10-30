using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class SignInResult
    {
        public SignInResult(ServiceFailureReason reason, SessionFailure? failure, AuthenticatedSession session)
        {
            Reason = reason;
            Failure = failure;
            Session = session;
        }

        public bool Succeeded
        {
            get { return Reason == ServiceFailureReason.None; }
        }

        public ServiceFailureReason Reason { get; }

        public SessionFailure? Failure { get; }

        public AuthenticatedSession Session { get; }
    }
}