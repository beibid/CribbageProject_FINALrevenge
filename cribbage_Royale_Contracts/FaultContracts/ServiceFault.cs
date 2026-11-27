using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Technical server fault that does not correspond to any business rule.
    /// </summary>
    [DataContract]
    public class ServiceFault
    {
        /// <summary>
        /// Code that identifies the technical source of the fault.
        /// </summary>
        [DataMember(IsRequired = true)]
        public ServiceFaultCode Code { get; set; }

        /// <summary>
        /// Identifier used to locate the matching entry in the server log.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string CorrelationId { get; set; }
    }
}