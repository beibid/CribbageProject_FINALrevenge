using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Tipo de hecho social que el servidor notifica fuera de la partida.
    /// </summary>
    [DataContract]
    public enum SocialEventType
    {
        /// <summary>
        /// El destinatario recibió una solicitud de amistad.
        /// </summary>
        [EnumMember]
        FriendRequestReceived,

        /// <summary>
        /// Una solicitud de amistad enviada por el destinatario fue aceptada.
        /// </summary>
        [EnumMember]
        FriendRequestAccepted,

        /// <summary>
        /// Una solicitud de amistad enviada por el destinatario fue rechazada.
        /// </summary>
        [EnumMember]
        FriendRequestRejected,

        /// <summary>
        /// El destinatario recibió una invitación a partida.
        /// </summary>
        [EnumMember]
        InvitationReceived,

        /// <summary>
        /// Una invitación enviada por el destinatario fue aceptada.
        /// </summary>
        [EnumMember]
        InvitationAccepted,

        /// <summary>
        /// Una invitación enviada por el destinatario fue rechazada.
        /// </summary>
        [EnumMember]
        InvitationRejected,

        /// <summary>
        /// Una invitación enviada por el destinatario venció sin respuesta.
        /// </summary>
        [EnumMember]
        InvitationExpired
    }
}