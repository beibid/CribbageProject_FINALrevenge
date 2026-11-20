using System.Collections.Generic;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Rules;
using CribbageRoyale.Server.Connections;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server.Matches
{
    public class MatchDisconnectionWatcher
    {
        private const string PlayerDisconnectedMessage =
            "Player {AccountId} disconnected from match {MatchId}. The rival wins by abandonment.";

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
                EndMatchOf(e.AccountId);
                DropInvitations(e.AccountId);
            }
        }

        private void EndMatchOf(int accountId)
        {
            CribbageMatch match;

            if (_matchRooms.FindMatchOf(accountId, out match))
            {
                int rivalAccountId = match.GetOpponentId(accountId);

                if (match.AbandonBy(accountId))
                {
                    _services.Logger.LogWarning(PlayerDisconnectedMessage, accountId, match.MatchId);
                    NotifyRival(match, rivalAccountId);
                }

                _matchRooms.CloseMatch(match.MatchId, accountId, rivalAccountId);
            }
        }

        private void NotifyRival(CribbageMatch match, int rivalAccountId)
        {
            MatchState state = _services.StateProjection.BuildMatchState(match, rivalAccountId);

            _services.PlayerChannel.TrySendNotification(
                rivalAccountId,
                callback => callback.MatchStateChanged(MatchEventType.RivalDisconnected, state));
        }

        private void DropInvitations(int accountId)
        {
            List<PendingInvitation> dropped = _invitationRegistry.TakeAllFor(accountId);

            foreach (PendingInvitation invitation in dropped)
            {
                NotifyCounterpart(invitation, accountId);
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
                callback => callback.MatchInvitationDeclined(declined));
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