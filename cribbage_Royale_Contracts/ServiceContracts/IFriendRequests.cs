using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Contracts.ServiceContracts
{
    /// <summary>
    /// Friend requests. Requires a registered account session, sent in the SessionToken header.
    /// </summary>
    [ServiceContract]
    public interface IFriendRequests
    {
        /// <summary>
        /// Sends a friend request from the caller to another account. The recipient is notified through
        /// the callback if connected.
        /// </summary>
        /// <param name="accountId">Account that receives the request.</param>
        /// <exception cref="FaultException{FriendshipFault}">The account is invalid or unavailable, is the
        /// caller's own, is already a friend, a request is pending, or a block exists between the
        /// accounts.</exception>
        /// <exception cref="FaultException{SessionFault}">The token is missing, invalid, or belongs to a
        /// guest.</exception>
        /// <exception cref="FaultException{ServiceFault}">The server could not record the request.</exception>
        [OperationContract]
        [FaultContract(typeof(FriendshipFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        void SendFriendRequest(int accountId);

        /// <summary>
        /// Accepts or rejects a received request. Only the recipient may call it; the sender is notified
        /// of the response through the callback if connected.
        /// </summary>
        /// <param name="requestId">Request being answered.</param>
        /// <param name="accepted">True to accept the request; false to reject it.</param>
        /// <exception cref="FaultException{FriendshipFault}">The request does not exist or is no longer
        /// pending, or the caller is not its recipient.</exception>
        /// <exception cref="FaultException{SessionFault}">The token is missing, invalid, or belongs to a
        /// guest.</exception>
        /// <exception cref="FaultException{ServiceFault}">The server could not record the response.</exception>
        [OperationContract]
        [FaultContract(typeof(FriendshipFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        void RespondToFriendRequest(long requestId, bool accepted);

        /// <summary>
        /// Cancels a sent request. Only the sender may call it, and only while the request is pending.
        /// </summary>
        /// <param name="requestId">Request to cancel.</param>
        /// <exception cref="FaultException{FriendshipFault}">The request does not exist or is no longer
        /// pending, or the caller is not its sender.</exception>
        /// <exception cref="FaultException{SessionFault}">The token is missing, invalid, or belongs to a
        /// guest.</exception>
        /// <exception cref="FaultException{ServiceFault}">The server could not cancel the request.</exception>
        [OperationContract]
        [FaultContract(typeof(FriendshipFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        void CancelFriendRequest(long requestId);

        /// <summary>
        /// Gets the pending requests received and sent by the caller, distinguished by Direction.
        /// </summary>
        /// <returns>The pending requests; an empty array if there are none.</returns>
        /// <exception cref="FaultException{FriendshipFault}">The operation was rejected by a friendship
        /// rule.</exception>
        /// <exception cref="FaultException{SessionFault}">The token is missing, invalid, or belongs to a
        /// guest.</exception>
        /// <exception cref="FaultException{ServiceFault}">The server could not complete the query.</exception>
        [OperationContract]
        [FaultContract(typeof(FriendshipFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        FriendRequest[] GetPendingFriendRequests();
    }
}