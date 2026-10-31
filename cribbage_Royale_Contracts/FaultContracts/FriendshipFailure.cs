using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Motivos por los que el servidor rechaza una operación de amistades, búsquedas o bloqueos.
    /// </summary>
    [DataContract]
    public enum FriendshipFailure
    {
        /// <summary>
        /// El identificador de cuenta no es positivo.
        /// </summary>
        [EnumMember]
        InvalidAccountId,

        /// <summary>
        /// La otra cuenta no existe o no está activa.
        /// </summary>
        [EnumMember]
        AccountUnavailable,

        /// <summary>
        /// La operación está dirigida a la propia cuenta.
        /// </summary>
        [EnumMember]
        CannotTargetSelf,

        /// <summary>
        /// Ya existe una amistad aceptada.
        /// </summary>
        [EnumMember]
        FriendshipAlreadyExists,

        /// <summary>
        /// No existe una amistad vigente entre las cuentas.
        /// </summary>
        [EnumMember]
        FriendshipNotFound,

        /// <summary>
        /// Hay una solicitud pendiente en alguno de los dos sentidos.
        /// </summary>
        [EnumMember]
        RequestAlreadyPending,

        /// <summary>
        /// La solicitud no existe o ya no está pendiente.
        /// </summary>
        [EnumMember]
        RequestUnavailable,

        /// <summary>
        /// La cuenta no es la que puede responder o cancelar esa solicitud.
        /// </summary>
        [EnumMember]
        UnauthorizedRequest,

        /// <summary>
        /// Existe un bloqueo entre las cuentas.
        /// </summary>
        [EnumMember]
        BlockedRelationship,

        /// <summary>
        /// El texto de búsqueda está vacío o no cumple el formato.
        /// </summary>
        [EnumMember]
        InvalidSearchText
    }
}