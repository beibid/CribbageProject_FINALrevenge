using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Reasons the server rejects a match invitation operation.
    /// </summary>
    [DataContract]
    public enum InvitationFailure
    {
        /// <summary>
        /// The account identifier does not match a registered account.
        /// </summary>
        [EnumMember]
        InvalidAccountId,

        /// <summary>
        /// The caller attempted to invite their own account.
        /// </summary>
        [EnumMember]
        CannotInviteSelf,

        /// <summary>
        /// There is no accepted friendship between the two accounts.
        /// </summary>
        [EnumMember]
        FriendshipNotFound,

        /// <summary>
        /// A block exists in either direction.
        /// </summary>
        [EnumMember]
        BlockedRelationship,

        /// <summary>
        /// The invited account is not connected.
        /// </summary>
        [EnumMember]
        RecipientUnavailable,

        /// <summary>
        /// The invited account is already in a match.
        /// </summary>
        [EnumMember]
        RecipientBusy,

        /// <summary>
        /// The caller is already in a match.
        /// </summary>
        [EnumMember]
        CallerBusy,

        /// <summary>
        /// A pending invitation already exists between the two accounts.
        /// </summary>
        [EnumMember]
        InvitationAlreadySent,

        /// <summary>
        /// The invitation does not exist or has already been handled.
        /// </summary>
        [EnumMember]
        InvitationNotFound,

        /// <summary>
        /// The caller is not the recipient of the invitation.
        /// </summary>
        [EnumMember]
        InvitationNotOwned
    }
}