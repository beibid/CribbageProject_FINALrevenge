using System;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Solicitud de amistad pendiente.
    /// </summary>
    [DataContract]
    public class FriendRequest
    {
        /// <summary>
        /// Identificador estable de la solicitud.
        /// </summary>
        [DataMember(IsRequired = true)]
        public long RequestId { get; set; }

        /// <summary>
        /// La otra cuenta involucrada.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Counterpart { get; set; }

        /// <summary>
        /// Recibida o enviada, desde el punto de vista de quien consulta.
        /// </summary>
        [DataMember(IsRequired = true)]
        public RequestDirection Direction { get; set; }

        /// <summary>
        /// Instante en UTC en que el servidor registró la solicitud.
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime CreatedAtUtc { get; set; }
    }
}