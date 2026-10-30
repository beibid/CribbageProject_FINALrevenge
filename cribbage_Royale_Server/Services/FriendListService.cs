using System;
using System.Collections.Generic;
using System.ServiceModel;
using cribbage_Royale_DataAccess.DataAccess;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using CribbageRoyale.Exceptions;
using CribbageRoyale.Server.Faults;
using CribbageRoyale.Server.Sessions;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class FriendListService : IFriendList
    {
        private const string FriendsQueryFailureMessage =
            "The friends list could not be returned. CorrelationId: {CorrelationId}";

        private readonly FriendListServices _services;
        private readonly ILogger _logger;

        public FriendListService(FriendListServices services, ILogger logger)
        {
            _services = services;
            _logger = logger;
        }

        public FriendSummary[] GetFriends()
        {
            SessionIdentity caller = _services.CallerSession.RequireRegisteredAccount();
            FriendSummary[] friends;

            try
            {
                List<FriendRecord> records = _services.FriendshipRepository.GetFriends(caller.AccountId);
                friends = BuildSummaries(records);
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildServiceFault(dataAccessException);
            }

            return friends;
        }

        private FriendSummary[] BuildSummaries(List<FriendRecord> records)
        {
            List<FriendSummary> summaries = new List<FriendSummary>();

            foreach (FriendRecord record in records)
            {
                summaries.Add(BuildSummary(record));
            }

            return summaries.ToArray();
        }

        private FriendSummary BuildSummary(FriendRecord record)
        {
            AccountReference account = new AccountReference();
            account.AccountId = record.AccountId;
            account.UserName = record.UserName;
            account.DisplayName = record.DisplayName;

            FriendSummary summary = new FriendSummary();
            summary.Account = account;
            summary.HasProfileImage = record.HasProfileImage;
            summary.IsOnline = _services.PlayerChannel.IsPlayerConnected(record.AccountId);
            summary.FriendsSinceUtc = ResolveFriendsSinceUtc(record.FriendsSince);

            return summary;
        }

        private DateTime ResolveFriendsSinceUtc(DateTime? friendsSince)
        {
            DateTime friendsSinceUtc = DateTime.MinValue;

            if (friendsSince.HasValue)
            {
                friendsSinceUtc = friendsSince.Value.ToUniversalTime();
            }

            return friendsSinceUtc;
        }

        private FaultException<ServiceFault> BuildServiceFault(DataAccessException dataAccessException)
        {
            FaultException<ServiceFault> faultException =
                ServiceFaultBuilder.Build(ServiceFaultCode.DataAccessFailure);

            _logger.LogError(dataAccessException, FriendsQueryFailureMessage, faultException.Detail.CorrelationId);

            return faultException;
        }
    }
}