using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Public reference to an account. Reused by every object that needs to identify a player.
    /// </summary>
    [DataContract]
    public class AccountReference
    {
        /// <summary>
        /// Stable identifier of the account.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int AccountId { get; set; }

        /// <summary>
        /// Public username.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string UserName { get; set; }

        /// <summary>
        /// Display name chosen by the player.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string DisplayName { get; set; }
    }
}