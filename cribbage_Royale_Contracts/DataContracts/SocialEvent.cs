using System;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Hecho ocurrido fuera de la partida que concierne al destinatario.
    /// </summary>
    [DataContract]
    public class SocialEvent
    {
        /// <summary>
        /// El otro jugador involucrado.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Counterpart { get; set; }

        /// <summary>
        /// Solicitud de amistad involucrada, cuando corresponde; nulo en otro caso.
        /// </summary>
        [DataMember]
        public long? FriendRequestId { get; set; }

        /// <summary>
        /// Invitación a partida involucrada, cuando corresponde; nulo en otro caso.
        /// </summary>
        [DataMember]
        public long? InvitationId { get; set; }

        /// <summary>
        /// Instante en UTC en que ocurrió el hecho.
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime OccurredAtUtc { get; set; }
    }
}