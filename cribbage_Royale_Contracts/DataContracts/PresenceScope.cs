using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Scope of a presence change.
    /// </summary>
    [DataContract]
    public enum PresenceScope
    {
        /// <summary>
        /// The change concerns the opponent in the current match.
        /// </summary>
        [EnumMember]
        Rival,

        /// <summary>
        /// The change concerns a friend of the recipient.
        /// </summary>
        [EnumMember]
        Friend
    }
}