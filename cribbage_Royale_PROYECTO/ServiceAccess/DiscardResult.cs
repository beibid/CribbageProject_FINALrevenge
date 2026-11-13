using CribbageRoyale.Contracts.FaultContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class DiscardResult
    {
        public DiscardResult(ServiceFailureReason reason, MatchFailure? failure)
        {
            Reason = reason;
            Failure = failure;
        }

        public bool Succeeded
        {
            get { return Reason == ServiceFailureReason.None; }
        }

        public ServiceFailureReason Reason { get; }

        public MatchFailure? Failure { get; }
    }
}