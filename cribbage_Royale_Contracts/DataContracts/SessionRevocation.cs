using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Aviso de que la sesión del destinatario dejó de ser válida.
    /// </summary>
    [DataContract]
    public class SessionRevocation
    {
        /// <summary>
        /// Motivo por el cual se revocó la sesión.
        /// </summary>
        [DataMember(IsRequired = true)]
        public SessionRevocationReason Reason { get; set; }
    }
}