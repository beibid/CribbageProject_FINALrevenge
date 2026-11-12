using System;
using System.Collections.Generic;
using System.ServiceModel;
using cribbage_Royale_DataAccess.DataAccess;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using CribbageRoyale.Exceptions;
using CribbageRoyale.Server.Connections;
using CribbageRoyale.Server.Faults;
using CribbageRoyale.Server.Projection;
using CribbageRoyale.Server.Sessions;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class FriendRequestsService : IFriendRequests
    {
        private const long FirstRequestId = 1;
        private const string SendFailureMessage =
            "The friend request could not be sent. CorrelationId: {CorrelationId}";
        private const string ResponseFailureMessage =
            "The response to the friend request could not be saved. CorrelationId: {CorrelationId}";
        private const string CancellationFailureMessage =
            "The friend request could not be cancelled. CorrelationId: {CorrelationId}";
        private const string PendingQueryFailureMessage =
            "The pending friend requests could not be returned. CorrelationId: {CorrelationId}";

        private readonly FriendRequestsServices _services;
        private readonly ILogger _logger;

        public FriendRequestsService(FriendRequestsServices services, ILogger logger)
        {
            _services = services;
            _logger = logger;
        }

        public void SendFriendRequest(int accountId)
        {
            SessionIdentity caller = _services.CallerSession.RequireRegisteredAccount();
            FriendshipTargetRules.EnsureIsOtherAccount(caller.AccountId, accountId);

            try
            {
                EnsureTargetIsAvailable(accountId);
                EnsureRelationshipAllowsRequest(caller.AccountId, accountId);

                long requestId = _services.FriendshipRepository.CreateFriendRequest(caller.AccountId, accountId);
                SocialEventRecord record = new SocialEventRecord(caller.AccountId, requestId, DateTime.UtcNow);
                Notify(accountId, SocialEventType.FriendRequestReceived, record);
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildServiceFault(dataAccessException, SendFailureMessage);
            }
        }

        public void RespondToFriendRequest(long requestId, bool accepted)
        {
            SessionIdentity caller = _services.CallerSession.RequireRegisteredAccount();

            try
            {
                FriendRequestRecord request = RequirePendingRequest(requestId);
                EnsureCallerIs(request.RecipientAccountId, caller.AccountId);

                ApplyResponse(requestId, accepted);
                NotifySender(request, accepted);
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildServiceFault(dataAccessException, ResponseFailureMessage);
            }
        }

        public void CancelFriendRequest(long requestId)
        {
            SessionIdentity caller = _services.CallerSession.RequireRegisteredAccount();

            try
            {
                FriendRequestRecord request = RequirePendingRequest(requestId);
                EnsureCallerIs(request.SenderAccountId, caller.AccountId);

                EnsureRequestWasPending(_services.FriendshipRepository.TryCancelFriendRequest(requestId));
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildServiceFault(dataAccessException, CancellationFailureMessage);
            }
        }

        public FriendRequest[] GetPendingFriendRequests()
        {
            SessionIdentity caller = _services.CallerSession.RequireRegisteredAccount();
            FriendRequest[] requests;

            try
            {
                List<PendingFriendRequestRecord> records =
                    _services.FriendshipRepository.GetPendingFriendRequests(caller.AccountId);
                requests = BuildFriendRequests(records);
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildServiceFault(dataAccessException, PendingQueryFailureMessage);
            }

            return requests;
        }

        private void EnsureTargetIsAvailable(int recipientAccountId)
        {
            if (!_services.FriendshipRepository.IsAccountAvailable(recipientAccountId))
            {
                throw FriendshipFaultBuilder.Build(FriendshipFailure.AccountUnavailable);
            }
        }

        private void EnsureRelationshipAllowsRequest(int senderAccountId, int recipientAccountId)
        {
            if (_services.FriendshipRepository.HasBlock(senderAccountId, recipientAccountId))
            {
                throw FriendshipFaultBuilder.Build(FriendshipFailure.BlockedRelationship);
            }

            if (_services.FriendshipRepository.AreFriends(senderAccountId, recipientAccountId))
            {
                throw FriendshipFaultBuilder.Build(FriendshipFailure.FriendshipAlreadyExists);
            }

            if (_services.FriendshipRepository.HasPendingRequest(senderAccountId, recipientAccountId))
            {
                throw FriendshipFaultBuilder.Build(FriendshipFailure.RequestAlreadyPending);
            }
        }

        private FriendRequestRecord RequirePendingRequest(long requestId)
        {
            FriendRequestRecord request;

            if (requestId < FirstRequestId
                || !_services.FriendshipRepository.TryGetPendingRequest(requestId, out request))
            {
                throw FriendshipFaultBuilder.Build(FriendshipFailure.RequestUnavailable);
            }

            return request;
        }

        private void EnsureCallerIs(int authorizedAccountId, int callerAccountId)
        {
            if (authorizedAccountId != callerAccountId)
            {
                throw FriendshipFaultBuilder.Build(FriendshipFailure.UnauthorizedRequest);
            }
        }

        private void ApplyResponse(long requestId, bool accepted)
        {
            bool wasPending;

            if (accepted)
            {
                wasPending = _services.FriendshipRepository.TryAcceptFriendRequest(requestId);
            }
            else
            {
                wasPending = _services.FriendshipRepository.TryRejectFriendRequest(requestId);
            }

            EnsureRequestWasPending(wasPending);
        }

        private void EnsureRequestWasPending(bool wasPending)
        {
            if (!wasPending)
            {
                throw FriendshipFaultBuilder.Build(FriendshipFailure.RequestUnavailable);
            }
        }

        private void NotifySender(FriendRequestRecord request, bool accepted)
        {
            SocialEventRecord record = new SocialEventRecord(
                request.RecipientAccountId,
                request.RequestId,
                DateTime.UtcNow);

            Notify(request.SenderAccountId, ResolveResponseEventType(accepted), record);
        }

        private SocialEventType ResolveResponseEventType(bool accepted)
        {
            SocialEventType eventType = SocialEventType.FriendRequestRejected;

            if (accepted)
            {
                eventType = SocialEventType.FriendRequestAccepted;
            }

            return eventType;
        }

        private void Notify(int recipientAccountId, SocialEventType eventType, SocialEventRecord record)
        {
            SocialEvent social = _services.StateProjection.BuildSocialEvent(record);

            _services.PlayerChannel.TrySendNotification(
                recipientAccountId,
                callback => callback.SocialEventOccurred(eventType, social));
        }

        private FriendRequest[] BuildFriendRequests(List<PendingFriendRequestRecord> records)
        {
            List<FriendRequest> requests = new List<FriendRequest>();

            foreach (PendingFriendRequestRecord record in records)
            {
                requests.Add(BuildFriendRequest(record));
            }

            return requests.ToArray();
        }

        private FriendRequest BuildFriendRequest(PendingFriendRequestRecord record)
        {
            FriendRequest request = new FriendRequest();
            request.RequestId = record.RequestId;
            request.Counterpart = RecordProjection.BuildAccount(record);
            request.Direction = ResolveDirection(record.IsReceived);
            request.CreatedAtUtc = RecordProjection.ToUtc(record.RequestedAt);

            return request;
        }

        private RequestDirection ResolveDirection(bool isReceived)
        {
            RequestDirection direction = RequestDirection.Sent;

            if (isReceived)
            {
                direction = RequestDirection.Received;
            }

            return direction;
        }

        private FaultException<ServiceFault> BuildServiceFault(DataAccessException dataAccessException, string message)
        {
            FaultException<ServiceFault> faultException =
                ServiceFaultBuilder.Build(ServiceFaultCode.DataAccessFailure);

            _logger.LogError(dataAccessException, message, faultException.Detail.CorrelationId);

            return faultException;
        }
    }
}