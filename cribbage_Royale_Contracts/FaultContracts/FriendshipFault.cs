using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Falla de negocio ocurrida en una operación de amistades, búsquedas o bloqueos.
    /// </summary>
    [DataContract]
    public class FriendshipFault
    {
        /// <summary>
        /// Motivo por el cual la operación fue rechazada.
        /// </summary>
        [DataMember(IsRequired = true)]
        public FriendshipFailure Reason { get; set; }
    }
}