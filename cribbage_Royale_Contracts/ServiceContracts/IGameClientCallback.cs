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
        /// Notifica que la sesión del destinatario dejó de ser válida.
        /// </summary>
        /// <param name="revocation">Aviso con el motivo de la revocación.</param>
        [OperationContract(IsOneWay = true)]
        void SessionRevoked(SessionRevocation revocation);
    }
}