using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Request from a player to send two cards from their hand to the crib.
    /// </summary>
    [DataContract]
    public class CribDiscard
    {
        /// <summary>
        /// Identifier of the match.
        /// </summary>
        [DataMember(IsRequired = true)]
        public long MatchId { get; set; }

        /// <summary>
        /// The two cards the player wants to discard.
        /// </summary>
        [DataMember(IsRequired = true)]
        public List<Card> Cards { get; set; }

        /// <summary>
        /// State version the client held when the discard was chosen.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int ExpectedStateVersion { get; set; }
    }
}