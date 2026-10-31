using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Cambio de presencia de un rival o de un amigo.
    /// </summary>
    [DataContract]
    public class PresenceChange
    {
        /// <summary>
        /// Cuenta cuya presencia cambió.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Account { get; set; }

        /// <summary>
        /// Indica si el cambio corresponde al rival de la partida en curso o a un amigo.
        /// </summary>
        [DataMember(IsRequired = true)]
        public PresenceScope Scope { get; set; }

        /// <summary>
        /// Indica si la cuenta quedó conectada.
        /// </summary>
        [DataMember(IsRequired = true)]
        public bool IsConnected { get; set; }

        /// <summary>
        /// Partida involucrada cuando el ámbito es el rival; nulo en cualquier otro caso.
        /// </summary>
        [DataMember]
        public long? MatchId { get; set; }
    }
}