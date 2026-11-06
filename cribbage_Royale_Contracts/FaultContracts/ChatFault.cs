using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Falla de negocio ocurrida al enviar un mensaje de chat.
    /// </summary>
    [DataContract]
    public class ChatFault
    {
        /// <summary>
        /// Motivo por el cual el envío fue rechazado.
        /// </summary>
        [DataMember(IsRequired = true)]
        public ChatFailure Reason { get; set; }
    }
}