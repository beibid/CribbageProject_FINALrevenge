using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Business fault returned by the match invitation service.
    /// </summary>
    [DataContract]
    public class InvitationFault
    {
        /// <summary>
        /// Reason the operation was rejected.
        /// </summary>
        [DataMember(IsRequired = true)]
        public InvitationFailure Reason { get; set; }
    }
}