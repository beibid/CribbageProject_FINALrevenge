using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using CribbageRoyale.Exceptions;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private const string DefaultAvatar = "default_avatar";
        private const string VerifiedAccountStatus = "VERIFIED";
        private const int MaximumSearchResults = 20;
        private const string FriendsQueryFailureMessage = "The friends list could not be queried.";
        private const string RequestCreationFailureMessage = "The friend request could not be saved.";
        private const string RequestResponseFailureMessage = "The response to the friend request could not be saved.";
        private const string RequestCancellationFailureMessage = "The friend request could not be cancelled.";
        private const string FriendRemovalFailureMessage = "The friendship could not be removed.";
        private const string RelationshipQueryFailureMessage =
            "The relationship between the accounts could not be queried.";
        private const string PlayerSearchFailureMessage = "The players could not be searched.";
        private const string PendingRequestsQueryFailureMessage = "The pending friend requests could not be queried.";

        public long CreateFriendRequest(int senderAccountId, int recipientAccountId)
        {
            return Execute(
                context => AddFriendRequest(context, senderAccountId, recipientAccountId),
                RequestCreationFailureMessage);
        }

        public bool TryAcceptFriendRequest(long requestId)
        {
            return Execute(context => AcceptPendingRequest(context, requestId), RequestResponseFailureMessage);
        }

        public bool TryRejectFriendRequest(long requestId)
        {
            return Execute(context => RemovePendingRequest(context, requestId), RequestResponseFailureMessage);
        }

        public bool TryCancelFriendRequest(long requestId)
        {
            return Execute(context => RemovePendingRequest(context, requestId), RequestCancellationFailureMessage);
        }

        public bool TryGetPendingRequest(long requestId, out FriendRequestRecord request)
        {
            List<FriendRequestRecord> requests = Execute(
                context => QueryPendingRequestRecords(context, requestId),
                RelationshipQueryFailureMessage);

            request = requests.FirstOrDefault();

            return request != null;
        }

        public List<PendingFriendRequestRecord> GetPendingFriendRequests(int accountId)
        {
            return Execute(
                context => QueryPendingRequestsOf(context, accountId),
                PendingRequestsQueryFailureMessage);
        }

        public void RespondToFriendRequest(int recipientAccountId, int senderAccountId, bool isAccepted)
        {
            ExecuteWrite(
                context => ResolveRequests(
                    context,
                    QueryPendingRequests(context, senderAccountId, recipientAccountId),
                    isAccepted),
                RequestResponseFailureMessage);
        }

        public void CancelFriendRequest(int senderAccountId, int recipientAccountId)
        {
            ExecuteWrite(
                context => RemoveFriendships(
                    context,
                    QueryPendingRequests(context, senderAccountId, recipientAccountId)),
                RequestCancellationFailureMessage);
        }

        public void RemoveFriend(int firstAccountId, int secondAccountId)
        {
            TryRemoveFriend(firstAccountId, secondAccountId);
        }

        public bool TryRemoveFriend(int firstAccountId, int secondAccountId)
        {
            return Execute(
                context => RemoveAcceptedFriendships(context, firstAccountId, secondAccountId),
                FriendRemovalFailureMessage);
        }

        public List<FriendRecord> GetFriends(int accountId)
        {
            return Execute(context => QueryFriends(context, accountId), FriendsQueryFailureMessage);
        }

        public List<int> GetFriendIds(int accountId)
        {
            return Execute(
                context => QueryFriendships(context, accountId).Keys.ToList(),
                FriendsQueryFailureMessage);
        }

        public List<PlayerRecord> SearchPlayers(int searcherAccountId, string searchText)
        {
            return Execute(
                context => QueryPlayers(context, searcherAccountId, searchText),
                PlayerSearchFailureMessage);
        }

        public List<int> GetSentRequestRecipientIds(int accountId)
        {
            return Execute(
                context => QuerySentRequestRecipientIds(context, accountId),
                RelationshipQueryFailureMessage);
        }

        public List<int> GetReceivedRequestSenderIds(int accountId)
        {
            return Execute(
                context => QueryReceivedRequestSenderIds(context, accountId),
                RelationshipQueryFailureMessage);
        }

        public bool IsAccountAvailable(int accountId)
        {
            return Execute(context => IsVerifiedAccount(context, accountId), RelationshipQueryFailureMessage);
        }

        public bool AreFriends(int firstAccountId, int secondAccountId)
        {
            return Execute(
                context => QueryAcceptedFriendships(context, firstAccountId, secondAccountId).Count > 0,
                RelationshipQueryFailureMessage);
        }

        public bool HasPendingRequest(int firstAccountId, int secondAccountId)
        {
            return Execute(
                context => HasPendingRequestInEitherDirection(context, firstAccountId, secondAccountId),
                RelationshipQueryFailureMessage);
        }

        public bool HasBlock(int firstAccountId, int secondAccountId)
        {
            return Execute(
                context => HasBlockInEitherDirection(context, firstAccountId, secondAccountId),
                RelationshipQueryFailureMessage);
        }

        private void ExecuteWrite(Action<CribbageEntities> writeOperation, string failureMessage)
        {
            try
            {
                using (CribbageEntities context = new CribbageEntities())
                {
                    writeOperation(context);
                }
            }
            catch (DbUpdateException updateException)
            {
                throw new DataAccessException(failureMessage, updateException);
            }
            catch (EntityException entityException)
            {
                throw new DataAccessException(failureMessage, entityException);
            }
            catch (SqlException sqlException)
            {
                throw new DataAccessException(failureMessage, sqlException);
            }
        }

        private TResult Execute<TResult>(Func<CribbageEntities, TResult> operation, string failureMessage)
        {
            TResult result;

            try
            {
                using (CribbageEntities context = new CribbageEntities())
                {
                    result = operation(context);
                }
            }
            catch (DbUpdateException updateException)
            {
                throw new DataAccessException(failureMessage, updateException);
            }
            catch (EntityException entityException)
            {
                throw new DataAccessException(failureMessage, entityException);
            }
            catch (SqlException sqlException)
            {
                throw new DataAccessException(failureMessage, sqlException);
            }

            return result;
        }

        private long AddFriendRequest(CribbageEntities context, int senderAccountId, int recipientAccountId)
        {
            friendship friendship = new friendship();
            friendship.requester_account_id = senderAccountId;
            friendship.addressee_account_id = recipientAccountId;
            friendship.status = FriendshipStatus.Pending;
            friendship.requested_at = DateTime.Now;

            context.friendship.Add(friendship);
            context.SaveChanges();

            return friendship.friendship_id;
        }

        private void ResolveRequests(CribbageEntities context, List<friendship> requests, bool isAccepted)
        {
            foreach (friendship request in requests)
            {
                if (isAccepted)
                {
                    AcceptRequest(request);
                }
                else
                {
                    context.friendship.Remove(request);
                }
            }

            context.SaveChanges();
        }

        private void AcceptRequest(friendship request)
        {
            request.status = FriendshipStatus.Accepted;
            request.responded_at = DateTime.Now;
        }

        private void RemoveFriendships(CribbageEntities context, List<friendship> friendships)
        {
            context.friendship.RemoveRange(friendships);
            context.SaveChanges();
        }

        private bool AcceptPendingRequest(CribbageEntities context, long requestId)
        {
            List<friendship> requests = QueryPendingRequestById(context, requestId);

            foreach (friendship request in requests)
            {
                AcceptRequest(request);
            }

            context.SaveChanges();

            return requests.Count > 0;
        }

        private bool RemoveAcceptedFriendships(CribbageEntities context, int firstAccountId, int secondAccountId)
        {
            List<friendship> friendships = QueryAcceptedFriendships(context, firstAccountId, secondAccountId);
            RemoveFriendships(context, friendships);

            return friendships.Count > 0;
        }

        private bool RemovePendingRequest(CribbageEntities context, long requestId)
        {
            List<friendship> requests = QueryPendingRequestById(context, requestId);
            RemoveFriendships(context, requests);

            return requests.Count > 0;
        }

        private List<friendship> QueryPendingRequestById(CribbageEntities context, long requestId)
        {
            return context.friendship
                .Where(friendship => friendship.friendship_id == requestId)
                .Where(friendship => friendship.status == FriendshipStatus.Pending)
                .ToList();
        }

        private List<FriendRequestRecord> QueryPendingRequestRecords(CribbageEntities context, long requestId)
        {
            return QueryPendingRequestById(context, requestId)
                .Select(request => BuildRequestRecord(request))
                .ToList();
        }

        private FriendRequestRecord BuildRequestRecord(friendship request)
        {
            FriendRequestRecord record = new FriendRequestRecord();
            record.RequestId = request.friendship_id;
            record.SenderAccountId = request.requester_account_id;
            record.RecipientAccountId = request.addressee_account_id;

            return record;
        }

        private List<PendingFriendRequestRecord> QueryPendingRequestsOf(CribbageEntities context, int accountId)
        {
            List<friendship> requests = QueryPendingRequestsInvolving(context, accountId);
            List<int> counterpartIds = requests.Select(request => ResolveCounterpartId(request, accountId)).ToList();

            Dictionary<int, account> accounts = context.account
                .Where(account => counterpartIds.Contains(account.account_id))
                .ToDictionary(account => account.account_id);
            List<player_profile> profiles = QueryProfiles(context, counterpartIds);

            List<PendingFriendRequestRecord> records = new List<PendingFriendRequestRecord>();

            foreach (friendship request in requests)
            {
                PendingFriendRequestRecord record = BuildPendingRecord(request, accountId);
                account account;

                if (accounts.TryGetValue(record.AccountId, out account))
                {
                    FillRecord(record, account, profiles);
                    records.Add(record);
                }
            }

            return records;
        }

        private List<friendship> QueryPendingRequestsInvolving(CribbageEntities context, int accountId)
        {
            return context.friendship
                .Where(friendship => friendship.status == FriendshipStatus.Pending)
                .Where(friendship => friendship.requester_account_id == accountId
                    || friendship.addressee_account_id == accountId)
                .OrderByDescending(friendship => friendship.requested_at)
                .ToList();
        }

        private PendingFriendRequestRecord BuildPendingRecord(friendship request, int accountId)
        {
            PendingFriendRequestRecord record = new PendingFriendRequestRecord();
            record.RequestId = request.friendship_id;
            record.AccountId = ResolveCounterpartId(request, accountId);
            record.IsReceived = request.addressee_account_id == accountId;
            record.RequestedAt = request.requested_at;

            return record;
        }

        private int ResolveCounterpartId(friendship request, int accountId)
        {
            int counterpartId = request.requester_account_id;

            if (request.requester_account_id == accountId)
            {
                counterpartId = request.addressee_account_id;
            }

            return counterpartId;
        }

        private List<friendship> QueryPendingRequests(
            CribbageEntities context,
            int senderAccountId,
            int recipientAccountId)
        {
            return context.friendship
                .Where(friendship => friendship.status == FriendshipStatus.Pending)
                .Where(friendship => friendship.requester_account_id == senderAccountId)
                .Where(friendship => friendship.addressee_account_id == recipientAccountId)
                .ToList();
        }

        private List<friendship> QueryAcceptedFriendships(
            CribbageEntities context,
            int firstAccountId,
            int secondAccountId)
        {
            List<friendship> friendships = QueryAcceptedInDirection(context, firstAccountId, secondAccountId);
            friendships.AddRange(QueryAcceptedInDirection(context, secondAccountId, firstAccountId));

            return friendships;
        }

        private List<friendship> QueryAcceptedInDirection(
            CribbageEntities context,
            int senderAccountId,
            int recipientAccountId)
        {
            return context.friendship
                .Where(friendship => friendship.status == FriendshipStatus.Accepted)
                .Where(friendship => friendship.requester_account_id == senderAccountId)
                .Where(friendship => friendship.addressee_account_id == recipientAccountId)
                .ToList();
        }

        private bool IsVerifiedAccount(CribbageEntities context, int accountId)
        {
            return context.account
                .Where(account => account.account_id == accountId)
                .Any(account => account.account_status == VerifiedAccountStatus);
        }

        private bool HasPendingRequestInEitherDirection(
            CribbageEntities context,
            int firstAccountId,
            int secondAccountId)
        {
            int pendingRequests = QueryPendingRequests(context, firstAccountId, secondAccountId).Count
                + QueryPendingRequests(context, secondAccountId, firstAccountId).Count;

            return pendingRequests > 0;
        }

        private bool HasBlockInEitherDirection(CribbageEntities context, int firstAccountId, int secondAccountId)
        {
            return HasBlockInDirection(context, firstAccountId, secondAccountId)
                || HasBlockInDirection(context, secondAccountId, firstAccountId);
        }

        private bool HasBlockInDirection(CribbageEntities context, int blockingAccountId, int blockedAccountId)
        {
            return context.user_block
                .Where(block => block.blocker_account_id == blockingAccountId)
                .Any(block => block.blocked_account_id == blockedAccountId);
        }

        private List<FriendRecord> QueryFriends(CribbageEntities context, int accountId)
        {
            Dictionary<int, DateTime?> friendships = QueryFriendships(context, accountId);
            List<int> friendIds = friendships.Keys.ToList();

            List<account> accounts = context.account
                .Where(account => friendIds.Contains(account.account_id))
                .ToList();
            List<player_profile> profiles = QueryProfiles(context, friendIds);

            List<FriendRecord> friends = new List<FriendRecord>();

            foreach (account account in accounts)
            {
                FriendRecord friend = new FriendRecord();
                FillRecord(friend, account, profiles);
                friend.FriendsSince = friendships[account.account_id];
                friends.Add(friend);
            }

            return friends;
        }

        private List<PlayerRecord> QueryPlayers(CribbageEntities context, int searcherAccountId, string searchText)
        {
            List<int> blockedAccountIds = QueryBlockedCounterpartIds(context, searcherAccountId);

            List<account> accounts = context.account
                .Where(account => account.username.Contains(searchText))
                .Where(account => account.account_status == VerifiedAccountStatus)
                .Where(account => account.account_id != searcherAccountId)
                .Where(account => !blockedAccountIds.Contains(account.account_id))
                .OrderBy(account => account.username)
                .Take(MaximumSearchResults)
                .ToList();

            List<int> accountIds = accounts.Select(account => account.account_id).ToList();
            List<player_profile> profiles = QueryProfiles(context, accountIds);

            List<PlayerRecord> players = new List<PlayerRecord>();

            foreach (account account in accounts)
            {
                PlayerRecord player = new PlayerRecord();
                FillRecord(player, account, profiles);
                players.Add(player);
            }

            return players;
        }

        private List<int> QueryBlockedCounterpartIds(CribbageEntities context, int accountId)
        {
            List<int> counterpartIds = context.user_block
                .Where(block => block.blocker_account_id == accountId)
                .Select(block => block.blocked_account_id)
                .ToList();

            counterpartIds.AddRange(context.user_block
                .Where(block => block.blocked_account_id == accountId)
                .Select(block => block.blocker_account_id)
                .ToList());

            return counterpartIds;
        }

        private List<int> QuerySentRequestRecipientIds(CribbageEntities context, int accountId)
        {
            return context.friendship
                .Where(friendship => friendship.status == FriendshipStatus.Pending)
                .Where(friendship => friendship.requester_account_id == accountId)
                .Select(friendship => friendship.addressee_account_id)
                .ToList();
        }

        private List<int> QueryReceivedRequestSenderIds(CribbageEntities context, int accountId)
        {
            return context.friendship
                .Where(friendship => friendship.status == FriendshipStatus.Pending)
                .Where(friendship => friendship.addressee_account_id == accountId)
                .Select(friendship => friendship.requester_account_id)
                .ToList();
        }

        private List<player_profile> QueryProfiles(CribbageEntities context, List<int> accountIds)
        {
            return context.player_profile
                .Where(profile => accountIds.Contains(profile.account_id))
                .ToList();
        }

        private Dictionary<int, DateTime?> QueryFriendships(CribbageEntities context, int accountId)
        {
            List<friend_view> rows = context.friend_view
                .Where(friendship => friendship.account_id == accountId || friendship.friend_account_id == accountId)
                .ToList();

            Dictionary<int, DateTime?> friendships = new Dictionary<int, DateTime?>();

            foreach (friend_view row in rows)
            {
                friendships[ResolveFriendId(row, accountId)] = row.responded_at;
            }

            friendships.Remove(accountId);

            return friendships;
        }

        private int ResolveFriendId(friend_view friendship, int accountId)
        {
            int friendId = friendship.account_id;

            if (friendship.account_id == accountId)
            {
                friendId = friendship.friend_account_id;
            }

            return friendId;
        }

        private void FillRecord(PlayerRecord record, account account, List<player_profile> profiles)
        {
            player_profile profile = profiles.FirstOrDefault(candidate => candidate.account_id == account.account_id);

            record.AccountId = account.account_id;
            record.UserName = account.username;
            record.DisplayName = account.username;
            record.HasProfileImage = false;

            if (profile != null)
            {
                record.DisplayName = profile.display_name;
                record.HasProfileImage = HasCustomAvatar(profile);
            }
        }

        private bool HasCustomAvatar(player_profile profile)
        {
            return !string.IsNullOrEmpty(profile.avatar) && profile.avatar != DefaultAvatar;
        }
    }
}
