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

        public event EventHandler<SessionRevokedEventArgs> RevocationReceived;

        public void PresenceChanged(PresenceChange presence)
        {
            _dispatcher.BeginInvoke(new Action(() => RaisePresence(presence)));
        }

        public void SessionRevoked(SessionRevocation revocation)
        {
            _dispatcher.BeginInvoke(new Action(() => RaiseRevocation(revocation)));
        }

        private void RaisePresence(PresenceChange presence)
        {
            EventHandler<PresenceChangedEventArgs> handler = PresenceReceived;

            if (handler != null)
            {
                handler(this, new PresenceChangedEventArgs(presence));
            }
        }

        private void RaiseRevocation(SessionRevocation revocation)
        {
            EventHandler<SessionRevokedEventArgs> handler = RevocationReceived;

            if (handler != null)
            {
                handler(this, new SessionRevokedEventArgs(revocation));
            }
        }
    }
}