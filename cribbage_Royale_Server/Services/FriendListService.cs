using System.Collections.Generic;
using System.ServiceModel;
using System.Text.RegularExpressions;
using cribbage_Royale_DataAccess.DataAccess;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using CribbageRoyale.Exceptions;
using CribbageRoyale.Server.Faults;
using CribbageRoyale.Server.Projection;
using CribbageRoyale.Server.Sessions;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class FriendListService : IFriendList
    {
        private const string SearchTextPattern = @"^[A-Za-z0-9_]{1,16}$";
        private const string FriendsQueryFailureMessage =
            "The friends list could not be returned. CorrelationId: {CorrelationId}";
        private const string PlayerSearchFailureMessage =
            "The player search could not be completed. CorrelationId: {CorrelationId}";

        private readonly FriendListServices _services;
        private readonly ILogger _logger;

        public FriendListService(FriendListServices services, ILogger logger)
        {
            _services = services;
            _logger = logger;
        }

        public PlayerSummary[] FindPlayers(string searchText)
        {
            SessionIdentity caller = _services.CallerSession.RequireRegisteredAccount();
            EnsureSearchTextIsValid(searchText);

            PlayerSummary[] players;

            try
            {
                List<PlayerRecord> records = _services.FriendshipRepository.SearchPlayers(caller.AccountId, searchText);
                RelationshipSnapshot relationships = LoadRelationships(caller.AccountId);
                players = BuildPlayerSummaries(records, relationships);
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildServiceFault(dataAccessException, PlayerSearchFailureMessage);
            }

            return players;
        }

        public FriendSummary[] GetFriends()
        {
            SessionIdentity caller = _services.CallerSession.RequireRegisteredAccount();
            FriendSummary[] friends;

            try
            {
                List<FriendRecord> records = _services.FriendshipRepository.GetFriends(caller.AccountId);
                friends = BuildFriendSummaries(records);
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildServiceFault(dataAccessException, FriendsQueryFailureMessage);
            }

            return friends;
        }

        private void EnsureSearchTextIsValid(string searchText)
        {
            if (searchText == null || !Regex.IsMatch(searchText, SearchTextPattern))
            {
                throw FriendshipFaultBuilder.Build(FriendshipFailure.InvalidSearchText);
            }
        }

        private RelationshipSnapshot LoadRelationships(int accountId)
        {
            return new RelationshipSnapshot(
                _services.FriendshipRepository.GetFriendIds(accountId),
                _services.FriendshipRepository.GetSentRequestRecipientIds(accountId),
                _services.FriendshipRepository.GetReceivedRequestSenderIds(accountId));
        }

        private PlayerSummary[] BuildPlayerSummaries(List<PlayerRecord> records, RelationshipSnapshot relationships)
        {
            List<PlayerSummary> summaries = new List<PlayerSummary>();

            foreach (PlayerRecord record in records)
            {
                PlayerSummary summary = new PlayerSummary();
                summary.Account = RecordProjection.BuildAccount(record);
                summary.HasProfileImage = record.HasProfileImage;
                summary.Relationship = relationships.Resolve(record.AccountId);
                summaries.Add(summary);
            }

            return summaries.ToArray();
        }

        private FriendSummary[] BuildFriendSummaries(List<FriendRecord> records)
        {
            List<FriendSummary> summaries = new List<FriendSummary>();

            foreach (FriendRecord record in records)
            {
                summaries.Add(BuildFriendSummary(record));
            }

            return summaries.ToArray();
        }

        private FriendSummary BuildFriendSummary(FriendRecord record)
        {
            FriendSummary summary = new FriendSummary();
            summary.Account = RecordProjection.BuildAccount(record);
            summary.HasProfileImage = record.HasProfileImage;
            summary.IsOnline = _services.PlayerChannel.IsPlayerConnected(record.AccountId);
            summary.FriendsSinceUtc = RecordProjection.ToUtc(record.FriendsSince);

            return summary;
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