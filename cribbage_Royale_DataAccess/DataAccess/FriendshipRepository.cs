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
        private const string DefaultAvatar = "avatar_por_omision";
        private const string VerifiedAccountStatus = "VERIFICADA";
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
            amistad friendship = new amistad();
            friendship.id_usuario_solicitante = senderAccountId;
            friendship.id_usuario_destinatario = recipientAccountId;
            friendship.estado = FriendshipStatus.Pending;
            friendship.fecha_solicitud = DateTime.Now;

            context.amistad.Add(friendship);
            context.SaveChanges();

            return friendship.id_amistad;
        }

        private void ResolveRequests(CribbageEntities context, List<amistad> requests, bool isAccepted)
        {
            foreach (amistad request in requests)
            {
                if (isAccepted)
                {
                    AcceptRequest(request);
                }
                else
                {
                    context.amistad.Remove(request);
                }
            }

            context.SaveChanges();
        }

        private void AcceptRequest(amistad request)
        {
            request.estado = FriendshipStatus.Accepted;
            request.fecha_respuesta = DateTime.Now;
        }

        private void RemoveFriendships(CribbageEntities context, List<amistad> friendships)
        {
            context.amistad.RemoveRange(friendships);
            context.SaveChanges();
        }

        private bool AcceptPendingRequest(CribbageEntities context, long requestId)
        {
            List<amistad> requests = QueryPendingRequestById(context, requestId);

            foreach (amistad request in requests)
            {
                AcceptRequest(request);
            }

            context.SaveChanges();

            return requests.Count > 0;
        }

        private bool RemoveAcceptedFriendships(CribbageEntities context, int firstAccountId, int secondAccountId)
        {
            List<amistad> friendships = QueryAcceptedFriendships(context, firstAccountId, secondAccountId);
            RemoveFriendships(context, friendships);

            return friendships.Count > 0;
        }

        private bool RemovePendingRequest(CribbageEntities context, long requestId)
        {
            List<amistad> requests = QueryPendingRequestById(context, requestId);
            RemoveFriendships(context, requests);

            return requests.Count > 0;
        }

        private List<amistad> QueryPendingRequestById(CribbageEntities context, long requestId)
        {
            return context.amistad
                .Where(friendship => friendship.id_amistad == requestId)
                .Where(friendship => friendship.estado == FriendshipStatus.Pending)
                .ToList();
        }

        private List<FriendRequestRecord> QueryPendingRequestRecords(CribbageEntities context, long requestId)
        {
            return QueryPendingRequestById(context, requestId)
                .Select(request => BuildRequestRecord(request))
                .ToList();
        }

        private FriendRequestRecord BuildRequestRecord(amistad request)
        {
            FriendRequestRecord record = new FriendRequestRecord();
            record.RequestId = request.id_amistad;
            record.SenderAccountId = request.id_usuario_solicitante;
            record.RecipientAccountId = request.id_usuario_destinatario;

            return record;
        }

        private List<PendingFriendRequestRecord> QueryPendingRequestsOf(CribbageEntities context, int accountId)
        {
            List<amistad> requests = QueryPendingRequestsInvolving(context, accountId);
            List<int> counterpartIds = requests.Select(request => ResolveCounterpartId(request, accountId)).ToList();

            Dictionary<int, usuario> accounts = context.usuario
                .Where(account => counterpartIds.Contains(account.id_usuario))
                .ToDictionary(account => account.id_usuario);
            List<perfil_jugador> profiles = QueryProfiles(context, counterpartIds);

            List<PendingFriendRequestRecord> records = new List<PendingFriendRequestRecord>();

            foreach (amistad request in requests)
            {
                PendingFriendRequestRecord record = BuildPendingRecord(request, accountId);
                usuario account;

                if (accounts.TryGetValue(record.AccountId, out account))
                {
                    FillRecord(record, account, profiles);
                    records.Add(record);
                }
            }

            return records;
        }

        private List<amistad> QueryPendingRequestsInvolving(CribbageEntities context, int accountId)
        {
            return context.amistad
                .Where(friendship => friendship.estado == FriendshipStatus.Pending)
                .Where(friendship => friendship.id_usuario_solicitante == accountId
                    || friendship.id_usuario_destinatario == accountId)
                .OrderByDescending(friendship => friendship.fecha_solicitud)
                .ToList();
        }

        private PendingFriendRequestRecord BuildPendingRecord(amistad request, int accountId)
        {
            PendingFriendRequestRecord record = new PendingFriendRequestRecord();
            record.RequestId = request.id_amistad;
            record.AccountId = ResolveCounterpartId(request, accountId);
            record.IsReceived = request.id_usuario_destinatario == accountId;
            record.RequestedAt = request.fecha_solicitud;

            return record;
        }

        private int ResolveCounterpartId(amistad request, int accountId)
        {
            int counterpartId = request.id_usuario_solicitante;

            if (request.id_usuario_solicitante == accountId)
            {
                counterpartId = request.id_usuario_destinatario;
            }

            return counterpartId;
        }

        private List<amistad> QueryPendingRequests(
            CribbageEntities context,
            int senderAccountId,
            int recipientAccountId)
        {
            return context.amistad
                .Where(friendship => friendship.estado == FriendshipStatus.Pending)
                .Where(friendship => friendship.id_usuario_solicitante == senderAccountId)
                .Where(friendship => friendship.id_usuario_destinatario == recipientAccountId)
                .ToList();
        }

        private List<amistad> QueryAcceptedFriendships(
            CribbageEntities context,
            int firstAccountId,
            int secondAccountId)
        {
            List<amistad> friendships = QueryAcceptedInDirection(context, firstAccountId, secondAccountId);
            friendships.AddRange(QueryAcceptedInDirection(context, secondAccountId, firstAccountId));

            return friendships;
        }

        private List<amistad> QueryAcceptedInDirection(
            CribbageEntities context,
            int senderAccountId,
            int recipientAccountId)
        {
            return context.amistad
                .Where(friendship => friendship.estado == FriendshipStatus.Accepted)
                .Where(friendship => friendship.id_usuario_solicitante == senderAccountId)
                .Where(friendship => friendship.id_usuario_destinatario == recipientAccountId)
                .ToList();
        }

        private bool IsVerifiedAccount(CribbageEntities context, int accountId)
        {
            return context.usuario
                .Where(account => account.id_usuario == accountId)
                .Any(account => account.estado_cuenta == VerifiedAccountStatus);
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
            return context.bloqueo
                .Where(block => block.id_usuario_que_bloquea == blockingAccountId)
                .Any(block => block.id_usuario_bloqueado == blockedAccountId);
        }

        private List<FriendRecord> QueryFriends(CribbageEntities context, int accountId)
        {
            Dictionary<int, DateTime?> friendships = QueryFriendships(context, accountId);
            List<int> friendIds = friendships.Keys.ToList();

            List<usuario> accounts = context.usuario
                .Where(account => friendIds.Contains(account.id_usuario))
                .ToList();
            List<perfil_jugador> profiles = QueryProfiles(context, friendIds);

            List<FriendRecord> friends = new List<FriendRecord>();

            foreach (usuario account in accounts)
            {
                FriendRecord friend = new FriendRecord();
                FillRecord(friend, account, profiles);
                friend.FriendsSince = friendships[account.id_usuario];
                friends.Add(friend);
            }

            return friends;
        }

        private List<PlayerRecord> QueryPlayers(CribbageEntities context, int searcherAccountId, string searchText)
        {
            List<int> blockedAccountIds = QueryBlockedCounterpartIds(context, searcherAccountId);

            List<usuario> accounts = context.usuario
                .Where(account => account.nombre_usuario.Contains(searchText))
                .Where(account => account.estado_cuenta == VerifiedAccountStatus)
                .Where(account => account.id_usuario != searcherAccountId)
                .Where(account => !blockedAccountIds.Contains(account.id_usuario))
                .OrderBy(account => account.nombre_usuario)
                .Take(MaximumSearchResults)
                .ToList();

            List<int> accountIds = accounts.Select(account => account.id_usuario).ToList();
            List<perfil_jugador> profiles = QueryProfiles(context, accountIds);

            List<PlayerRecord> players = new List<PlayerRecord>();

            foreach (usuario account in accounts)
            {
                PlayerRecord player = new PlayerRecord();
                FillRecord(player, account, profiles);
                players.Add(player);
            }

            return players;
        }

        private List<int> QueryBlockedCounterpartIds(CribbageEntities context, int accountId)
        {
            List<int> counterpartIds = context.bloqueo
                .Where(block => block.id_usuario_que_bloquea == accountId)
                .Select(block => block.id_usuario_bloqueado)
                .ToList();

            counterpartIds.AddRange(context.bloqueo
                .Where(block => block.id_usuario_bloqueado == accountId)
                .Select(block => block.id_usuario_que_bloquea)
                .ToList());

            return counterpartIds;
        }

        private List<int> QuerySentRequestRecipientIds(CribbageEntities context, int accountId)
        {
            return context.amistad
                .Where(friendship => friendship.estado == FriendshipStatus.Pending)
                .Where(friendship => friendship.id_usuario_solicitante == accountId)
                .Select(friendship => friendship.id_usuario_destinatario)
                .ToList();
        }

        private List<int> QueryReceivedRequestSenderIds(CribbageEntities context, int accountId)
        {
            return context.amistad
                .Where(friendship => friendship.estado == FriendshipStatus.Pending)
                .Where(friendship => friendship.id_usuario_destinatario == accountId)
                .Select(friendship => friendship.id_usuario_solicitante)
                .ToList();
        }

        private List<perfil_jugador> QueryProfiles(CribbageEntities context, List<int> accountIds)
        {
            return context.perfil_jugador
                .Where(profile => accountIds.Contains(profile.id_usuario))
                .ToList();
        }

        private Dictionary<int, DateTime?> QueryFriendships(CribbageEntities context, int accountId)
        {
            List<vista_amigos> rows = context.vista_amigos
                .Where(friendship => friendship.id_usuario == accountId || friendship.id_usuario_amigo == accountId)
                .ToList();

            Dictionary<int, DateTime?> friendships = new Dictionary<int, DateTime?>();

            foreach (vista_amigos row in rows)
            {
                friendships[ResolveFriendId(row, accountId)] = row.fecha_respuesta;
            }

            friendships.Remove(accountId);

            return friendships;
        }

        private int ResolveFriendId(vista_amigos friendship, int accountId)
        {
            int friendId = friendship.id_usuario;

            if (friendship.id_usuario == accountId)
            {
                friendId = friendship.id_usuario_amigo;
            }

            return friendId;
        }

        private void FillRecord(PlayerRecord record, usuario account, List<perfil_jugador> profiles)
        {
            perfil_jugador profile = profiles.FirstOrDefault(candidate => candidate.id_usuario == account.id_usuario);

            record.AccountId = account.id_usuario;
            record.UserName = account.nombre_usuario;
            record.DisplayName = account.nombre_usuario;
            record.HasProfileImage = false;

            if (profile != null)
            {
                record.DisplayName = profile.nombre_mostrado;
                record.HasProfileImage = HasCustomAvatar(profile);
            }
        }

        private bool HasCustomAvatar(perfil_jugador profile)
        {
            return !string.IsNullOrEmpty(profile.avatar) && profile.avatar != DefaultAvatar;
        }
    }
}