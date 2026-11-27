using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Business fault raised by a friendship, search or block operation.
    /// </summary>
    [DataContract]
    public class FriendshipFault
    {
        /// <summary>
        /// Reason the operation was rejected.
        /// </summary>
        [DataMember(IsRequired = true)]
        public FriendshipFailure Reason { get; set; }
    }
}