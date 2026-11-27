using System;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Private session data of a registered player, returned only to the account owner.
    /// </summary>
    [DataContract]
    public class AuthenticatedSession
    {
        /// <summary>
        /// Opaque token issued by the server. The client sends it in the SessionToken header of every
        /// subsequent operation.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string SessionToken { get; set; }

        /// <summary>
        /// Public identity of the account that owns the session.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Account { get; set; }

        /// <summary>
        /// Email address of the account, returned only to the account owner.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Preferred locale code of the player, for example es-MX.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string PreferredLocaleCode { get; set; }

        /// <summary>
        /// UTC instant at which the session expires.
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime ExpiresAtUtc { get; set; }

        /// <summary>
        /// Whether the account belongs to the support team. The client uses it to decide whether to
        /// show the support panel.
        /// </summary>
        [DataMember(IsRequired = true)]
        public bool IsSupportAccount { get; set; }

        /// <summary>
        /// Unfinished match offered for resumption, or null if there is none.
        /// </summary>
        [DataMember]
        public long? UnfinishedMatchId { get; set; }
    }
}