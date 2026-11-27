using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Business fault raised while opening or closing a player session.
    /// </summary>
    [DataContract]
    public class SessionFault
    {
        /// <summary>
        /// Reason the session operation was rejected.
        /// </summary>
        [DataMember(IsRequired = true)]
        public SessionFailure Failure { get; set; }
    }
}