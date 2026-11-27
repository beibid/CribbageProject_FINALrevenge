using System;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Event outside of a match that concerns the recipient.
    /// </summary>
    [DataContract]
    public class SocialEvent
    {
        /// <summary>
        /// The other player involved.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Counterpart { get; set; }

        /// <summary>
        /// Friend request involved, if any; null otherwise.
        /// </summary>
        [DataMember]
        public long? FriendRequestId { get; set; }

        /// <summary>
        /// Match invitation involved, if any; null otherwise.
        /// </summary>
        [DataMember]
        public long? InvitationId { get; set; }

        /// <summary>
        /// UTC instant at which the event occurred.
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime OccurredAtUtc { get; set; }
    }
}