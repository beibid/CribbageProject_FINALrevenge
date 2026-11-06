using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;

namespace CribbageRoyale.Contracts.ServiceContracts
{
    /// <summary>
    /// Operaciones que el servidor invoca sobre el cliente para notificarle cambios.
    /// </summary>
    public interface IGameClientCallback
    {
        /// <summary>
        /// Notifica que un jugador se conectó o se desconectó.
        /// </summary>
        /// <param name="presence">Cambio de presencia ocurrido.</param>
        [OperationContract(IsOneWay = true)]
        void PresenceChanged(PresenceChange presence);

        /// <summary>
        /// Notifica un hecho social fuera de la partida, como una solicitud de amistad recibida.
        /// </summary>
        /// <param name="eventType">Tipo de hecho ocurrido.</param>
        /// <param name="social">Detalle del hecho.</param>
        [OperationContract(IsOneWay = true)]
        void SocialEventOccurred(SocialEventType eventType, SocialEvent social);

        /// <summary>
        /// Notifica que un amigo envió un mensaje de chat al destinatario.
        /// </summary>
        /// <param name="message">Mensaje recibido.</param>
        [OperationContract(IsOneWay = true)]
        void ChatMessageReceived(ChatMessage message);

        /// <summary>
        /// Notifica que la sesión del destinatario dejó de ser válida.
        /// </summary>
        /// <param name="revocation">Aviso con el motivo de la revocación.</param>
        [OperationContract(IsOneWay = true)]
        void SessionRevoked(SessionRevocation revocation);
    }
}