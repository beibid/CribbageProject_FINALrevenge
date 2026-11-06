using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using CribbageRoyale.Exceptions;
using CribbageRoyale.Server.Connections;
using CribbageRoyale.Server.Faults;
using CribbageRoyale.Server.Matches;
using CribbageRoyale.Server.Sessions;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class MatchInvitationService : IMatchInvitation
    {
        private const int FirstRegisteredAccountId = 1;
        private const string InvitationFailureMessage =
            "The match invitation could not be processed. CorrelationId: {CorrelationId}";

        private readonly MatchInvitationServices _services;
        private readonly ILogger _logger;

        public MatchInvitationService(MatchInvitationServices services, ILogger logger)
        {
            _services = services;
            _logger = logger;
        }

        public MatchInvitation SendInvitation(int accountId)
        {
            SessionIdentity caller = _services.CallerSession.RequireRegisteredAccount();
            MatchInvitation invitation;

            try
            {
                EnsureTargetIsValid(caller.AccountId, accountId);
                EnsureRelationshipAllowsInvitation(caller.AccountId, accountId);
                EnsureBothArePlayable(caller.AccountId, accountId);

                invitation = CreateInvitation(caller.AccountId, accountId);
                Deliver(accountId, invitation);
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildServiceFault(dataAccessException, InvitationFailureMessage);
            }

            return invitation;
        }

        public void RespondToInvitation(long invitationId, bool accepted)
        {
            SessionIdentity caller = _services.CallerSession.RequireRegisteredAccount();
            PendingInvitation invitation = TakeReceivedInvitation(invitationId, caller.AccountId);

            try
            {
                Answer(invitation, accepted);
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildServiceFault(dataAccessException, InvitationFailureMessage);
            }
        }

        public void CancelInvitation(long invitationId)
        {
            SessionIdentity caller = _services.CallerSession.RequireRegisteredAccount();
            PendingInvitation invitation = TakeSentInvitation(invitationId, caller.AccountId);

            try
            {
                NotifyWithdrawn(invitation);
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildServiceFault(dataAccessException, InvitationFailureMessage);
            }
        }

        private void EnsureTargetIsValid(int senderAccountId, int recipientAccountId)
        {
            if (recipientAccountId < FirstRegisteredAccountId)
            {
                throw InvitationFaultBuilder.Build(InvitationFailure.InvalidAccountId);
            }

            if (recipientAccountId == senderAccountId)
            {
                throw InvitationFaultBuilder.Build(InvitationFailure.CannotInviteSelf);
            }
        }

        private void EnsureRelationshipAllowsInvitation(int senderAccountId, int recipientAccountId)
        {
            if (_services.FriendshipRepository.HasBlock(senderAccountId, recipientAccountId))
            {
                throw InvitationFaultBuilder.Build(InvitationFailure.BlockedRelationship);
            }

            if (!_services.FriendshipRepository.AreFriends(senderAccountId, recipientAccountId))
            {
                throw InvitationFaultBuilder.Build(InvitationFailure.FriendshipNotFound);
            }
        }

        private void EnsureBothArePlayable(int senderAccountId, int recipientAccountId)
        {
            if (_services.MatchRooms.IsInMatch(senderAccountId))
            {
                throw InvitationFaultBuilder.Build(InvitationFailure.CallerBusy);
            }

            if (_services.MatchRooms.IsInMatch(recipientAccountId))
            {
                throw InvitationFaultBuilder.Build(InvitationFailure.RecipientBusy);
            }
        }

        private MatchInvitation CreateInvitation(int senderAccountId, int recipientAccountId)
        {
            PendingInvitation pending;

            if (!_services.InvitationRegistry.TryCreate(senderAccountId, recipientAccountId, out pending))
            {
                throw InvitationFaultBuilder.Build(InvitationFailure.InvitationAlreadySent);
            }

            return BuildInvitation(pending);
        }

        private MatchInvitation BuildInvitation(PendingInvitation pending)
        {
            return _services.StateProjection.BuildMatchInvitation(
                pending.InvitationId,
                pending.SenderAccountId,
                pending.RecipientAccountId);
        }

        private void Deliver(int recipientAccountId, MatchInvitation invitation)
        {
            Notification notification = new MatchInvitationNotification(invitation);
            bool delivered = _services.PlayerChannel.TrySendNotification(recipientAccountId, notification);

            if (!delivered)
            {
                TakeBack(invitation.InvitationId, recipientAccountId);

                throw InvitationFaultBuilder.Build(InvitationFailure.RecipientUnavailable);
            }
        }

        private void TakeBack(long invitationId, int recipientAccountId)
        {
            PendingInvitation discarded;

            _services.InvitationRegistry.TryTake(
                invitationId,
                pending => pending.RecipientAccountId == recipientAccountId,
                out discarded);
        }

        private PendingInvitation TakeReceivedInvitation(long invitationId, int recipientAccountId)
        {
            PendingInvitation invitation;

            bool taken = _services.InvitationRegistry.TryTake(
                invitationId,
                pending => pending.RecipientAccountId == recipientAccountId,
                out invitation);

            if (!taken)
            {
                throw InvitationFaultBuilder.Build(InvitationFailure.InvitationNotFound);
            }

            return invitation;
        }

        private PendingInvitation TakeSentInvitation(long invitationId, int senderAccountId)
        {
            PendingInvitation invitation;

            bool taken = _services.InvitationRegistry.TryTake(
                invitationId,
                pending => pending.SenderAccountId == senderAccountId,
                out invitation);

            if (!taken)
            {
                throw InvitationFaultBuilder.Build(InvitationFailure.InvitationNotFound);
            }

            return invitation;
        }

        private void Answer(PendingInvitation invitation, bool accepted)
        {
            if (accepted)
            {
                StartMatch(invitation);
            }
            else
            {
                NotifyDeclined(invitation);
            }
        }

        private void StartMatch(PendingInvitation invitation)
        {
            long matchId = _services.MatchRooms.StartMatch(
                invitation.SenderAccountId,
                invitation.RecipientAccountId);

            NotifyHost(invitation, matchId);
            NotifyGuest(invitation, matchId);
        }

        private void NotifyHost(PendingInvitation invitation, long matchId)
        {
            MatchStart matchStart = _services.StateProjection.BuildMatchStart(
                matchId,
                invitation.RecipientAccountId,
                true);

            Send(invitation.SenderAccountId, new MatchStartedNotification(matchStart));
        }

        private void NotifyGuest(PendingInvitation invitation, long matchId)
        {
            MatchStart matchStart = _services.StateProjection.BuildMatchStart(
                matchId,
                invitation.SenderAccountId,
                false);

            Send(invitation.RecipientAccountId, new MatchStartedNotification(matchStart));
        }

        private void NotifyDeclined(PendingInvitation invitation)
        {
            MatchInvitation declined = BuildInvitation(invitation);

            Send(invitation.SenderAccountId, new MatchInvitationDeclinedNotification(declined));
        }

        private void NotifyWithdrawn(PendingInvitation invitation)
        {
            MatchInvitation withdrawn = BuildInvitation(invitation);

            Send(invitation.RecipientAccountId, new MatchInvitationDeclinedNotification(withdrawn));
        }

        private void Send(int accountId, Notification notification)
        {
            _services.PlayerChannel.TrySendNotification(accountId, notification);
        }

        private FaultException<ServiceFault> BuildServiceFault(
            DataAccessException dataAccessException,
            string message)
        {
            FaultException<ServiceFault> faultException =
                ServiceFaultBuilder.Build(ServiceFaultCode.DataAccessFailure);

            _logger.LogError(dataAccessException, message, faultException.Detail.CorrelationId);

            return faultException;
        }
    }
}