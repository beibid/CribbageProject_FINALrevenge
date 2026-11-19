using CribbageRoyale.Contracts.FaultContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class MatchActionResult
    {
        public MatchActionResult(ServiceFailureReason reason, MatchFailure? failure)
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