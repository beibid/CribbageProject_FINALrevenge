using System;
using System.Linq;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class FriendshipRepository
    {
        public void SendFriendRequest(int senderAccountId, int recipientAccountId)
        {
            using (CribbageEntities context = new CribbageEntities())
            {
                amistad friendshipEntity = new amistad();
                friendshipEntity.id_usuario_solicitante = senderAccountId;
                friendshipEntity.id_usuario_destinatario = recipientAccountId;
                friendshipEntity.estado = FriendshipStatus.Pending;
                friendshipEntity.fecha_solicitud = DateTime.Now;
                context.amistad.Add(friendshipEntity);
                context.SaveChanges();
            }
        }

        public void RespondToFriendRequest(int recipientAccountId, int senderAccountId, bool isAccepted)
        {
            using (CribbageEntities context = new CribbageEntities())
            {
                amistad friendshipEntity = context.amistad.First(friendRow =>
                    friendRow.id_usuario_solicitante == senderAccountId
                    && friendRow.id_usuario_destinatario == recipientAccountId
                    && friendRow.estado == FriendshipStatus.Pending);
                if (isAccepted)
                {
                    friendshipEntity.estado = FriendshipStatus.Accepted;
                    friendshipEntity.fecha_respuesta = DateTime.Now;
                }
                else
                {
                    context.amistad.Remove(friendshipEntity);
                }
                context.SaveChanges();
            }
        }

        public void CancelFriendRequest(int senderAccountId, int recipientAccountId)
        {
            using (CribbageEntities context = new CribbageEntities())
            {
                amistad friendshipEntity = context.amistad.First(friendRow =>
                    friendRow.id_usuario_solicitante == senderAccountId
                    && friendRow.id_usuario_destinatario == recipientAccountId
                    && friendRow.estado == FriendshipStatus.Pending);
                context.amistad.Remove(friendshipEntity);
                context.SaveChanges();
            }
        }

        public void RemoveFriend(int firstAccountId, int secondAccountId)
        {
            using (CribbageEntities context = new CribbageEntities())
            {
                amistad friendshipEntity = context.amistad.First(friendRow =>
                    (friendRow.id_usuario_solicitante == firstAccountId && friendRow.id_usuario_destinatario == secondAccountId
                        || friendRow.id_usuario_solicitante == secondAccountId && friendRow.id_usuario_destinatario == firstAccountId)
                    && friendRow.estado == FriendshipStatus.Accepted);
                context.amistad.Remove(friendshipEntity);
                context.SaveChanges();
            }
        }
    }
}