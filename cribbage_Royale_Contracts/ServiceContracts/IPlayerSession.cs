using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Contracts.ServiceContracts
{
    /// <summary>
    /// Lets a player open and close a session on the server.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IGameClientCallback), SessionMode = SessionMode.Required)]
    public interface IPlayerSession
    {
        /// <summary>
        /// Opens a session for a registered player.
        /// </summary>
        /// <param name="userNameOrEmail">Username or email of the player.</param>
        /// <param name="password">Password of the player.</param>
        /// <returns>Session granted to the player.</returns>
        [OperationContract]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        AuthenticatedSession LogIn(string userNameOrEmail, string password);

        /// <summary>
        /// Opens a temporary session for a player without a registered account.
        /// </summary>
        /// <param name="displayName">Display name the guest wants to use.</param>
        /// <returns>Temporary session granted to the guest.</returns>
        [OperationContract]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        GuestSession LogInAsGuest(string displayName);

        /// <summary>
        /// Closes the session opened by the calling client.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        void LogOut();
    }
}