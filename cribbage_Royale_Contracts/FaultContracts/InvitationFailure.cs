using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Motivos por los que el servidor rechaza una operación de invitación a partida.
    /// </summary>
    [DataContract]
    public enum InvitationFailure
    {
        /// <summary>
        /// El identificador de cuenta no corresponde a una cuenta registrada.
        /// </summary>
        [EnumMember]
        InvalidAccountId,

        /// <summary>
        /// Quien llama intentó invitarse a sí mismo.
        /// </summary>
        [EnumMember]
        CannotInviteSelf,

        /// <summary>
        /// No existe amistad aceptada entre las dos cuentas.
        /// </summary>
        [EnumMember]
        FriendshipNotFound,

        /// <summary>
        /// Existe un bloqueo en alguno de los dos sentidos.
        /// </summary>
        [EnumMember]
        BlockedRelationship,

        /// <summary>
        /// La cuenta invitada no está conectada.
        /// </summary>
        [EnumMember]
        RecipientUnavailable,

        /// <summary>
        /// La cuenta invitada ya se encuentra en una partida.
        /// </summary>
        [EnumMember]
        RecipientBusy,

        /// <summary>
        /// Quien llama ya se encuentra en una partida.
        /// </summary>
        [EnumMember]
        CallerBusy,

        /// <summary>
        /// Ya existe una invitación pendiente entre las dos cuentas.
        /// </summary>
        [EnumMember]
        InvitationAlreadySent,

        /// <summary>
        /// La invitación no existe o ya fue atendida.
        /// </summary>
        [EnumMember]
        InvitationNotFound,

        /// <summary>
        /// Quien llama no es el destinatario de la invitación.
        /// </summary>
        [EnumMember]
        InvitationNotOwned
    }
}