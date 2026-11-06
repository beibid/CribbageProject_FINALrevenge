using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Contracts.ServiceContracts
{
    /// <summary>
    /// Invitaciones a partida entre amigos. Exige una sesión de cuenta registrada,
    /// enviada en el encabezado SessionToken.
    /// </summary>
    [ServiceContract]
    public interface IMatchInvitation
    {
        /// <summary>
        /// Invita a un amigo conectado a jugar una partida. El invitado recibe el aviso
        /// por su canal de notificaciones.
        /// </summary>
        /// <param name="accountId">Cuenta del amigo a invitar.</param>
        /// <returns>La invitación creada, con el identificador asignado por el servidor.</returns>
        /// <exception cref="FaultException{InvitationFault}">La invitación fue rechazada por una
        /// regla de invitaciones.</exception>
        /// <exception cref="FaultException{SessionFault}">El testigo falta, no es válido o pertenece
        /// a un invitado.</exception>
        /// <exception cref="FaultException{ServiceFault}">El servidor no pudo registrar la
        /// invitación.</exception>
        [OperationContract]
        [FaultContract(typeof(InvitationFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        MatchInvitation SendInvitation(int accountId);

        /// <summary>
        /// Acepta o rechaza una invitación recibida. Si se acepta, ambos jugadores reciben el
        /// aviso de inicio de partida por su canal de notificaciones.
        /// </summary>
        /// <param name="invitationId">Invitación a responder.</param>
        /// <param name="accepted">Verdadero para aceptar, falso para rechazar.</param>
        /// <exception cref="FaultException{InvitationFault}">La invitación no existe, ya fue
        /// atendida o no pertenece a quien llama.</exception>
        /// <exception cref="FaultException{SessionFault}">El testigo falta, no es válido o pertenece
        /// a un invitado.</exception>
        /// <exception cref="FaultException{ServiceFault}">El servidor no pudo registrar la
        /// respuesta.</exception>
        [OperationContract]
        [FaultContract(typeof(InvitationFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        void RespondToInvitation(long invitationId, bool accepted);

        /// <summary>
        /// Retira una invitación que quien llama envió y que sigue sin respuesta.
        /// </summary>
        /// <param name="invitationId">Invitación a retirar.</param>
        /// <exception cref="FaultException{InvitationFault}">La invitación no existe, ya fue
        /// atendida o no pertenece a quien llama.</exception>
        /// <exception cref="FaultException{SessionFault}">El testigo falta, no es válido o pertenece
        /// a un invitado.</exception>
        /// <exception cref="FaultException{ServiceFault}">El servidor no pudo retirar la
        /// invitación.</exception>
        [OperationContract]
        [FaultContract(typeof(InvitationFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        void CancelInvitation(long invitationId);
    }
}