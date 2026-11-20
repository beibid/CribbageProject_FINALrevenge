using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Type of change that occurred in a match.
    /// </summary>
    [DataContract]
    public enum MatchEventType
    {
        /// <summary>
        /// The player is searching for a match.
        /// </summary>
        [EnumMember]
        Searching = 0,

        /// <summary>
        /// The match started.
        /// </summary>
        [EnumMember]
        Started = 1,

        /// <summary>
        /// A new hand was dealt.
        /// </summary>
        [EnumMember]
        HandDealt = 2,

        /// <summary>
        /// The rival played a card.
        /// </summary>
        [EnumMember]
        RivalPlayed = 3,

        /// <summary>
        /// The cut card was revealed.
        /// </summary>
        [EnumMember]
        CutCardRevealed = 4,

        /// <summary>
        /// The hands and the crib were counted.
        /// </summary>
        [EnumMember]
        CountFinished = 5,

        /// <summary>
        /// The round finished.
        /// </summary>
        [EnumMember]
        RoundFinished = 6,

        /// <summary>
        /// The match resumed.
        /// </summary>
        [EnumMember]
        Resumed = 7,

        /// <summary>
        /// The match finished.
        /// </summary>
        [EnumMember]
        Finished = 8,

        /// <summary>
        /// A player discarded cards to the crib.
        /// </summary>
        [EnumMember]
        CardsDiscarded = 9,

        /// <summary>
        /// The rival disconnected and the match ended by abandonment.
        /// </summary>
        [EnumMember]
        RivalDisconnected = 10
    }
}