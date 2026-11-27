using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Fault sent when a profile operation is rejected.
    /// </summary>
    [DataContract]
    public class ProfileFault
    {
        /// <summary>
        /// Reason why the operation was rejected.
        /// </summary>
        [DataMember]
        public ProfileFailure Reason { get; set; }
    }
}