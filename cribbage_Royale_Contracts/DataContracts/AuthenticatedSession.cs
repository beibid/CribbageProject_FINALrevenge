using System;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Datos privados de la sesión de un jugador registrado, entregados solo a su titular.
    /// </summary>
    [DataContract]
    public class AuthenticatedSession
    {
        /// <summary>
        /// Testigo opaco emitido por el servidor; el cliente lo envía en el encabezado SessionToken
        /// de cada operación posterior.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string SessionToken { get; set; }

        /// <summary>
        /// Identidad pública de la cuenta a la que pertenece la sesión.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Account { get; set; }

        /// <summary>
        /// Correo electrónico de la cuenta, devuelto únicamente a su titular.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Código del idioma preferido del jugador, por ejemplo es-MX.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string PreferredLocaleCode { get; set; }

        /// <summary>
        /// Instante en UTC en que caduca la sesión.
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime ExpiresAtUtc { get; set; }

        /// <summary>
        /// Indica si la cuenta pertenece al equipo de soporte; el cliente decide con esto si ofrece
        /// el panel de soporte.
        /// </summary>
        [DataMember(IsRequired = true)]
        public bool IsSupportAccount { get; set; }

        /// <summary>
        /// Partida sin concluir que se ofrece reanudar; nulo si no hay.
        /// </summary>
        [DataMember]
        public long? UnfinishedMatchId { get; set; }
    }
}