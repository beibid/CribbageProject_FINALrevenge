using System;
using System.Collections.Generic;
using System.Linq;
using cribbage_Royale_PROYECTO.Models;

namespace cribbage_Royale_PROYECTO.DataAccess
{
    public class MatchRepository
    {
        private const string AbandonmentEventType = "ABANDONO_PARTIDA";

        public MatchInformationSummary GetMatchInformation(long matchId, int viewerAccountId)
        {
            MatchInformationSummary summary = null;

            using (CribbageEntities context = new CribbageEntities())
            {
                List<participante_partida> participants = context.participante_partida
                    .Where(currentParticipant => currentParticipant.id_partida == matchId)
                    .ToList();
                participante_partida viewer = participants
                    .FirstOrDefault(currentParticipant => currentParticipant.id_usuario == viewerAccountId);
                participante_partida opponent = participants
                    .FirstOrDefault(currentParticipant => currentParticipant.id_usuario != viewerAccountId);

                if (viewer != null && opponent != null)
                {
                    summary = BuildMatchInformation(context, viewer, opponent);
                }
            }

            return summary;
        }

        public UnfinishedMatchSummary FindUnfinishedMatch(int accountId)
        {
            UnfinishedMatchSummary unfinishedMatch = null;

            using (CribbageEntities context = new CribbageEntities())
            {
                long matchId = (from participant in context.participante_partida
                                join currentMatch in context.partida
                                    on participant.id_partida equals currentMatch.id_partida
                                where participant.id_usuario == accountId
                                    && currentMatch.estado_partida == MatchStatus.InProgress
                                orderby currentMatch.fecha_inicio descending
                                select currentMatch.id_partida).FirstOrDefault();

                if (matchId != 0)
                {
                    unfinishedMatch = BuildUnfinishedMatch(context, matchId, accountId);
                }
            }

            return unfinishedMatch;
        }
        public static bool HasUnfinishedMatchInContext(CribbageEntities context, int accountId)
        {
            return (from participant in context.participante_partida
                    join currentMatch in context.partida
                        on participant.id_partida equals currentMatch.id_partida
                    where participant.id_usuario == accountId
                        && currentMatch.estado_partida == MatchStatus.InProgress
                    select participant.id_participante).Any();
        }
        public bool MarkParticipantConnected(long matchId, int accountId)
        {
            bool updated = false;

            using (CribbageEntities context = new CribbageEntities())
            {
                participante_partida participant = context.participante_partida
                    .FirstOrDefault(currentParticipant => currentParticipant.id_partida == matchId
                        && currentParticipant.id_usuario == accountId);

                if (participant != null)
                {
                    participant.esta_conectado = true;
                    context.SaveChanges();
                    updated = true;
                }
            }

            return updated;
        }

        public string RegisterAbandonment(long matchId, int accountId)
        {
            string outcome = AbandonmentOutcome.AlreadyConcluded;

            using (CribbageEntities context = new CribbageEntities())
            {
                partida currentMatch = context.partida
                    .FirstOrDefault(matchRow => matchRow.id_partida == matchId);
                participante_partida opponent = context.participante_partida
                    .FirstOrDefault(currentParticipant => currentParticipant.id_partida == matchId
                        && currentParticipant.id_usuario != accountId);
                bool isParticipant = context.participante_partida
                    .Any(currentParticipant => currentParticipant.id_partida == matchId
                        && currentParticipant.id_usuario == accountId);

                if (currentMatch != null && opponent != null && isParticipant
                    && currentMatch.estado_partida == MatchStatus.InProgress)
                {
                    DateTime now = DateTime.Now;
                    currentMatch.estado_partida = MatchStatus.Concluded;
                    currentMatch.motivo_termino = MatchTerminationReason.Abandonment;
                    currentMatch.fecha_fin = now;
                    currentMatch.id_usuario_ganador = opponent.id_usuario;

                    bitacora_evento abandonmentEvent = new bitacora_evento();
                    abandonmentEvent.id_usuario = accountId;
                    abandonmentEvent.id_partida = matchId;
                    abandonmentEvent.tipo_evento = AbandonmentEventType;
                    abandonmentEvent.descripcion = MatchTerminationReason.Abandonment;
                    abandonmentEvent.fecha_evento = now;
                    context.bitacora_evento.Add(abandonmentEvent);

                    context.SaveChanges();
                    outcome = AbandonmentOutcome.Registered;
                }
            }

            return outcome;
        }

