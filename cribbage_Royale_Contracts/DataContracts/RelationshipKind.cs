using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Relationship between the caller and another account.
    /// </summary>
    [DataContract]
    public enum RelationshipKind
    {
        /// <summary>
        /// There is no relationship between the accounts.
        /// </summary>
        [EnumMember]
        None,

        /// <summary>
        /// The accounts are friends.
        /// </summary>
        [EnumMember]
        Friend,

        /// <summary>
        /// The caller sent a request that is still pending.
        /// </summary>
        [EnumMember]
        RequestSent,

        /// <summary>
        /// The caller received a request that is still pending.
        /// </summary>
        [EnumMember]
        RequestReceived,

        /// <summary>
        /// A block exists between the accounts.
        /// </summary>
        [EnumMember]
        Blocked
    }
}