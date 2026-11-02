using System.ServiceModel;
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
    }
}