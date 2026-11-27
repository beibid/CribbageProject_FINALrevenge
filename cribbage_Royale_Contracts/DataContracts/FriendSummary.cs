using System;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Identity and presence of a friend.
    /// </summary>
    [DataContract]
    public class FriendSummary
    {
        /// <summary>
        /// Public identity of the friend.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Account { get; set; }

        /// <summary>
        /// Whether the friend has a custom profile picture.
        /// </summary>
        [DataMember(IsRequired = true)]
        public bool HasProfileImage { get; set; }

        /// <summary>
        /// Presence known to the server at query time.
        /// </summary>
        [DataMember(IsRequired = true)]
        public bool IsOnline { get; set; }

        /// <summary>
        /// UTC instant at which the friendship was established.
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime FriendsSinceUtc { get; set; }
    }
}