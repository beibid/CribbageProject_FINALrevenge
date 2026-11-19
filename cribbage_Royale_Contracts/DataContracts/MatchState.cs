using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Match state as seen by one participant.
    /// </summary>
    [DataContract]
    public class MatchState
    {
        /// <summary>
        /// Identifier of the match this state belongs to.
        /// </summary>
        [DataMember]
        public long MatchId { get; set; }

        /// <summary>
        /// Version of the state, incremented on every change.
        /// </summary>
        [DataMember]
        public int StateVersion { get; set; }

        /// <summary>
        /// Current phase of the match.
        /// </summary>
        [DataMember]
        public MatchPhase Phase { get; set; }

        /// <summary>
        /// Account of the opposing player.
        /// </summary>
        [DataMember]
        public AccountReference Rival { get; set; }

        /// <summary>
        /// Number of cards the opposing player holds.
        /// </summary>
        [DataMember]
        public int RivalCardCount { get; set; }

        /// <summary>
        /// Cards held by the recipient of this state.
        /// </summary>
        [DataMember]
        public List<Card> OwnHand { get; set; }

        /// <summary>
        /// Cards already played during the pegging phase.
        /// </summary>
        [DataMember]
        public List<Card> PlayedCards { get; set; }

        /// <summary>
        /// Cards of the crib, revealed only during the counting phase.
        /// </summary>
        [DataMember]
        public List<Card> CribCards { get; set; }

        /// <summary>
        /// Number of cards already placed in the crib.
        /// </summary>
        [DataMember]
        public int CribCardCount { get; set; }

        /// <summary>
        /// Card cut after both players discarded.
        /// </summary>
        [DataMember]
        public Card CutCard { get; set; }

        /// <summary>
        /// Score of the recipient of this state.
        /// </summary>
        [DataMember]
        public int OwnScore { get; set; }

        /// <summary>
        /// Score of the opposing player.
        /// </summary>
        [DataMember]
        public int RivalScore { get; set; }

        /// <summary>
        /// Account that owns the crib in the current hand.
        /// </summary>
        [DataMember]
        public int DealerAccountId { get; set; }

        /// <summary>
        /// Account whose turn it is to play.
        /// </summary>
        [DataMember]
        public int TurnAccountId { get; set; }

        /// <summary>
        /// Running count of the pegging phase.
        /// </summary>
        [DataMember]
        public int PeggingCount { get; set; }
    }
}