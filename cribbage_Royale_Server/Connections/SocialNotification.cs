using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.ServiceContracts;

namespace CribbageRoyale.Server.Connections
{
    public class SocialNotification : Notification
    {
        private readonly SocialEventType _eventType;
        private readonly SocialEvent _social;

        public SocialNotification(SocialEventType eventType, SocialEvent social)
        {
            _eventType = eventType;
            _social = social;
        }

        public override void Deliver(IGameClientCallback callback)
        {
            callback.SocialEventOccurred(_eventType, _social);
        }
    }
}