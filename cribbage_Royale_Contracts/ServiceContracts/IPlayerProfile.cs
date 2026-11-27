using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Contracts.ServiceContracts
{
    /// <summary>
    /// Profile of the player who owns the session. Requires a registered account session, sent in
    /// the SessionToken header.
    /// </summary>
    [ServiceContract]
    public interface IPlayerProfile
    {
        /// <summary>
        /// Gets the caller's profile.
        /// </summary>
        /// <returns>The caller's profile data.</returns>
        /// <exception cref="FaultException{SessionFault}">The token is missing, invalid, or belongs to a
        /// guest.</exception>
        /// <exception cref="FaultException{ServiceFault}">The server could not complete the query.</exception>
        [OperationContract]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        OwnProfile GetOwnProfile();

        /// <summary>
        /// Changes the name shown to other players.
        /// </summary>
        /// <param name="displayName">New display name.</param>
        /// <exception cref="FaultException{ProfileFault}">The display name is invalid.</exception>
        /// <exception cref="FaultException{SessionFault}">The token is missing, invalid, or belongs to a
        /// guest.</exception>
        /// <exception cref="FaultException{ServiceFault}">The server could not save the change.</exception>
        [OperationContract]
        [FaultContract(typeof(ProfileFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        void UpdateDisplayName(string displayName);

        /// <summary>
        /// Changes the password after checking the current one.
        /// </summary>
        /// <param name="currentPassword">Current password of the player.</param>
        /// <param name="newPassword">New password.</param>
        /// <exception cref="FaultException{ProfileFault}">The current password is wrong or the new one is
        /// invalid.</exception>
        /// <exception cref="FaultException{SessionFault}">The token is missing, invalid, or belongs to a
        /// guest.</exception>
        /// <exception cref="FaultException{ServiceFault}">The server could not save the change.</exception>
        [OperationContract]
        [FaultContract(typeof(ProfileFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        void ChangePassword(string currentPassword, string newPassword);

        /// <summary>
        /// Replaces the avatar with a new JPEG image.
        /// </summary>
        /// <param name="image">JPEG image of 15 KB or less.</param>
        /// <exception cref="FaultException{ProfileFault}">The image is empty, too large, or is not a
        /// JPEG image.</exception>
        /// <exception cref="FaultException{SessionFault}">The token is missing, invalid, or belongs to a
        /// guest.</exception>
        /// <exception cref="FaultException{ServiceFault}">The server could not save the change.</exception>
        [OperationContract]
        [FaultContract(typeof(ProfileFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        void UpdateAvatar(byte[] image);
    }
}