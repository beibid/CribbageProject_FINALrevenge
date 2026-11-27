using System;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Pending friend request.
    /// </summary>
    [DataContract]
    public class FriendRequest
    {
        /// <summary>
        /// Stable identifier of the request.
        /// </summary>
        [DataMember(IsRequired = true)]
        public long RequestId { get; set; }

        /// <summary>
        /// The other account involved.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Counterpart { get; set; }

        /// <summary>
        /// Received or sent, from the point of view of the caller.
        /// </summary>
        [DataMember(IsRequired = true)]
        public RequestDirection Direction { get; set; }

        /// <summary>
        /// UTC instant at which the server recorded the request.
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime CreatedAtUtc { get; set; }
    }
}