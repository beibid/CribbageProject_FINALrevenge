using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Reasons the server rejects sending a chat message.
    /// </summary>
    [DataContract]
    public enum ChatFailure
    {
        /// <summary>
        /// The account identifier is not positive.
        /// </summary>
        [EnumMember]
        InvalidAccountId,

        /// <summary>
        /// The message is addressed to the caller's own account.
        /// </summary>
        [EnumMember]
        CannotTargetSelf,

        /// <summary>
        /// There is no active friendship between the accounts.
        /// </summary>
        [EnumMember]
        FriendshipNotFound,

        /// <summary>
        /// A block exists between the accounts.
        /// </summary>
        [EnumMember]
        BlockedRelationship,

        /// <summary>
        /// The recipient is not connected and the message is not stored.
        /// </summary>
        [EnumMember]
        RecipientUnavailable,

        /// <summary>
        /// The text is empty or exceeds the maximum allowed length.
        /// </summary>
        [EnumMember]
        InvalidMessageText
    }
}