using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Business fault returned by the match play service.
    /// </summary>
    [DataContract]
    public class MatchFault
    {
        /// <summary>
        /// Reason the operation was rejected.
        /// </summary>
        [DataMember(IsRequired = true)]
        public MatchFailure Reason { get; set; }
    }
}