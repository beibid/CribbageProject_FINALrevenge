using System;
using CribbageRoyale.Contracts.DataContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class MatchInvitationEventArgs : EventArgs
    {
        public MatchInvitationEventArgs(MatchInvitation invitation)
        {
            Invitation = invitation;
        }

        public MatchInvitation Invitation { get; }
    }
}