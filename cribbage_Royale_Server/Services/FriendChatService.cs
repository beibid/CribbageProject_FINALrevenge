using System;
using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using CribbageRoyale.Exceptions;
using CribbageRoyale.Server.Connections;
using CribbageRoyale.Server.Faults;
using CribbageRoyale.Server.Sessions;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class FriendChatService : IFriendChat
    {
        private const int FirstRegisteredAccountId = 1;
        private const int MaximumMessageLength = 500;
        private const string DeliveryFailureMessage =
            "The chat message could not be delivered. CorrelationId: {CorrelationId}";

        private readonly FriendChatServices _services;
        private readonly ILogger _logger;

        public FriendChatService(FriendChatServices services, ILogger logger)
        {
            _services = services;
            _logger = logger;
        }

        public ChatMessage SendMessage(int accountId, string text)
        {
            SessionIdentity caller = _services.CallerSession.RequireRegisteredAccount();
            ChatMessage message;

            try
            {
                EnsureTargetIsValid(caller.AccountId, accountId);
                EnsureMessageTextIsValid(text);
                EnsureRelationshipAllowsChat(caller.AccountId, accountId);

                message = _services.StateProjection.BuildChatMessage(
                    caller.AccountId,
                    text,
                    DateTime.UtcNow);

                Deliver(accountId, message);
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildServiceFault(dataAccessException, DeliveryFailureMessage);
            }

            return message;
        }

        private void EnsureTargetIsValid(int senderAccountId, int recipientAccountId)
        {
            if (recipientAccountId < FirstRegisteredAccountId)
            {
                throw ChatFaultBuilder.Build(ChatFailure.InvalidAccountId);
            }

            if (recipientAccountId == senderAccountId)
            {
                throw ChatFaultBuilder.Build(ChatFailure.CannotTargetSelf);
            }
        }

        private void EnsureMessageTextIsValid(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length > MaximumMessageLength)
            {
                throw ChatFaultBuilder.Build(ChatFailure.InvalidMessageText);
            }
        }

        private void EnsureRelationshipAllowsChat(int senderAccountId, int recipientAccountId)
        {
            if (_services.FriendshipRepository.HasBlock(senderAccountId, recipientAccountId))
            {
                throw ChatFaultBuilder.Build(ChatFailure.BlockedRelationship);
            }

            if (!_services.FriendshipRepository.AreFriends(senderAccountId, recipientAccountId))
            {
                throw ChatFaultBuilder.Build(ChatFailure.FriendshipNotFound);
            }
        }

        private void Deliver(int recipientAccountId, ChatMessage message)
        {
            Notification notification = new ChatNotification(message);

            if (!_services.PlayerChannel.TrySendNotification(recipientAccountId, notification))
            {
                throw ChatFaultBuilder.Build(ChatFailure.RecipientUnavailable);
            }
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