using CribbageRoyale.Contracts.DataContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class PendingFriendRequestsResult
    {
        public PendingFriendRequestsResult(ServiceFailureReason reason, FriendRequest[] requests)
        {
            Reason = reason;
            Requests = requests;
        }

        public bool Succeeded
        {
            get { return Reason == ServiceFailureReason.None; }
        }

        public ServiceFailureReason Reason { get; }

        public FriendRequest[] Requests { get; }
    }
}