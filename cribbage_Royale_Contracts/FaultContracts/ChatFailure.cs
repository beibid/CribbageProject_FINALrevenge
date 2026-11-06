using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Motivos por los que el servidor rechaza el envío de un mensaje de chat.
    /// </summary>
    [DataContract]
    public enum ChatFailure
    {
        /// <summary>
        /// El identificador de cuenta no es positivo.
        /// </summary>
        [EnumMember]
        InvalidAccountId,

        /// <summary>
        /// El mensaje está dirigido a la propia cuenta.
        /// </summary>
        [EnumMember]
        CannotTargetSelf,

        /// <summary>
        /// No existe una amistad vigente entre las cuentas.
        /// </summary>
        [EnumMember]
        FriendshipNotFound,

        /// <summary>
        /// Existe un bloqueo entre las cuentas.
        /// </summary>
        [EnumMember]
        BlockedRelationship,

        /// <summary>
        /// El destinatario no está conectado y el mensaje no se conserva.
        /// </summary>
        [EnumMember]
        RecipientUnavailable,

        /// <summary>
        /// El texto está vacío o excede el máximo permitido.
        /// </summary>
        [EnumMember]
        InvalidMessageText
    }
}