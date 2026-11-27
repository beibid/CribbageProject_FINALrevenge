using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Notice that a match has started, with the data each player needs to join it.
    /// </summary>
    [DataContract]
    public class MatchStart
    {
        /// <summary>
        /// Identifier assigned to the match by the server.
        /// </summary>
        [DataMember(IsRequired = true)]
        public long MatchId { get; set; }

        /// <summary>
        /// The other player at the table, from the point of view of the recipient.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Opponent { get; set; }

        /// <summary>
        /// Whether the recipient is the host of the match.
        /// </summary>
        [DataMember(IsRequired = true)]
        public bool IsHost { get; set; }
    }
}