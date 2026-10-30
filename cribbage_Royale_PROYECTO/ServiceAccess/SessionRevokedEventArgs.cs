using System;
using CribbageRoyale.Contracts.DataContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class SessionRevokedEventArgs : EventArgs
    {
        private readonly SessionRevocation _revocation;

        public SessionRevokedEventArgs(SessionRevocation revocation)
        {
            _revocation = revocation;
        }

        public SessionRevocation Revocation
        {
            get { return _revocation; }
        }
    }
}