using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Contracts.ServiceContracts
{
    /// <summary>
    /// Player search and friend list with presence. Requires a registered account session, sent in
    /// the SessionToken header.
    /// </summary>
    [ServiceContract]
    public interface IFriendList
    {
        /// <summary>
        /// Searches players by username. Excludes the caller and any account with a block in either
        /// direction.
        /// </summary>
        /// <param name="searchText">Text that must appear in the username.</param>
        /// <returns>The players found; an empty array if there are no matches.</returns>
        /// <exception cref="FaultException{FriendshipFault}">The search text is empty or has an invalid
        /// format.</exception>
        /// <exception cref="FaultException{SessionFault}">The token is missing, invalid, or belongs to a
        /// guest.</exception>
        /// <exception cref="FaultException{ServiceFault}">The server could not complete the search.</exception>
        [OperationContract]
        [FaultContract(typeof(FriendshipFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        PlayerSummary[] FindPlayers(string searchText);

        /// <summary>
        /// Gets the caller's friends, with the presence known to the server at query time.
        /// </summary>
        /// <returns>The caller's friends; an empty array if there are none.</returns>
        /// <exception cref="FaultException{FriendshipFault}">The operation was rejected by a friendship
        /// rule.</exception>
        /// <exception cref="FaultException{SessionFault}">The token is missing, invalid, or belongs to a
        /// guest.</exception>
        /// <exception cref="FaultException{ServiceFault}">The server could not complete the query.</exception>
        [OperationContract]
        [FaultContract(typeof(FriendshipFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        FriendSummary[] GetFriends();


        /// <summary>
        /// Removes the friendship between the caller and another account, for both sides. The removed
        /// friend is not notified.
        /// </summary>
        /// <param name="accountId">Account of the friend to remove.</param>
        /// <exception cref="FaultException{FriendshipFault}">The account is invalid, is the caller's own, or
        /// has no active friendship with the caller.</exception>
        /// <exception cref="FaultException{SessionFault}">The token is missing, invalid, or belongs to a
        /// guest.</exception>
        /// <exception cref="FaultException{ServiceFault}">The server could not remove the friendship.</exception>
        [OperationContract]
        [FaultContract(typeof(FriendshipFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        void RemoveFriend(int accountId);
    }
}