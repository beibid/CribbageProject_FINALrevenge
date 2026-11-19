using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Reasons why a match operation can be rejected.
    /// </summary>
    [DataContract]
    public enum MatchFailure
    {
        /// <summary>
        /// The match does not exist.
        /// </summary>
        [EnumMember]
        MatchNotFound = 0,

        /// <summary>
        /// The caller does not belong to the match.
        /// </summary>
        [EnumMember]
        NotAParticipant = 1,

        /// <summary>
        /// The match is not in a phase that allows the operation.
        /// </summary>
        [EnumMember]
        MatchNotInProgress = 2,

        /// <summary>
        /// The discarded cards are not valid.
        /// </summary>
        [EnumMember]
        InvalidDiscard = 3,

        /// <summary>
        /// It is not the caller's turn to play.
        /// </summary>
        [EnumMember]
        NotYourTurn = 4,

        /// <summary>
        /// The played card is not valid for the current count.
        /// </summary>
        [EnumMember]
        InvalidPlay = 5
    }
}