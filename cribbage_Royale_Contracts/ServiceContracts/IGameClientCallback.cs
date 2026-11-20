using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;

namespace CribbageRoyale.Contracts.ServiceContracts
{
    /// <summary>
    /// Operations the server invokes on the client to notify it of changes.
    /// </summary>
    public interface IGameClientCallback
    {
        /// <summary>
        /// Notifies that a player connected or disconnected.
        /// </summary>
        /// <param name="presence">Presence change that occurred.</param>
        [OperationContract(IsOneWay = true)]
        void PresenceChanged(PresenceChange presence);

        /// <summary>
        /// Notifies a social event outside the match, such as a friend request received.
        /// </summary>
        /// <param name="eventType">Type of event that occurred.</param>
        /// <param name="social">Details of the event.</param>
        [OperationContract(IsOneWay = true)]
        void SocialEventOccurred(SocialEventType eventType, SocialEvent social);

        /// <summary>
        /// Notifies that a friend sent a chat message to the recipient.
        /// </summary>
        /// <param name="message">Message received.</param>
        [OperationContract(IsOneWay = true)]
        void ChatMessageReceived(ChatMessage message);

        /// <summary>
        /// Tells the player that a friend invited them to a match.
        /// </summary>
        /// <param name="invitation">Invitation received.</param>
        [OperationContract(IsOneWay = true)]
        void MatchInvitationReceived(MatchInvitation invitation);

        /// <summary>
        /// Tells the sender that their invitation was declined or is no longer available.
        /// </summary>
        /// <param name="invitation">Invitation that is no longer standing.</param>
        [OperationContract(IsOneWay = true)]
        void MatchInvitationDeclined(MatchInvitation invitation);

        /// <summary>
        /// Tells the player that their match started and they can enter the table.
        /// </summary>
        /// <param name="matchStart">Data of the match that is starting.</param>
        [OperationContract(IsOneWay = true)]
        void MatchStarted(MatchStart matchStart);

        /// <summary>
        /// Notifies that the match state changed.
        /// </summary>
        /// <param name="eventType">Type of change that occurred.</param>
        /// <param name="state">Match state as seen by the recipient.</param>
        [OperationContract(IsOneWay = true)]
        void MatchStateChanged(MatchEventType eventType, MatchState state);

        /// <summary>
        /// Notifies that the recipient's session is no longer valid.
        /// </summary>
        /// <param name="revocation">Notice with the reason for the revocation.</param>
        [OperationContract(IsOneWay = true)]
        void SessionRevoked(SessionRevocation revocation);
    }
}