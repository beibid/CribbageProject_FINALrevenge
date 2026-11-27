using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Direction of a request from the point of view of the caller.
    /// </summary>
    [DataContract]
    public enum RequestDirection
    {
        /// <summary>
        /// The caller is the recipient of the request.
        /// </summary>
        [EnumMember]
        Received,

        /// <summary>
        /// The caller is the sender of the request.
        /// </summary>
        [EnumMember]
        Sent
    }
}