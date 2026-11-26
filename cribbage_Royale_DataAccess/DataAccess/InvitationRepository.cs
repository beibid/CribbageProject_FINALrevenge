using System;
using System.Data.Entity;
using System.Linq;
using cribbage_Royale_DataAccess.Models;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class InvitationRepository
    {
        private const int InvitationLifetimeSeconds = 60;
        private const int FirstHandNumber = 1;

        public InvitationCreationResult CreateInvitation(int senderAccountId, int recipientAccountId)
        {
            InvitationCreationResult result = new InvitationCreationResult();

            using (CribbageEntities context = new CribbageEntities())
            {
                DateTime now = DateTime.Now;
                result.Outcome = EvaluateRestrictions(context, senderAccountId, recipientAccountId);

                if (result.Outcome == InvitationCreationOutcome.Created)
                {
                    match_invitation newInvitation = new match_invitation();
                    newInvitation.sender_account_id = senderAccountId;
                    newInvitation.recipient_account_id = recipientAccountId;
                    newInvitation.status = InvitationStatus.Pending;
                    newInvitation.sent_at = now;
                    newInvitation.expires_at = now.AddSeconds(InvitationLifetimeSeconds);
                    context.match_invitation.Add(newInvitation);
                    context.SaveChanges();

                    result.InvitationId = newInvitation.match_invitation_id;
                }
            }

            return result;
        }

        public MatchInvitationSummary GetInvitation(long invitationId)
        {
            MatchInvitationSummary summary = null;

            using (CribbageEntities context = new CribbageEntities())
            {
                match_invitation invitation = context.match_invitation
                    .FirstOrDefault(currentInvitation => currentInvitation.match_invitation_id == invitationId);

                if (invitation != null)
                {
                    int senderAccountId = invitation.sender_account_id;
                    player_profile senderProfile = context.player_profile
                        .FirstOrDefault(profile => profile.account_id == senderAccountId);

                    summary = new MatchInvitationSummary();
                    summary.InvitationId = invitation.match_invitation_id;
                    summary.SenderAccountId = senderAccountId;
                    summary.RecipientAccountId = invitation.recipient_account_id;
                    summary.ExpirationDate = invitation.expires_at;
                    summary.MatchId = invitation.match_id;
                    summary.Status = invitation.status;

                    if (invitation.status == InvitationStatus.Pending
                        && invitation.expires_at <= DateTime.Now)
                    {
                        summary.Status = InvitationStatus.Expired;
                    }

                    if (senderProfile != null)
                    {
                        summary.SenderDisplayName = senderProfile.display_name;
                        summary.SenderSkillLevel = senderProfile.skill_level;
                    }
                }
            }

            return summary;
        }

        public InvitationResponseResult AcceptInvitation(long invitationId, int recipientAccountId)
        {
            InvitationResponseResult result = new InvitationResponseResult();
            result.Outcome = InvitationAcceptOutcome.NoLongerAvailable;

            using (CribbageEntities context = new CribbageEntities())
            {
                match_invitation invitation = context.match_invitation
                    .FirstOrDefault(currentInvitation => currentInvitation.match_invitation_id == invitationId);

                bool isAvailable = invitation != null
                    && invitation.status == InvitationStatus.Pending
                    && invitation.expires_at > DateTime.Now
                    && invitation.recipient_account_id == recipientAccountId
                    && !MatchRepository.HasUnfinishedMatchInContext(context, invitation.sender_account_id);

                if (isAvailable && MatchRepository.HasUnfinishedMatchInContext(context, recipientAccountId))
                {
                    result.Outcome = InvitationAcceptOutcome.HasUnfinishedMatch;
                }
                else if (isAvailable)
                {
                    result.MatchId = CreateMatchFromInvitation(context, invitation);
                    result.Outcome = InvitationAcceptOutcome.Accepted;
                }
            }

            return result;
        }

        public bool RejectInvitation(long invitationId)
        {
            return UpdatePendingStatus(invitationId, InvitationStatus.Rejected);
        }

        public bool CancelInvitation(long invitationId)
        {
            return UpdatePendingStatus(invitationId, InvitationStatus.Cancelled);
        }

        public bool ExpireInvitation(long invitationId)
        {
            return UpdatePendingStatus(invitationId, InvitationStatus.Expired);
        }

        private string EvaluateRestrictions(CribbageEntities context, int senderAccountId, int recipientAccountId)
        {
            string outcome = InvitationCreationOutcome.Created;
            DateTime now = DateTime.Now;

            bool recipientIsOnline = context.session
                .Any(currentSession => currentSession.account_id == recipientAccountId
                    && currentSession.revoked_at == null
                    && currentSession.expires_at > now);
            bool alreadyInvited = context.match_invitation
                .Any(currentInvitation => currentInvitation.sender_account_id == senderAccountId
                    && currentInvitation.recipient_account_id == recipientAccountId
                    && currentInvitation.status == InvitationStatus.Pending
                    && currentInvitation.expires_at > now);

            if (MatchRepository.HasUnfinishedMatchInContext(context, senderAccountId))
            {
                outcome = InvitationCreationOutcome.SenderHasUnfinishedMatch;
            }
            else if (MatchRepository.HasUnfinishedMatchInContext(context, recipientAccountId))
            {
                outcome = InvitationCreationOutcome.RecipientHasUnfinishedMatch;
            }
            else if (!recipientIsOnline)
            {
                outcome = InvitationCreationOutcome.RecipientOffline;
            }
            else if (alreadyInvited)
            {
                outcome = InvitationCreationOutcome.AlreadyInvitedByYou;
            }

            return outcome;
        }

        private long CreateMatchFromInvitation(CribbageEntities context, match_invitation invitation)
        {
            long matchId = 0;

            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                DateTime now = DateTime.Now;

                match newMatch = new match();
                newMatch.match_status = MatchStatus.InProgress;
                newMatch.origin = MatchOrigin.Invitation;
                newMatch.started_at = now;
                context.match.Add(newMatch);
                context.SaveChanges();

                match_participant senderParticipant = new match_participant();
                senderParticipant.match_id = newMatch.match_id;
                senderParticipant.account_id = invitation.sender_account_id;
                match_participant recipientParticipant = new match_participant();
                recipientParticipant.match_id = newMatch.match_id;
                recipientParticipant.account_id = invitation.recipient_account_id;
                context.match_participant.Add(senderParticipant);
                context.match_participant.Add(recipientParticipant);
                context.SaveChanges();

                Random randomGenerator = new Random();
                match_participant dealer = senderParticipant;

                if (randomGenerator.Next(2) == 1)
                {
                    dealer = recipientParticipant;
                }

                round firstHand = new round();
                firstHand.match_id = newMatch.match_id;
                firstHand.round_number = FirstHandNumber;
                firstHand.dealer_participant_id = dealer.match_participant_id;
                firstHand.round_status = HandState.Dealing;
                firstHand.started_at = now;
                context.round.Add(firstHand);

                invitation.status = InvitationStatus.Accepted;
                invitation.match_id = newMatch.match_id;
                context.SaveChanges();

                transaction.Commit();
                matchId = newMatch.match_id;
            }

            return matchId;
        }

        private bool UpdatePendingStatus(long invitationId, string newStatus)
        {
            bool updated = false;

            using (CribbageEntities context = new CribbageEntities())
            {
                match_invitation invitation = context.match_invitation
                    .FirstOrDefault(currentInvitation => currentInvitation.match_invitation_id == invitationId);

                if (invitation != null && invitation.status == InvitationStatus.Pending)
                {
                    invitation.status = newStatus;
                    context.SaveChanges();
                    updated = true;
                }
            }

            return updated;
        }
    }
}