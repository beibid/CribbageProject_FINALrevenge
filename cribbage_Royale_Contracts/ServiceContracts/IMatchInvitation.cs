using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Contracts.ServiceContracts
{
    /// <summary>
    /// Match invitations between friends. Requires a registered account session, sent in the
    /// SessionToken header.
    /// </summary>
    [ServiceContract]
    public interface IMatchInvitation
    {
        /// <summary>
        /// Invites a connected friend to play a match. The invited player is notified through their
        /// notification channel.
        /// </summary>
        /// <param name="accountId">Account of the friend to invite.</param>
        /// <returns>The created invitation, with the identifier assigned by the server.</returns>
        /// <exception cref="FaultException{InvitationFault}">The invitation was rejected by an
        /// invitation rule.</exception>
        /// <exception cref="FaultException{SessionFault}">The token is missing, invalid, or belongs
        /// to a guest.</exception>
        /// <exception cref="FaultException{ServiceFault}">The server could not record the
        /// invitation.</exception>
        [OperationContract]
        [FaultContract(typeof(InvitationFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        MatchInvitation SendInvitation(int accountId);

        /// <summary>
        /// Accepts or rejects a received invitation. If accepted, both players receive the match start
        /// notice through their notification channel.
        /// </summary>
        /// <param name="invitationId">Invitation to answer.</param>
        /// <param name="accepted">True to accept; false to reject.</param>
        /// <exception cref="FaultException{InvitationFault}">The invitation does not exist, has
        /// already been handled, or does not belong to the caller.</exception>
        /// <exception cref="FaultException{SessionFault}">The token is missing, invalid, or belongs
        /// to a guest.</exception>
        /// <exception cref="FaultException{ServiceFault}">The server could not record the
        /// response.</exception>
        [OperationContract]
        [FaultContract(typeof(InvitationFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        void RespondToInvitation(long invitationId, bool accepted);

        /// <summary>
        /// Withdraws an invitation the caller sent that has not been answered yet.
        /// </summary>
        /// <param name="invitationId">Invitation to withdraw.</param>
        /// <exception cref="FaultException{InvitationFault}">The invitation does not exist, has
        /// already been handled, or does not belong to the caller.</exception>
        /// <exception cref="FaultException{SessionFault}">The token is missing, invalid, or belongs
        /// to a guest.</exception>
        /// <exception cref="FaultException{ServiceFault}">The server could not withdraw the
        /// invitation.</exception>
        [OperationContract]
        [FaultContract(typeof(InvitationFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        void CancelInvitation(long invitationId);
    }
}