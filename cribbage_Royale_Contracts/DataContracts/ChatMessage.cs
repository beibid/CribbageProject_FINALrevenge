using System;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Chat message sent between two friends.
    /// </summary>
    [DataContract]
    public class ChatMessage
    {
        /// <summary>
        /// Account that wrote the message.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Sender { get; set; }

        /// <summary>
        /// Message text, already validated by the server.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Text { get; set; }

        /// <summary>
        /// Instant at which the server received the message.
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime SentAtUtc { get; set; }
    }
}