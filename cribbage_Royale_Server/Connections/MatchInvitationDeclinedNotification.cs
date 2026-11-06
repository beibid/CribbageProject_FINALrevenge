using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.ServiceContracts;

namespace CribbageRoyale.Server.Connections
{
    public class MatchInvitationDeclinedNotification : Notification
    {
        private readonly MatchInvitation _invitation;

        public MatchInvitationDeclinedNotification(MatchInvitation invitation)
        {
            _invitation = invitation;
        }

        public override void Deliver(IGameClientCallback callback)
        {
            callback.MatchInvitationDeclined(_invitation);
        }
    }
}