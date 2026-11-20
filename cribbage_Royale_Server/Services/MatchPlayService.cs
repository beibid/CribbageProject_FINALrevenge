using System.Collections.Generic;
using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using CribbageRoyale.Rules;
using CribbageRoyale.Server.Sessions;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class MatchPlayService : IMatchPlay
    {
        private const string OperationRejectedMessage =
            "A match operation was rejected. Reason: {Reason}. MatchId: {MatchId}";
        private const string MatchAbandonedMessage =
            "Player {AccountId} abandoned match {MatchId}.";

        private readonly MatchPlayServices _services;
        private readonly ILogger _logger;

        public MatchPlayService(MatchPlayServices services, ILogger logger)
        {
            _services = services;
            _logger = logger;
        }

        public void DiscardToCrib(CribDiscard discard)
        {
            SessionIdentity caller = _services.CallerSession.RequireRegisteredAccount();
            CribbageMatch match = RequireMatch(discard.MatchId, caller.AccountId);

            EnsurePhaseAllowsDiscard(match);
            Discard(match, caller.AccountId, discard.Cards);
            SendStateToBoth(match, caller.AccountId, ResolveDiscardEvent(match));
        }

        public void PlayCard(CardPlay play)
        {
            SessionIdentity caller = _services.CallerSession.RequireRegisteredAccount();
            CribbageMatch match = RequireMatch(play.MatchId, caller.AccountId);

            EnsurePhaseAllowsPlay(match);
            EnsureCallerTurn(match, caller.AccountId);
            Play(match, caller.AccountId, play.Card);
            SendStateToBoth(match, caller.AccountId, MatchEventType.RivalPlayed);
            CloseWhenFinished(match, caller.AccountId);
        }

        public void AbandonMatch(long matchId)
        {
            SessionIdentity caller = _services.CallerSession.RequireRegisteredAccount();
            CribbageMatch match = RequireMatch(matchId, caller.AccountId);

            if (match.AbandonBy(caller.AccountId))
            {
                _logger.LogInformation(MatchAbandonedMessage, caller.AccountId, matchId);

                SendState(
                    match,
                    match.GetOpponentId(caller.AccountId),
                    MatchEventType.RivalAbandoned);
            }

            CloseWhenFinished(match, caller.AccountId);
        }

        private CribbageMatch RequireMatch(long matchId, int accountId)
        {
            CribbageMatch match;

            if (!_services.MatchRooms.TryGetMatch(matchId, out match))
            {
                throw BuildMatchFault(MatchFailure.MatchNotFound, matchId);
            }

            if (!match.IsParticipant(accountId))
            {
                throw BuildMatchFault(MatchFailure.NotAParticipant, matchId);
            }

            return match;
        }

        private void EnsurePhaseAllowsDiscard(CribbageMatch match)
        {
            if (match.Phase != MatchPhase.Discarding)
            {
                throw BuildMatchFault(MatchFailure.MatchNotInProgress, match.MatchId);
            }
        }

        private void EnsurePhaseAllowsPlay(CribbageMatch match)
        {
            if (match.Phase != MatchPhase.Playing)
            {
                throw BuildMatchFault(MatchFailure.MatchNotInProgress, match.MatchId);
            }
        }

        private void EnsureCallerTurn(CribbageMatch match, int accountId)
        {
            if (match.TurnAccountId != accountId)
            {
                throw BuildMatchFault(MatchFailure.NotYourTurn, match.MatchId);
            }
        }

        private void Discard(CribbageMatch match, int accountId, List<Card> cards)
        {
            if (!match.AcceptDiscard(accountId, cards))
            {
                throw BuildMatchFault(MatchFailure.InvalidDiscard, match.MatchId);
            }
        }

        private void Play(CribbageMatch match, int accountId, Card card)
        {
            if (!match.AcceptPlay(accountId, card))
            {
                throw BuildMatchFault(MatchFailure.InvalidPlay, match.MatchId);
            }
        }

        private void CloseWhenFinished(CribbageMatch match, int accountId)
        {
            if (match.Phase == MatchPhase.Finished)
            {
                _services.MatchRooms.CloseMatch(
                    match.MatchId,
                    accountId,
                    match.GetOpponentId(accountId));
            }
        }

        private void SendStateToBoth(CribbageMatch match, int accountId, MatchEventType eventType)
        {
            SendState(match, accountId, eventType);
            SendState(match, match.GetOpponentId(accountId), eventType);
        }

        private void SendState(CribbageMatch match, int accountId, MatchEventType eventType)
        {
            MatchState state = _services.StateProjection.BuildMatchState(match, accountId);

            _services.PlayerChannel.TrySendNotification(
                accountId,
                callback => callback.MatchStateChanged(eventType, state));
        }

        private FaultException<MatchFault> BuildMatchFault(MatchFailure reason, long matchId)
        {
            MatchFault fault = new MatchFault();
            fault.Reason = reason;

            _logger.LogWarning(OperationRejectedMessage, reason, matchId);

            return new FaultException<MatchFault>(fault, reason.ToString());
        }

        private static MatchEventType ResolveDiscardEvent(CribbageMatch match)
        {
            MatchEventType eventType = MatchEventType.CardsDiscarded;

            if (match.HasCutCard)
            {
                eventType = MatchEventType.CutCardRevealed;
            }

            return eventType;
        }
    }
}