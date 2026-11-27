using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Card from a standard 52-card deck. It is the only card type in the system and is shared by the
    /// contracts and the rules project.
    /// </summary>
    [DataContract]
    public class Card
    {
        /// <summary>
        /// Rank of the card.
        /// </summary>
        [DataMember]
        public CardRank Rank { get; set; }

        /// <summary>
        /// Suit of the card.
        /// </summary>
        [DataMember]
        public CardSuit Suit { get; set; }
    }
}