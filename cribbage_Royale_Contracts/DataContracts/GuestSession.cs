using System;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Guest session with the display name confirmed by the server.
    /// </summary>
    [DataContract]
    public class GuestSession
    {
        /// <summary>
        /// Opaque guest session token. The client sends it in the SessionToken header of every
        /// subsequent operation.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string SessionToken { get; set; }

        /// <summary>
        /// Final guest display name, including the reserved prefix added by the server.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string DisplayName { get; set; }

        /// <summary>
        /// UTC instant at which the session expires.
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime ExpiresAtUtc { get; set; }
    }
}