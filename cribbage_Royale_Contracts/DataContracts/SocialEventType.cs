using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Kind of social event the server notifies outside of a match.
    /// </summary>
    [DataContract]
    public enum SocialEventType
    {
        /// <summary>
        /// The recipient received a friend request.
        /// </summary>
        [EnumMember]
        FriendRequestReceived,

        /// <summary>
        /// A friend request sent by the recipient was accepted.
        /// </summary>
        [EnumMember]
        FriendRequestAccepted,

        /// <summary>
        /// A friend request sent by the recipient was rejected.
        /// </summary>
        [EnumMember]
        FriendRequestRejected,

        /// <summary>
        /// The recipient received a match invitation.
        /// </summary>
        [EnumMember]
        InvitationReceived,

        /// <summary>
        /// An invitation sent by the recipient was accepted.
        /// </summary>
        [EnumMember]
        InvitationAccepted,

        /// <summary>
        /// An invitation sent by the recipient was rejected.
        /// </summary>
        [EnumMember]
        InvitationRejected,

        /// <summary>
        /// An invitation sent by the recipient expired without a response.
        /// </summary>
        [EnumMember]
        InvitationExpired
    }
}