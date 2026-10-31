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
        private const string FriendsQueryFailureMessage = "The friends list could not be queried.";
        private const string RequestCreationFailureMessage = "The friend request could not be saved.";
        private const string RequestResponseFailureMessage = "The response to the friend request could not be saved.";
        private const string RequestCancellationFailureMessage = "The friend request could not be cancelled.";
        private const string FriendRemovalFailureMessage = "The friendship could not be removed.";

        public void SendFriendRequest(int senderAccountId, int recipientAccountId)
        {
            ExecuteWrite(
                context => AddFriendRequest(context, senderAccountId, recipientAccountId),
                RequestCreationFailureMessage);
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
            ExecuteWrite(
                context => RemoveFriendships(
                    context,
                    QueryAcceptedFriendships(context, firstAccountId, secondAccountId)),
                FriendRemovalFailureMessage);
        }

        public List<FriendRecord> GetFriends(int accountId)
        {
            return ExecuteQuery(context => QueryFriends(context, accountId), FriendsQueryFailureMessage);
        }

        public List<int> GetFriendIds(int accountId)
        {
            return ExecuteQuery(
                context => QueryFriendships(context, accountId).Keys.ToList(),
                FriendsQueryFailureMessage);
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

        private TResult ExecuteQuery<TResult>(Func<CribbageEntities, TResult> query, string failureMessage)
        {
            TResult result;

            try
            {
                using (CribbageEntities context = new CribbageEntities())
                {
                    result = query(context);
                }
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

        private void AddFriendRequest(CribbageEntities context, int senderAccountId, int recipientAccountId)
        {
            amistad friendship = new amistad();
            friendship.id_usuario_solicitante = senderAccountId;
            friendship.id_usuario_destinatario = recipientAccountId;
            friendship.estado = FriendshipStatus.Pending;
            friendship.fecha_solicitud = DateTime.Now;

            context.amistad.Add(friendship);
            context.SaveChanges();
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

        private List<FriendRecord> QueryFriends(CribbageEntities context, int accountId)
        {
            Dictionary<int, DateTime?> friendships = QueryFriendships(context, accountId);
            List<int> friendIds = friendships.Keys.ToList();

            List<usuario> accounts = context.usuario
                .Where(account => friendIds.Contains(account.id_usuario))
                .ToList();
            List<perfil_jugador> profiles = context.perfil_jugador
                .Where(profile => friendIds.Contains(profile.id_usuario))
                .ToList();

            List<FriendRecord> friends = new List<FriendRecord>();

            foreach (usuario account in accounts)
            {
                perfil_jugador profile = profiles.FirstOrDefault(record => record.id_usuario == account.id_usuario);
                FriendRecord friend = BuildRecord(account, profile);
                friend.FriendsSince = friendships[account.id_usuario];
                friends.Add(friend);
            }

            return friends;
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

        private FriendRecord BuildRecord(usuario account, perfil_jugador profile)
        {
            FriendRecord friend = new FriendRecord();
            friend.AccountId = account.id_usuario;
            friend.UserName = account.nombre_usuario;
            friend.DisplayName = account.nombre_usuario;
            friend.HasProfileImage = false;

            if (profile != null)
            {
                friend.DisplayName = profile.nombre_mostrado;
                friend.HasProfileImage = HasCustomAvatar(profile);
            }

            return friend;
        }

        private bool HasCustomAvatar(perfil_jugador profile)
        {
            return !string.IsNullOrEmpty(profile.avatar) && profile.avatar != DefaultAvatar;
        }
    }
}