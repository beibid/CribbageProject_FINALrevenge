using System;
using CribbageRoyale.Contracts.DataContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class PresenceChangedEventArgs : EventArgs
    {
        private readonly PresenceChange _presence;

        public PresenceChangedEventArgs(PresenceChange presence)
        {
            _presence = presence;
        }

        public PresenceChange Presence
        {
            get { return _presence; }
        }
    }
}