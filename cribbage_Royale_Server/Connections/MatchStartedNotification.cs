using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.ServiceContracts;

namespace CribbageRoyale.Server.Connections
{
    public class MatchStartedNotification : Notification
    {
        private readonly MatchStart _matchStart;

        public MatchStartedNotification(MatchStart matchStart)
        {
            _matchStart = matchStart;
        }

        public override void Deliver(IGameClientCallback callback)
        {
            callback.MatchStarted(_matchStart);
        }
    }
}