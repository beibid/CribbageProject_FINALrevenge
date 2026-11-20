using System;
using CribbageRoyale.Contracts.DataContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class MatchStateChangedEventArgs : EventArgs
    {
        public MatchStateChangedEventArgs(MatchEventType eventType, MatchState state)
        {
            EventType = eventType;
            State = state;
        }

        public MatchEventType EventType { get; }

        public MatchState State { get; }
    }
}