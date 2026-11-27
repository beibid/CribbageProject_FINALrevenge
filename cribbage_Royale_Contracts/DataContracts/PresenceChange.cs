using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Presence change of an opponent or a friend.
    /// </summary>
    [DataContract]
    public class PresenceChange
    {
        /// <summary>
        /// Account whose presence changed.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Account { get; set; }

        /// <summary>
        /// Whether the change concerns the opponent in the current match or a friend.
        /// </summary>
        [DataMember(IsRequired = true)]
        public PresenceScope Scope { get; set; }

        /// <summary>
        /// Whether the account is now connected.
        /// </summary>
        [DataMember(IsRequired = true)]
        public bool IsConnected { get; set; }

        /// <summary>
        /// Match involved when the scope is the opponent; null otherwise.
        /// </summary>
        [DataMember]
        public long? MatchId { get; set; }
    }
}