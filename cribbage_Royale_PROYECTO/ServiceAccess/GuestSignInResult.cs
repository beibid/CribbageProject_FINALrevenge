using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class GuestSignInResult
    {
        public GuestSignInResult(ServiceFailureReason reason, SessionFailure? failure, GuestSession session)
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

        public GuestSession Session { get; }
    }
}