using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class PlayerSearchResult
    {
        public PlayerSearchResult(ServiceFailureReason reason, FriendshipFailure? failure, PlayerSummary[] players)
        {
            Reason = reason;
            Failure = failure;
            Players = players;
        }

        public bool Succeeded
        {
            get { return Reason == ServiceFailureReason.None; }
        }

        public ServiceFailureReason Reason { get; }

        public FriendshipFailure? Failure { get; }

        public PlayerSummary[] Players { get; }
    }
}