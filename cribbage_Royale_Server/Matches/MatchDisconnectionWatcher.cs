using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Rules;
using CribbageRoyale.Server.Connections;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server.Matches
{
    public class MatchDisconnectionWatcher
    {
        private static readonly TimeSpan ReturnGracePeriod = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan ResumeDelay = TimeSpan.FromSeconds(2);

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
            if (e.IsOnline)
            {
                Task.Delay(ResumeDelay).ContinueWith(task => ResumeMatchOf(e.AccountId));
            }
            else
            {
                WaitForReturn(e.AccountId);
                DropInvitations(e.AccountId);
            }
        }

        private void WaitForReturn(int accountId)
        {
            CribbageMatch match;

            if (_matchRooms.FindMatchOf(accountId, out match) && match.Phase != MatchPhase.Finished)
            {
                _services.Logger.LogWarning(
                    "Player {AccountId} disconnected from match {MatchId}. Waiting for the player to return.",
                    accountId,
                    match.MatchId);

                SendState(match, match.GetOpponentId(accountId), MatchEventType.RivalDisconnected);
                Task.Delay(ReturnGracePeriod).ContinueWith(task => EndMatchWhenStillGone(accountId));
            }
        }

        private void EndMatchWhenStillGone(int accountId)
        {
            CribbageMatch match;

            if (!_services.PlayerChannel.IsPlayerConnected(accountId)
                && _matchRooms.FindMatchOf(accountId, out match))
            {
                int rivalAccountId = match.GetOpponentId(accountId);

                if (_services.PlayerChannel.IsPlayerConnected(rivalAccountId))
                {
                    AwardAbandonment(match, accountId, rivalAccountId);
                }
                else
                {
                    _services.Logger.LogInformation(
                        "Both players left match {MatchId}. The match was cancelled.",
                        match.MatchId);
                }

                _matchRooms.CloseMatch(match.MatchId, accountId, rivalAccountId);
            }
        }

        private void AwardAbandonment(CribbageMatch match, int accountId, int rivalAccountId)
        {
            if (match.AbandonBy(accountId))
            {
                _services.Logger.LogWarning(
                    "Player {AccountId} did not return to match {MatchId}. The rival wins by abandonment.",
                    accountId,
                    match.MatchId);

                SendState(match, rivalAccountId, MatchEventType.RivalAbandoned);
            }
        }

        private void ResumeMatchOf(int accountId)
        {
            CribbageMatch match;

            if (_matchRooms.FindMatchOf(accountId, out match) && match.Phase != MatchPhase.Finished)
            {
                int rivalAccountId = match.GetOpponentId(accountId);

                MatchStart matchStart = _services.StateProjection.BuildMatchStart(
                    match.MatchId,
                    rivalAccountId,
                    accountId == match.HostAccountId);

                _services.PlayerChannel.TrySendNotification(
                    accountId,
                    callback => callback.MatchStarted(matchStart));

                SendState(match, accountId, MatchEventType.Started);
                SendState(match, rivalAccountId, MatchEventType.Resumed);

                _services.Logger.LogInformation(
                    "Player {AccountId} returned to match {MatchId}.",
                    accountId,
                    match.MatchId);
            }
        }

        private void SendState(CribbageMatch match, int accountId, MatchEventType eventType)
        {
            MatchState state = _services.StateProjection.BuildMatchState(match, accountId);

            _services.PlayerChannel.TrySendNotification(
                accountId,
                callback => callback.MatchStateChanged(eventType, state));
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