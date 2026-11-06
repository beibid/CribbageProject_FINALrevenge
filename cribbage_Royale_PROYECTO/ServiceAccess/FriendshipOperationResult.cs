using CribbageRoyale.Contracts.FaultContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class FriendshipOperationResult
    {
        public FriendshipOperationResult(ServiceFailureReason reason, FriendshipFailure? failure)
        {
            Reason = reason;
            Failure = failure;
        }

        public bool Succeeded
        {
            get { return Reason == ServiceFailureReason.None; }
        }

        public ServiceFailureReason Reason { get; }

        public FriendshipFailure? Failure { get; }
    }
}