        private MatchInformationSummary BuildMatchInformation(CribbageEntities context,
            participante_partida viewer, participante_partida opponent)
        {
            MatchInformationSummary summary = new MatchInformationSummary();
            long matchId = viewer.id_partida;
            int opponentAccountId = opponent.id_usuario;

            perfil_jugador opponentProfile = context.perfil_jugador
                .FirstOrDefault(profile => profile.id_usuario == opponentAccountId);

            summary.MatchId = matchId;
            summary.OpponentAccountId = opponentAccountId;
            summary.OpponentConnected = opponent.esta_conectado;

            if (opponentProfile != null)
            {
                summary.OpponentDisplayName = opponentProfile.nombre_mostrado;
                summary.OpponentSkillLevel = opponentProfile.nivel_habilidad;
            }

            List<long> handIds = context.manga
                .Where(hand => hand.id_partida == matchId)
                .Select(hand => hand.id_manga)
                .ToList();

            summary.ViewerScore = GetScore(context, handIds, viewer.id_participante);
            summary.OpponentScore = GetScore(context, handIds, opponent.id_participante);

            manga currentHand = context.manga
                .Where(hand => hand.id_partida == matchId)
                .OrderByDescending(hand => hand.numero_manga)
                .FirstOrDefault();

            if (currentHand != null)
            {
                FillCurrentHandInformation(context, summary, currentHand);
                summary.OpponentRemainingCards = CountCardsInHand(context, currentHand.id_manga,
                    opponent.id_participante);
            }

            return summary;
        }

        private void FillCurrentHandInformation(CribbageEntities context, MatchInformationSummary summary,
            manga currentHand)
        {
            long dealerParticipantId = currentHand.id_participante_repartidor;

            summary.HandNumber = currentHand.numero_manga;
            summary.RunningCount = currentHand.conteo_actual;
            summary.TurnDeadline = currentHand.fecha_fin_turno;

            participante_partida dealer = context.participante_partida
                .FirstOrDefault(currentParticipant => currentParticipant.id_participante == dealerParticipantId);

            if (dealer != null)
            {
                summary.DealerDisplayName = GetDisplayName(context, dealer.id_usuario);
            }

            if (currentHand.id_carta_volteada.HasValue)
            {
                byte cutCardId = currentHand.id_carta_volteada.Value;
                carta cutCard = context.carta.FirstOrDefault(currentCard => currentCard.id_carta == cutCardId);

                if (cutCard != null)
                {
                    summary.CutCardText = CardTextFormatter.Format(cutCard.palo, cutCard.valor);
                }
            }
        }

        private int CountCardsInHand(CribbageEntities context, long handId, long participantId)
        {
            return context.carta_en_manga
                .Count(cardInHand => cardInHand.id_manga == handId
                    && cardInHand.id_participante_origen == participantId
                    && cardInHand.estado == CardInMatchState.InHand);
        }

        private string GetDisplayName(CribbageEntities context, int accountId)
        {
            string displayName = string.Empty;
            perfil_jugador profile = context.perfil_jugador
                .FirstOrDefault(currentProfile => currentProfile.id_usuario == accountId);

            if (profile != null)
            {
                displayName = profile.nombre_mostrado;
            }

            return displayName;
        }

        private UnfinishedMatchSummary BuildUnfinishedMatch(CribbageEntities context, long matchId, int accountId)
        {
            UnfinishedMatchSummary summary = new UnfinishedMatchSummary();
            summary.MatchId = matchId;

            participante_partida viewer = context.participante_partida
                .FirstOrDefault(currentParticipant => currentParticipant.id_partida == matchId
                    && currentParticipant.id_usuario == accountId);
            participante_partida opponent = context.participante_partida
                .FirstOrDefault(currentParticipant => currentParticipant.id_partida == matchId
                    && currentParticipant.id_usuario != accountId);

            if (viewer != null && opponent != null)
            {
                List<long> handIds = context.manga
                    .Where(hand => hand.id_partida == matchId)
                    .Select(hand => hand.id_manga)
                    .ToList();

                summary.ViewerScore = GetScore(context, handIds, viewer.id_participante);
                summary.OpponentScore = GetScore(context, handIds, opponent.id_participante);
                summary.OpponentDisplayName = GetDisplayName(context, opponent.id_usuario);
            }

            return summary;
        }

        private int GetScore(CribbageEntities context, List<long> handIds, long participantId)
        {
            int? total = context.puntuacion
                .Where(score => handIds.Contains(score.id_manga) && score.id_participante == participantId)
                .Sum(score => (int?)score.puntos);

            return total.GetValueOrDefault();
        }
    }
}