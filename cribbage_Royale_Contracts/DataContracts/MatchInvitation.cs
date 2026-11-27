using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Match invitation between two friends, with both accounts involved.
    /// </summary>
    [DataContract]
    public class MatchInvitation
    {
        /// <summary>
        /// Identifier assigned to the invitation by the server.
        /// </summary>
        [DataMember(IsRequired = true)]
        public long InvitationId { get; set; }

        /// <summary>
        /// Account that sent the invitation.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Sender { get; set; }

        /// <summary>
        /// Account that received the invitation.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Recipient { get; set; }
    }
}