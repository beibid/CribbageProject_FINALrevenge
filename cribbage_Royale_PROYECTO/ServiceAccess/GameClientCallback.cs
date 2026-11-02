using System;
using System.ServiceModel;
using System.Windows.Threading;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.ServiceContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class GameClientCallback : IGameClientCallback
    {
        private readonly Dispatcher _dispatcher;

        public GameClientCallback(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public event EventHandler<PresenceChangedEventArgs> PresenceReceived;

        public event EventHandler<SocialEventReceivedEventArgs> SocialEventReceived;

        public event EventHandler<SessionRevokedEventArgs> RevocationReceived;

        public void PresenceChanged(PresenceChange presence)
        {
            _dispatcher.BeginInvoke(new Action(() => OnPresenceReceived(presence)));
        }

        public void SocialEventOccurred(SocialEventType eventType, SocialEvent social)
        {
            _dispatcher.BeginInvoke(new Action(() => OnSocialEventReceived(eventType, social)));
        }

        public void SessionRevoked(SessionRevocation revocation)
        {
            _dispatcher.BeginInvoke(new Action(() => OnRevocationReceived(revocation)));
        }

        private void OnPresenceReceived(PresenceChange presence)
        {
            EventHandler<PresenceChangedEventArgs> handler = PresenceReceived;

            if (handler != null)
            {
                handler(this, new PresenceChangedEventArgs(presence));
            }
        }

        private void OnSocialEventReceived(SocialEventType eventType, SocialEvent social)
        {
            EventHandler<SocialEventReceivedEventArgs> handler = SocialEventReceived;

            if (handler != null)
            {
                handler(this, new SocialEventReceivedEventArgs(eventType, social));
            }
        }

        private void OnRevocationReceived(SessionRevocation revocation)
        {
            EventHandler<SessionRevokedEventArgs> handler = RevocationReceived;

            if (handler != null)
            {
                handler(this, new SessionRevokedEventArgs(revocation));
            }
        }
    }
}