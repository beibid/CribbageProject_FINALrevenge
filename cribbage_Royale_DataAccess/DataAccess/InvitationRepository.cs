using System;
using System.Data.Entity;
using System.Linq;
using cribbage_Royale_DataAccess.Models;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class InvitationRepository
    {
        private const int InvitationLifetimeSeconds = 60;
        private const int FirstHandNumber = 1;

        public InvitationCreationResult CreateInvitation(int senderAccountId, int recipientAccountId)
        {
            InvitationCreationResult result = new InvitationCreationResult();

            using (CribbageEntities context = new CribbageEntities())
            {
                DateTime now = DateTime.Now;
                result.Outcome = EvaluateRestrictions(context, senderAccountId, recipientAccountId);

                if (result.Outcome == InvitationCreationOutcome.Created)
                {
                    invitacion_partida newInvitation = new invitacion_partida();
                    newInvitation.id_usuario_emisor = senderAccountId;
                    newInvitation.id_usuario_destinatario = recipientAccountId;
                    newInvitation.estado = InvitationStatus.Pending;
                    newInvitation.fecha_envio = now;
                    newInvitation.fecha_expiracion = now.AddSeconds(InvitationLifetimeSeconds);
                    context.invitacion_partida.Add(newInvitation);
                    context.SaveChanges();

                    result.InvitationId = newInvitation.id_invitacion;
                }
            }

            return result;
        }

        public MatchInvitationSummary GetInvitation(long invitationId)
        {
            MatchInvitationSummary summary = null;

            using (CribbageEntities context = new CribbageEntities())
            {
                invitacion_partida invitation = context.invitacion_partida
                    .FirstOrDefault(currentInvitation => currentInvitation.id_invitacion == invitationId);

                if (invitation != null)
                {
                    int senderAccountId = invitation.id_usuario_emisor;
                    perfil_jugador senderProfile = context.perfil_jugador
                        .FirstOrDefault(profile => profile.id_usuario == senderAccountId);

                    summary = new MatchInvitationSummary();
                    summary.InvitationId = invitation.id_invitacion;
                    summary.SenderAccountId = senderAccountId;
                    summary.RecipientAccountId = invitation.id_usuario_destinatario;
                    summary.ExpirationDate = invitation.fecha_expiracion;
                    summary.MatchId = invitation.id_partida;
                    summary.Status = invitation.estado;

                    if (invitation.estado == InvitationStatus.Pending
                        && invitation.fecha_expiracion <= DateTime.Now)
                    {
                        summary.Status = InvitationStatus.Expired;
                    }

                    if (senderProfile != null)
                    {
                        summary.SenderDisplayName = senderProfile.nombre_mostrado;
                        summary.SenderSkillLevel = senderProfile.nivel_habilidad;
                    }
                }
            }

            return summary;
        }

        public InvitationResponseResult AcceptInvitation(long invitationId, int recipientAccountId)
        {
            InvitationResponseResult result = new InvitationResponseResult();
            result.Outcome = InvitationAcceptOutcome.NoLongerAvailable;

            using (CribbageEntities context = new CribbageEntities())
            {
                invitacion_partida invitation = context.invitacion_partida
                    .FirstOrDefault(currentInvitation => currentInvitation.id_invitacion == invitationId);

                bool isAvailable = invitation != null
                    && invitation.estado == InvitationStatus.Pending
                    && invitation.fecha_expiracion > DateTime.Now
                    && invitation.id_usuario_destinatario == recipientAccountId
                    && !MatchRepository.HasUnfinishedMatchInContext(context, invitation.id_usuario_emisor);

                if (isAvailable && MatchRepository.HasUnfinishedMatchInContext(context, recipientAccountId))
                {
                    result.Outcome = InvitationAcceptOutcome.HasUnfinishedMatch;
                }
                else if (isAvailable)
                {
                    result.MatchId = CreateMatchFromInvitation(context, invitation);
                    result.Outcome = InvitationAcceptOutcome.Accepted;
                }
            }

            return result;
        }

        public bool RejectInvitation(long invitationId)
        {
            return UpdatePendingStatus(invitationId, InvitationStatus.Rejected);
        }

        public bool CancelInvitation(long invitationId)
        {
            return UpdatePendingStatus(invitationId, InvitationStatus.Cancelled);
        }

        public bool ExpireInvitation(long invitationId)
        {
            return UpdatePendingStatus(invitationId, InvitationStatus.Expired);
        }

        private string EvaluateRestrictions(CribbageEntities context, int senderAccountId, int recipientAccountId)
        {
            string outcome = InvitationCreationOutcome.Created;
            DateTime now = DateTime.Now;

            bool recipientIsOnline = context.sesion
                .Any(currentSession => currentSession.id_usuario == recipientAccountId
                    && currentSession.fecha_revocacion == null
                    && currentSession.fecha_expiracion > now);
            bool alreadyInvited = context.invitacion_partida
                .Any(currentInvitation => currentInvitation.id_usuario_emisor == senderAccountId
                    && currentInvitation.id_usuario_destinatario == recipientAccountId
                    && currentInvitation.estado == InvitationStatus.Pending
                    && currentInvitation.fecha_expiracion > now);

            if (MatchRepository.HasUnfinishedMatchInContext(context, senderAccountId))
            {
                outcome = InvitationCreationOutcome.SenderHasUnfinishedMatch;
            }
            else if (MatchRepository.HasUnfinishedMatchInContext(context, recipientAccountId))
            {
                outcome = InvitationCreationOutcome.RecipientHasUnfinishedMatch;
            }
            else if (!recipientIsOnline)
            {
                outcome = InvitationCreationOutcome.RecipientOffline;
            }
            else if (alreadyInvited)
            {
                outcome = InvitationCreationOutcome.AlreadyInvitedByYou;
            }

            return outcome;
        }

        private long CreateMatchFromInvitation(CribbageEntities context, invitacion_partida invitation)
        {
            long matchId = 0;

            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                DateTime now = DateTime.Now;

                partida newMatch = new partida();
                newMatch.estado_partida = MatchStatus.InProgress;
                newMatch.origen = MatchOrigin.Invitation;
                newMatch.fecha_inicio = now;
                context.partida.Add(newMatch);
                context.SaveChanges();

                participante_partida senderParticipant = new participante_partida();
                senderParticipant.id_partida = newMatch.id_partida;
                senderParticipant.id_usuario = invitation.id_usuario_emisor;
                participante_partida recipientParticipant = new participante_partida();
                recipientParticipant.id_partida = newMatch.id_partida;
                recipientParticipant.id_usuario = invitation.id_usuario_destinatario;
                context.participante_partida.Add(senderParticipant);
                context.participante_partida.Add(recipientParticipant);
                context.SaveChanges();

                Random randomGenerator = new Random();
                participante_partida dealer = senderParticipant;

                if (randomGenerator.Next(2) == 1)
                {
                    dealer = recipientParticipant;
                }

                manga firstHand = new manga();
                firstHand.id_partida = newMatch.id_partida;
                firstHand.numero_manga = FirstHandNumber;
                firstHand.id_participante_repartidor = dealer.id_participante;
                firstHand.estado_actual = HandState.Dealing;
                firstHand.fecha_inicio = now;
                context.manga.Add(firstHand);

                invitation.estado = InvitationStatus.Accepted;
                invitation.id_partida = newMatch.id_partida;
                context.SaveChanges();

                transaction.Commit();
                matchId = newMatch.id_partida;
            }

            return matchId;
        }

        private bool UpdatePendingStatus(long invitationId, string newStatus)
        {
            bool updated = false;

            using (CribbageEntities context = new CribbageEntities())
            {
                invitacion_partida invitation = context.invitacion_partida
                    .FirstOrDefault(currentInvitation => currentInvitation.id_invitacion == invitationId);

                if (invitation != null && invitation.estado == InvitationStatus.Pending)
                {
                    invitation.estado = newStatus;
                    context.SaveChanges();
                    updated = true;
                }
            }

            return updated;
        }
    }
}