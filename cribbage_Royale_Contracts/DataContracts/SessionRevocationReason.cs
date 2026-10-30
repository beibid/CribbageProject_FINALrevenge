using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Motivo por el cual el servidor dio por terminada una sesión.
    /// </summary>
    [DataContract]
    public enum SessionRevocationReason
    {
        /// <summary>
        /// La misma cuenta inició sesión desde otro cliente.
        /// </summary>
        [EnumMember]
        SignedInElsewhere,

        /// <summary>
        /// La cuenta fue suspendida mientras la sesión estaba abierta.
        /// </summary>
        [EnumMember]
        AccountSuspended,

        /// <summary>
        /// La sesión alcanzó su tiempo máximo de vigencia.
        /// </summary>
        [EnumMember]
        SessionExpired
    }
}