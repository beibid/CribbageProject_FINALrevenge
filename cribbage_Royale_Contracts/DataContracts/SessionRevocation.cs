using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Notice that the recipient's session is no longer valid.
    /// </summary>
    [DataContract]
    public class SessionRevocation
    {
        /// <summary>
        /// Reason the session was revoked.
        /// </summary>
        [DataMember(IsRequired = true)]
        public SessionRevocationReason Reason { get; set; }
    }
}