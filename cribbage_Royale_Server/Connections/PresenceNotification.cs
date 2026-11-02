using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.ServiceContracts;

namespace CribbageRoyale.Server.Connections
{
    public class PresenceNotification : Notification
    {
        private readonly PresenceChange _presence;

        public PresenceNotification(PresenceChange presence)
        {
            _presence = presence;
        }

        public override void Deliver(IGameClientCallback callback)
        {
            callback.PresenceChanged(_presence);
        }
    }
}