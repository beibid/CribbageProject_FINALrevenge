using System;
using CribbageRoyale.Contracts.DataContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class MatchStartedEventArgs : EventArgs
    {
        public MatchStartedEventArgs(MatchStart matchStart)
        {
            MatchStart = matchStart;
        }

        public MatchStart MatchStart { get; }
    }
}