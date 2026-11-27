using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Reason why a profile operation was rejected.
    /// </summary>
    [DataContract]
    public enum ProfileFailure
    {
        /// <summary>
        /// The display name is empty or longer than allowed.
        /// </summary>
        [EnumMember]
        InvalidDisplayName = 0,

        /// <summary>
        /// The new password does not meet the password rules.
        /// </summary>
        [EnumMember]
        InvalidPassword = 1,

        /// <summary>
        /// The current password is wrong.
        /// </summary>
        [EnumMember]
        WrongPassword = 2,

        /// <summary>
        /// The image is empty, too large, or is not a JPEG image.
        /// </summary>
        [EnumMember]
        InvalidImage = 3
    }
}