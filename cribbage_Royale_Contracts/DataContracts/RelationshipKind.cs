using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Relación entre quien consulta y otra cuenta.
    /// </summary>
    [DataContract]
    public enum RelationshipKind
    {
        /// <summary>
        /// No existe ninguna relación entre las cuentas.
        /// </summary>
        [EnumMember]
        None,

        /// <summary>
        /// Las cuentas son amigas.
        /// </summary>
        [EnumMember]
        Friend,

        /// <summary>
        /// Quien consulta envió una solicitud que sigue pendiente.
        /// </summary>
        [EnumMember]
        RequestSent,

        /// <summary>
        /// Quien consulta recibió una solicitud que sigue pendiente.
        /// </summary>
        [EnumMember]
        RequestReceived,

        /// <summary>
        /// Existe un bloqueo entre las cuentas.
        /// </summary>
        [EnumMember]
        Blocked
    }
}