using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Contracts.ServiceContracts
{
    /// <summary>
    /// Chat messages between friends. Requires a registered account session, sent in the
    /// SessionToken header.
    /// </summary>
    [ServiceContract]
    public interface IFriendChat
    {
        /// <summary>
        /// Sends a message to a connected friend. The recipient receives it through the callback.
        /// </summary>
        /// <param name="accountId">Account that receives the message.</param>
        /// <param name="text">Message text, from 1 to 500 characters.</param>
        /// <returns>The message as accepted by the server, with the timestamp the server assigned.</returns>
        /// <exception cref="FaultException{ChatFault}">The account is invalid or is the caller's own,
        /// the accounts are not friends, a block exists, the recipient is not connected, or the text has
        /// an invalid format.</exception>
        /// <exception cref="FaultException{SessionFault}">The token is missing, invalid, or belongs to a
        /// guest.</exception>
        /// <exception cref="FaultException{ServiceFault}">The server could not deliver the message.</exception>
        [OperationContract]
        [FaultContract(typeof(ChatFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        ChatMessage SendMessage(int accountId, string text);
    }
}