using CribbageRoyale.Contracts.DataContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class FriendListResult
    {
        public FriendListResult(ServiceFailureReason reason, FriendSummary[] friends)
        {
            Reason = reason;
            Friends = friends;
        }

        public bool Succeeded
        {
            get { return Reason == ServiceFailureReason.None; }
        }

        public ServiceFailureReason Reason { get; }

        public FriendSummary[] Friends { get; }
    }
}