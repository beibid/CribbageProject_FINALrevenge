using System.Collections.Generic;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Exceptions;
using CribbageRoyale.Server.Connections;
using CribbageRoyale.Server.Projection;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server.Matches
{
    public class MatchDisconnectionWatcher
    {
        private const string CleanupFailureMessage =
            "The pending invitations of a disconnected player could not be cleaned up.";

        private readonly MatchInvitationRegistry _invitationRegistry;
        private readonly MatchRoomRegistry _matchRooms;
        private readonly MatchWatcherServices _services;

        public MatchDisconnectionWatcher(MatchInvitationRegistry invitationRegistry,
            MatchRoomRegistry matchRooms, MatchWatcherServices services)
        {
            _invitationRegistry = invitationRegistry;
            _matchRooms = matchRooms;
            _services = services;
        }

        public void Watch()
        {
            _services.PlayerChannel.PlayerConnectionChanged += PlayerChannel_PlayerConnectionChanged;
        }

        private void PlayerChannel_PlayerConnectionChanged(object sender,
            PlayerConnectionChangedEventArgs e)
        {
            if (!e.IsOnline)
            {
                _matchRooms.RemovePlayer(e.AccountId);
                DropInvitations(e.AccountId);
            }
        }

        private void DropInvitations(int accountId)
        {
            List<PendingInvitation> dropped = _invitationRegistry.TakeAllFor(accountId);

            foreach (PendingInvitation invitation in dropped)
            {
                TryNotifyCounterpart(invitation, accountId);
            }
        }

        private void TryNotifyCounterpart(PendingInvitation invitation, int goneAccountId)
        {
            try
            {
                NotifyCounterpart(invitation, goneAccountId);
            }
            catch (DataAccessException dataAccessException)
            {
                _services.Logger.LogWarning(dataAccessException, CleanupFailureMessage);
            }
        }

        private void NotifyCounterpart(PendingInvitation invitation, int goneAccountId)
        {
            MatchInvitation declined = _services.StateProjection.BuildMatchInvitation(
                invitation.InvitationId,
                invitation.SenderAccountId,
                invitation.RecipientAccountId);

            _services.PlayerChannel.TrySendNotification(
                ResolveCounterpart(invitation, goneAccountId),
                new MatchInvitationDeclinedNotification(declined));
        }

        private int ResolveCounterpart(PendingInvitation invitation, int goneAccountId)
        {
            int counterpartAccountId = invitation.SenderAccountId;

            if (goneAccountId == invitation.SenderAccountId)
            {
                counterpartAccountId = invitation.RecipientAccountId;
            }

            return counterpartAccountId;
        }
    }
}