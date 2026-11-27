using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Business fault raised while sending a chat message.
    /// </summary>
    [DataContract]
    public class ChatFault
    {
        /// <summary>
        /// Reason the message was rejected.
        /// </summary>
        [DataMember(IsRequired = true)]
        public ChatFailure Reason { get; set; }
    }
}