using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Reasons the server rejects a friendship, search or block operation.
    /// </summary>
    [DataContract]
    public enum FriendshipFailure
    {
        /// <summary>
        /// The account identifier is not positive.
        /// </summary>
        [EnumMember]
        InvalidAccountId,

        /// <summary>
        /// The other account does not exist or is not active.
        /// </summary>
        [EnumMember]
        AccountUnavailable,

        /// <summary>
        /// The operation targets the caller's own account.
        /// </summary>
        [EnumMember]
        CannotTargetSelf,

        /// <summary>
        /// An accepted friendship already exists.
        /// </summary>
        [EnumMember]
        FriendshipAlreadyExists,

        /// <summary>
        /// There is no active friendship between the accounts.
        /// </summary>
        [EnumMember]
        FriendshipNotFound,

        /// <summary>
        /// A request is already pending in either direction.
        /// </summary>
        [EnumMember]
        RequestAlreadyPending,

        /// <summary>
        /// The request does not exist or is no longer pending.
        /// </summary>
        [EnumMember]
        RequestUnavailable,

        /// <summary>
        /// The caller is not allowed to respond to or cancel the request.
        /// </summary>
        [EnumMember]
        UnauthorizedRequest,

        /// <summary>
        /// A block exists between the accounts.
        /// </summary>
        [EnumMember]
        BlockedRelationship,

        /// <summary>
        /// The search text is empty or has an invalid format.
        /// </summary>
        [EnumMember]
        InvalidSearchText
    }
}