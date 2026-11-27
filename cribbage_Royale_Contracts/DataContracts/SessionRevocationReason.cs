using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Reason the server terminated a session.
    /// </summary>
    [DataContract]
    public enum SessionRevocationReason
    {
        /// <summary>
        /// The same account signed in from another client.
        /// </summary>
        [EnumMember]
        SignedInElsewhere,

        /// <summary>
        /// The account was suspended while the session was open.
        /// </summary>
        [EnumMember]
        AccountSuspended,

        /// <summary>
        /// The session reached its maximum lifetime.
        /// </summary>
        [EnumMember]
        SessionExpired
    }
}