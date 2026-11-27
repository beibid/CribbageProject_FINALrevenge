using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Player returned by a search.
    /// </summary>
    [DataContract]
    public class PlayerSummary
    {
        /// <summary>
        /// Public identity of the player.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Account { get; set; }

        /// <summary>
        /// Whether the player has a custom profile picture.
        /// </summary>
        [DataMember(IsRequired = true)]
        public bool HasProfileImage { get; set; }

        /// <summary>
        /// Relationship between the player and the caller who searched.
        /// </summary>
        [DataMember(IsRequired = true)]
        public RelationshipKind Relationship { get; set; }
    }
}