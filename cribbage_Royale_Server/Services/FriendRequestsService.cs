using System;
using System.ServiceModel;
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
        private const int FirstRegisteredAccountId = 1;
        private const string RequestFailureMessage =
            "The friend request could not be sent. CorrelationId: {CorrelationId}";

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

            try
            {
                EnsureTargetIsValid(caller.AccountId, accountId);
                EnsureRelationshipAllowsRequest(caller.AccountId, accountId);

                long requestId = _services.FriendshipRepository.CreateFriendRequest(caller.AccountId, accountId);
                NotifyRecipient(caller.AccountId, accountId, requestId);
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildServiceFault(dataAccessException);
            }
        }

        private void EnsureTargetIsValid(int senderAccountId, int recipientAccountId)
        {
            if (recipientAccountId < FirstRegisteredAccountId)
            {
                throw FriendshipFaultBuilder.Build(FriendshipFailure.InvalidAccountId);
            }

            if (recipientAccountId == senderAccountId)
            {
                throw FriendshipFaultBuilder.Build(FriendshipFailure.CannotTargetSelf);
            }

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

        private void NotifyRecipient(int senderAccountId, int recipientAccountId, long requestId)
        {
            SocialEventRecord record = new SocialEventRecord(senderAccountId, requestId, DateTime.UtcNow);
            SocialEvent social = _services.StateProjection.BuildSocialEvent(record);
            Notification notification = new SocialNotification(SocialEventType.FriendRequestReceived, social);

            _services.PlayerChannel.TrySendNotification(recipientAccountId, notification);
        }

        private FaultException<ServiceFault> BuildServiceFault(DataAccessException dataAccessException)
        {
            FaultException<ServiceFault> faultException =
                ServiceFaultBuilder.Build(ServiceFaultCode.DataAccessFailure);

            _logger.LogError(dataAccessException, RequestFailureMessage, faultException.Detail.CorrelationId);

            return faultException;
        }
    }
}