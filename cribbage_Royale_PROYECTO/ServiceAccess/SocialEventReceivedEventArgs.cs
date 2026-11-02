using System;
using CribbageRoyale.Contracts.DataContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class SocialEventReceivedEventArgs : EventArgs
    {
        public SocialEventReceivedEventArgs(SocialEventType eventType, SocialEvent social)
        {
            EventType = eventType;
            Social = social;
        }

        public SocialEventType EventType { get; }

        public SocialEvent Social { get; }
    }
}