using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Request to play one card during the pegging phase.
    /// </summary>
    [DataContract]
    public class CardPlay
    {
        /// <summary>
        /// Identifier of the match the card is played in.
        /// </summary>
        [DataMember]
        public long MatchId { get; set; }

        /// <summary>
        /// Card the player wants to put on the table.
        /// </summary>
        [DataMember]
        public Card Card { get; set; }

        /// <summary>
        /// State version the client held when the card was chosen.
        /// </summary>
        [DataMember]
        public int ExpectedStateVersion { get; set; }
    }
}