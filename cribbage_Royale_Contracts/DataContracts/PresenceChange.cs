using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Informa que un jugador se conectó o se desconectó del servidor.
    /// </summary>
    [DataContract]
    public class PresenceChange
    {
        /// <summary>
        /// Identificador de la cuenta cuyo estado de conexión cambió.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int AccountId { get; set; }

        /// <summary>
        /// Indica si el jugador quedó conectado.
        /// </summary>
        [DataMember(IsRequired = true)]
        public bool IsOnline { get; set; }

        /// <summary>
        /// Ámbito por el cual el destinatario recibe este cambio.
        /// </summary>
        [DataMember(IsRequired = true)]
        public PresenceScope Scope { get; set; }
    }
}