using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Technical fault codes carried by <see cref="ServiceFault"/>.
    /// </summary>
    [DataContract]
    public enum ServiceFaultCode
    {
        /// <summary>
        /// The operation failed while reading from or writing to the database.
        /// </summary>
        [EnumMember]
        DataAccessFailure,

        /// <summary>
        /// The operation could not deliver an email.
        /// </summary>
        [EnumMember]
        MailDeliveryFailure,

        /// <summary>
        /// The operation failed due to an unexpected condition inside the server.
        /// </summary>
        [EnumMember]
        InternalFailure
    }
}