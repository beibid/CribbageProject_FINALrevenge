using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.ServiceContracts;

namespace CribbageRoyale.Server.Connections
{
    public class MatchInvitationNotification : Notification
    {
        private readonly MatchInvitation _invitation;

        public MatchInvitationNotification(MatchInvitation invitation)
        {
            _invitation = invitation;
        }

        public override void Deliver(IGameClientCallback callback)
        {
            callback.MatchInvitationReceived(_invitation);
        }
    }
}