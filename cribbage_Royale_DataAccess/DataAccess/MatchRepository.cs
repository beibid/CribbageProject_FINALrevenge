using System;
using System.Collections.Generic;
using System.Linq;
using cribbage_Royale_DataAccess.Models;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class MatchRepository
    {
        private const string AbandonmentEventType = "MATCH_ABANDONMENT";

        public MatchInformationSummary GetMatchInformation(long matchId, int viewerAccountId)
        {
            MatchInformationSummary summary = null;

            using (CribbageEntities context = new CribbageEntities())
            {
                List<match_participant> participants = context.match_participant
                    .Where(currentParticipant => currentParticipant.match_id == matchId)
                    .ToList();
                match_participant viewer = participants
                    .FirstOrDefault(currentParticipant => currentParticipant.account_id == viewerAccountId);
                match_participant opponent = participants
                    .FirstOrDefault(currentParticipant => currentParticipant.account_id != viewerAccountId);

                if (viewer != null && opponent != null)
                {
                    summary = BuildMatchInformation(context, viewer, opponent);
                }
            }

            return summary;
        }

        public UnfinishedMatchSummary FindUnfinishedMatch(int accountId)
        {
            UnfinishedMatchSummary unfinishedMatch = null;

            using (CribbageEntities context = new CribbageEntities())
            {
                long matchId = (from participant in context.match_participant
                                join currentMatch in context.match
                                    on participant.match_id equals currentMatch.match_id
                                where participant.account_id == accountId
                                    && currentMatch.match_status == MatchStatus.InProgress
                                orderby currentMatch.started_at descending
                                select currentMatch.match_id).FirstOrDefault();

                if (matchId != 0)
                {
                    unfinishedMatch = BuildUnfinishedMatch(context, matchId, accountId);
                }
            }

            return unfinishedMatch;
        }
        public static bool HasUnfinishedMatchInContext(CribbageEntities context, int accountId)
        {
            return (from participant in context.match_participant
                    join currentMatch in context.match
                        on participant.match_id equals currentMatch.match_id
                    where participant.account_id == accountId
                        && currentMatch.match_status == MatchStatus.InProgress
                    select participant.match_participant_id).Any();
        }
        public bool MarkParticipantConnected(long matchId, int accountId)
        {
            bool updated = false;

            using (CribbageEntities context = new CribbageEntities())
            {
                match_participant participant = context.match_participant
                    .FirstOrDefault(currentParticipant => currentParticipant.match_id == matchId
                        && currentParticipant.account_id == accountId);

                if (participant != null)
                {
                    participant.is_connected = true;
                    context.SaveChanges();
                    updated = true;
                }
            }

            return updated;
        }

        public string RegisterAbandonment(long matchId, int accountId)
        {
            string outcome = AbandonmentOutcome.AlreadyConcluded;

            using (CribbageEntities context = new CribbageEntities())
            {
                match currentMatch = context.match
                    .FirstOrDefault(matchRow => matchRow.match_id == matchId);
                match_participant opponent = context.match_participant
                    .FirstOrDefault(currentParticipant => currentParticipant.match_id == matchId
                        && currentParticipant.account_id != accountId);
                bool isParticipant = context.match_participant
                    .Any(currentParticipant => currentParticipant.match_id == matchId
                        && currentParticipant.account_id == accountId);

                if (currentMatch != null && opponent != null && isParticipant
                    && currentMatch.match_status == MatchStatus.InProgress)
                {
                    DateTime now = DateTime.Now;
                    currentMatch.match_status = MatchStatus.Concluded;
                    currentMatch.end_reason = MatchTerminationReason.Abandonment;
                    currentMatch.ended_at = now;
                    currentMatch.winner_account_id = opponent.account_id;

                    event_log abandonmentEvent = new event_log();
                    abandonmentEvent.account_id = accountId;
                    abandonmentEvent.match_id = matchId;
                    abandonmentEvent.event_type = AbandonmentEventType;
                    abandonmentEvent.description = MatchTerminationReason.Abandonment;
                    abandonmentEvent.logged_at = now;
                    context.event_log.Add(abandonmentEvent);

                    context.SaveChanges();
                    outcome = AbandonmentOutcome.Registered;
                }
            }

            return outcome;
        }

        private MatchInformationSummary BuildMatchInformation(CribbageEntities context,
            match_participant viewer, match_participant opponent)
        {
            MatchInformationSummary summary = new MatchInformationSummary();
            long matchId = viewer.match_id;
            int opponentAccountId = opponent.account_id;

            player_profile opponentProfile = context.player_profile
                .FirstOrDefault(profile => profile.account_id == opponentAccountId);

            summary.MatchId = matchId;
            summary.OpponentAccountId = opponentAccountId;
            summary.OpponentConnected = opponent.is_connected;

            if (opponentProfile != null)
            {
                summary.OpponentDisplayName = opponentProfile.display_name;
                summary.OpponentSkillLevel = opponentProfile.skill_level;
            }

            List<long> handIds = context.round
                .Where(hand => hand.match_id == matchId)
                .Select(hand => hand.round_id)
                .ToList();

            summary.ViewerScore = GetScore(context, handIds, viewer.match_participant_id);
            summary.OpponentScore = GetScore(context, handIds, opponent.match_participant_id);

            round currentHand = context.round
                .Where(hand => hand.match_id == matchId)
                .OrderByDescending(hand => hand.round_number)
                .FirstOrDefault();

            if (currentHand != null)
            {
                FillCurrentHandInformation(context, summary, currentHand);
                summary.OpponentRemainingCards = CountCardsInHand(context, currentHand.round_id,
                    opponent.match_participant_id);
            }

            return summary;
        }

        private void FillCurrentHandInformation(CribbageEntities context, MatchInformationSummary summary,
            round currentHand)
        {
            long dealerParticipantId = currentHand.dealer_participant_id;

            summary.HandNumber = currentHand.round_number;
            summary.RunningCount = currentHand.current_count;
            summary.TurnDeadline = currentHand.turn_ends_at;

            match_participant dealer = context.match_participant
                .FirstOrDefault(currentParticipant => currentParticipant.match_participant_id == dealerParticipantId);

            if (dealer != null)
            {
                summary.DealerDisplayName = GetDisplayName(context, dealer.account_id);
            }

            if (currentHand.starter_card_id.HasValue)
            {
                byte cutCardId = currentHand.starter_card_id.Value;
                card cutCard = context.card.FirstOrDefault(currentCard => currentCard.card_id == cutCardId);

                if (cutCard != null)
                {
                    summary.CutCardText = CardTextFormatter.Format(cutCard.suit, cutCard.rank_value);
                }
            }
        }

        private int CountCardsInHand(CribbageEntities context, long handId, long participantId)
        {
            return context.round_card
                .Count(cardInHand => cardInHand.round_id == handId
                    && cardInHand.owner_participant_id == participantId
                    && cardInHand.card_status == CardInMatchState.InHand);
        }

        private string GetDisplayName(CribbageEntities context, int accountId)
        {
            string displayName = string.Empty;
            player_profile profile = context.player_profile
                .FirstOrDefault(currentProfile => currentProfile.account_id == accountId);

            if (profile != null)
            {
                displayName = profile.display_name;
            }

            return displayName;
        }

        private UnfinishedMatchSummary BuildUnfinishedMatch(CribbageEntities context, long matchId, int accountId)
        {
            UnfinishedMatchSummary summary = new UnfinishedMatchSummary();
            summary.MatchId = matchId;

            match_participant viewer = context.match_participant
                .FirstOrDefault(currentParticipant => currentParticipant.match_id == matchId
                    && currentParticipant.account_id == accountId);
            match_participant opponent = context.match_participant
                .FirstOrDefault(currentParticipant => currentParticipant.match_id == matchId
                    && currentParticipant.account_id != accountId);

            if (viewer != null && opponent != null)
            {
                List<long> handIds = context.round
                    .Where(hand => hand.match_id == matchId)
                    .Select(hand => hand.round_id)
                    .ToList();

                summary.ViewerScore = GetScore(context, handIds, viewer.match_participant_id);
                summary.OpponentScore = GetScore(context, handIds, opponent.match_participant_id);
                summary.OpponentDisplayName = GetDisplayName(context, opponent.account_id);
            }

            return summary;
        }

        private int GetScore(CribbageEntities context, List<long> handIds, long participantId)
        {
            int? total = context.score_entry
                .Where(score => handIds.Contains(score.round_id) && score.match_participant_id == participantId)
                .Sum(score => (int?)score.points);

            return total.GetValueOrDefault();
        }
    }
}