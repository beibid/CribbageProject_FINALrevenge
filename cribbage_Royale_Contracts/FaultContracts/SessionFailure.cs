using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Reasons the server rejects signing in, signing out, or an operation that requires a valid
    /// session.
    /// </summary>
    [DataContract]
    public enum SessionFailure
    {
        /// <summary>
        /// The username, email or password does not match.
        /// </summary>
        [EnumMember]
        InvalidCredentials,

        /// <summary>
        /// The account exists but has not been verified yet.
        /// </summary>
        [EnumMember]
        AccountNotVerified,

        /// <summary>
        /// The account is suspended.
        /// </summary>
        [EnumMember]
        AccountSuspended,

        /// <summary>
        /// The account was deleted.
        /// </summary>
        [EnumMember]
        AccountDeleted,

        /// <summary>
        /// The session token header is missing or empty.
        /// </summary>
        [EnumMember]
        MissingSessionToken,

        /// <summary>
        /// The session token is invalid or has been revoked.
        /// </summary>
        [EnumMember]
        InvalidSession,

        /// <summary>
        /// The operation requires a registered account and the session belongs to a guest.
        /// </summary>
        [EnumMember]
        GuestNotAllowed,

        /// <summary>
        /// The name proposed by the guest has an invalid format.
        /// </summary>
        [EnumMember]
        InvalidGuestName,

        /// <summary>
        /// The name proposed by the guest is already in use.
        /// </summary>
        [EnumMember]
        GuestNameUnavailable
    }
}