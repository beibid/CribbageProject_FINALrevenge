using System;
using System.Linq;
using cribbage_Royale_DataAccess.Models;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class PlayerProfileRepository
    {
        public PublicProfileSummary GetPublicProfile(int viewerAccountId, int targetAccountId)
        {
            PublicProfileSummary summary = new PublicProfileSummary();
            using (CribbageEntities context = new CribbageEntities())
            {
                usuario targetUserEntity = context.usuario.First(userRow => userRow.id_usuario == targetAccountId);
                perfil_jugador targetProfileEntity = context.perfil_jugador.First(profileRow => profileRow.id_usuario == targetAccountId);
                summary.AccountId = targetAccountId;
                summary.DisplayName = targetProfileEntity.nombre_mostrado;
                summary.Username = targetUserEntity.nombre_usuario;
                summary.RegistrationDate = targetUserEntity.fecha_registro;
                summary.MatchesPlayed = targetProfileEntity.partidas_jugadas;
                summary.Wins = targetProfileEntity.victorias;
                summary.WinPercentage = CalculateWinPercentage(targetProfileEntity.partidas_jugadas, targetProfileEntity.victorias);
                summary.SkillLevel = targetProfileEntity.nivel_habilidad;
                summary.RelationshipStatus = DetermineRelationshipStatus(context, viewerAccountId, targetAccountId);
            }
            return summary;
        }

        private decimal CalculateWinPercentage(int matchesPlayed, int wins)
        {
            decimal winPercentage;
            if (matchesPlayed == 0)
            {
                winPercentage = 0m;
            }
            else
            {
                winPercentage = Math.Round(wins * 100m / matchesPlayed, 2);
            }
            return winPercentage;
        }

        private string DetermineRelationshipStatus(CribbageEntities context, int viewerAccountId, int targetAccountId)
        {
            string relationshipStatus;
            bloqueo blockEntity = context.bloqueo.FirstOrDefault(blockRow =>
                blockRow.id_usuario_que_bloquea == viewerAccountId && blockRow.id_usuario_bloqueado == targetAccountId);
            amistad friendshipEntity = context.amistad.FirstOrDefault(friendRow =>
                (friendRow.id_usuario_solicitante == viewerAccountId && friendRow.id_usuario_destinatario == targetAccountId)
                || (friendRow.id_usuario_solicitante == targetAccountId && friendRow.id_usuario_destinatario == viewerAccountId));
            if (blockEntity != null)
            {
                relationshipStatus = RelationshipStatus.Blocked;
            }
            else if (friendshipEntity == null)
            {
                relationshipStatus = RelationshipStatus.None;
            }
            else if (friendshipEntity.estado == FriendshipStatus.Accepted)
            {
                relationshipStatus = RelationshipStatus.Friend;
            }
            else if (friendshipEntity.estado == FriendshipStatus.Pending && friendshipEntity.id_usuario_solicitante == viewerAccountId)
            {
                relationshipStatus = RelationshipStatus.RequestSent;
            }
            else if (friendshipEntity.estado == FriendshipStatus.Pending && friendshipEntity.id_usuario_solicitante == targetAccountId)
            {
                relationshipStatus = RelationshipStatus.RequestReceived;
            }
            else
            {
                relationshipStatus = RelationshipStatus.None;
            }
            return relationshipStatus;
        }
    }
}