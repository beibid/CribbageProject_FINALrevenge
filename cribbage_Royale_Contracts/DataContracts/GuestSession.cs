using System;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Sesión de invitado con el nombre confirmado por el servidor.
    /// </summary>
    [DataContract]
    public class GuestSession
    {
        /// <summary>
        /// Testigo opaco de la sesión de invitado; el cliente lo envía en el encabezado SessionToken
        /// de cada operación posterior.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string SessionToken { get; set; }

        /// <summary>
        /// Nombre definitivo del invitado, con el prefijo reservado que añade el servidor.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Instante en UTC en que caduca la sesión.
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime ExpiresAtUtc { get; set; }
    }
}