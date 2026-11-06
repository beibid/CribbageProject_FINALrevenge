using System;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Mensaje de chat enviado entre dos amigos.
    /// </summary>
    [DataContract]
    public class ChatMessage
    {
        /// <summary>
        /// Cuenta que escribió el mensaje.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Sender { get; set; }

        /// <summary>
        /// Texto del mensaje, ya validado por el servidor.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Text { get; set; }

        /// <summary>
        /// Instante en que el servidor recibió el mensaje.
        /// </summary>
        [DataMember(IsRequired = true)]
        public DateTime SentAtUtc { get; set; }
    }
}