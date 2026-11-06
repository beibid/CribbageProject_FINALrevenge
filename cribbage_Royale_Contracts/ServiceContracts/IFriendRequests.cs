using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Contracts.ServiceContracts
{
    /// <summary>
    /// Solicitudes de amistad. Exige una sesión de cuenta registrada, enviada en el encabezado
    /// SessionToken.
    /// </summary>
    [ServiceContract]
    public interface IFriendRequests
    {
        /// <summary>
        /// Envía una solicitud de amistad de quien llama a otra cuenta. El destinatario recibe el aviso
        /// por el callback si está conectado.
        /// </summary>
        /// <param name="accountId">Cuenta que recibe la solicitud.</param>
        /// <exception cref="FaultException{FriendshipFault}">La cuenta no es válida o no está disponible, es la
        /// propia, ya es amiga, hay una solicitud pendiente o existe un bloqueo entre las cuentas.</exception>
        /// <exception cref="FaultException{SessionFault}">El testigo falta, no es válido o pertenece a un
        /// invitado.</exception>
        /// <exception cref="FaultException{ServiceFault}">El servidor no pudo registrar la solicitud.</exception>
        [OperationContract]
        [FaultContract(typeof(FriendshipFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        void SendFriendRequest(int accountId);

        /// <summary>
        /// Acepta o rechaza una solicitud recibida. Solo la ejecuta el destinatario; el remitente recibe
        /// el aviso de la respuesta por el callback si está conectado.
        /// </summary>
        /// <param name="requestId">Solicitud que se responde.</param>
        /// <param name="accepted">Verdadero para aceptar la solicitud; falso para rechazarla.</param>
        /// <exception cref="FaultException{FriendshipFault}">La solicitud no existe o ya no está pendiente, o
        /// quien llama no es su destinatario.</exception>
        /// <exception cref="FaultException{SessionFault}">El testigo falta, no es válido o pertenece a un
        /// invitado.</exception>
        /// <exception cref="FaultException{ServiceFault}">El servidor no pudo registrar la respuesta.</exception>
        [OperationContract]
        [FaultContract(typeof(FriendshipFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        void RespondToFriendRequest(long requestId, bool accepted);

        /// <summary>
        /// Cancela una solicitud enviada. Solo la ejecuta el remitente y solo mientras siga pendiente.
        /// </summary>
        /// <param name="requestId">Solicitud que se cancela.</param>
        /// <exception cref="FaultException{FriendshipFault}">La solicitud no existe o ya no está pendiente, o
        /// quien llama no es su remitente.</exception>
        /// <exception cref="FaultException{SessionFault}">El testigo falta, no es válido o pertenece a un
        /// invitado.</exception>
        /// <exception cref="FaultException{ServiceFault}">El servidor no pudo cancelar la solicitud.</exception>
        [OperationContract]
        [FaultContract(typeof(FriendshipFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        void CancelFriendRequest(long requestId);

        /// <summary>
        /// Obtiene las solicitudes pendientes recibidas y enviadas por quien llama, distinguidas por
        /// Direction.
        /// </summary>
        /// <returns>Las solicitudes pendientes; un arreglo vacío si no tiene ninguna.</returns>
        /// <exception cref="FaultException{FriendshipFault}">La operación fue rechazada por una regla de
        /// amistades.</exception>
        /// <exception cref="FaultException{SessionFault}">El testigo falta, no es válido o pertenece a un
        /// invitado.</exception>
        /// <exception cref="FaultException{ServiceFault}">El servidor no pudo completar la consulta.</exception>
        [OperationContract]
        [FaultContract(typeof(FriendshipFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        FriendRequest[] GetPendingFriendRequests();
    }
}