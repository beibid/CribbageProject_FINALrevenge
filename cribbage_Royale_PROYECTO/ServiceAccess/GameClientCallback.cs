using cribbage_Royale_DataAccess.DataAccess;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using System;
using System.ServiceModel;
using System.Windows.Threading;

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

        public event EventHandler<ChatMessageReceivedEventArgs> ChatMessageArrived;

        public event EventHandler<MatchInvitationEventArgs> InvitationArrived;

        public event EventHandler<MatchInvitationEventArgs> InvitationWithdrawn;

        public event EventHandler<MatchStartedEventArgs> MatchStartReceived;

        public event EventHandler<MatchStateChangedEventArgs> MatchStateReceived;

        public event EventHandler<SessionRevokedEventArgs> RevocationReceived;

        public void PresenceChanged(PresenceChange presence)
        {
            _dispatcher.BeginInvoke(new Action(() => OnPresenceReceived(presence)));
        }

        public void SocialEventOccurred(SocialEventType eventType, SocialEvent social)
        {
            _dispatcher.BeginInvoke(new Action(() => OnSocialEventReceived(eventType, social)));
        }

        public void ChatMessageReceived(ChatMessage message)
        {
            _dispatcher.BeginInvoke(new Action(() => OnChatMessageArrived(message)));
        }

        public void MatchInvitationReceived(MatchInvitation invitation)
        {
            _dispatcher.BeginInvoke(new Action(() => OnInvitationArrived(invitation)));
        }

        public void MatchInvitationDeclined(MatchInvitation invitation)
        {
            _dispatcher.BeginInvoke(new Action(() => OnInvitationWithdrawn(invitation)));
        }

        public void MatchStarted(MatchStart matchStart)
        {
            _dispatcher.BeginInvoke(new Action(() => OnMatchStartReceived(matchStart)));
        }

        public void MatchStateChanged(MatchEventType eventType, MatchState state)
        {
            _dispatcher.BeginInvoke(new Action(() => OnMatchStateReceived(eventType, state)));
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

        private void OnChatMessageArrived(ChatMessage message)
        {
            EventHandler<ChatMessageReceivedEventArgs> handler = ChatMessageArrived;

            if (handler != null)
            {
                handler(this, new ChatMessageReceivedEventArgs(message));
            }
        }

        private void OnInvitationArrived(MatchInvitation invitation)
        {
            EventHandler<MatchInvitationEventArgs> handler = InvitationArrived;

            if (handler != null)
            {
                handler(this, new MatchInvitationEventArgs(invitation));
            }
        }

        private void OnInvitationWithdrawn(MatchInvitation invitation)
        {
            EventHandler<MatchInvitationEventArgs> handler = InvitationWithdrawn;

            if (handler != null)
            {
                handler(this, new MatchInvitationEventArgs(invitation));
            }
        }

        private void OnMatchStartReceived(MatchStart matchStart)
        {
            EventHandler<MatchStartedEventArgs> handler = MatchStartReceived;

            if (handler != null)
            {
                handler(this, new MatchStartedEventArgs(matchStart));
            }
        }

        private void OnMatchStateReceived(MatchEventType eventType, MatchState state)
        {
            EventHandler<MatchStateChangedEventArgs> handler = MatchStateReceived;

            if (handler != null)
            {
                handler(this, new MatchStateChangedEventArgs(eventType, state));
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