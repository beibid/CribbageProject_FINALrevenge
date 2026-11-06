using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Contracts.ServiceContracts
{
    /// <summary>
    /// Mensajes de chat entre amigos. Exige una sesión de cuenta registrada, enviada en el
    /// encabezado SessionToken.
    /// </summary>
    [ServiceContract]
    public interface IFriendChat
    {
        /// <summary>
        /// Envía un mensaje a un amigo conectado. El destinatario lo recibe por el callback.
        /// </summary>
        /// <param name="accountId">Cuenta que recibe el mensaje.</param>
        /// <param name="text">Texto del mensaje, de 1 a 500 caracteres.</param>
        /// <returns>El mensaje tal como quedó registrado, con el instante que asignó el servidor.</returns>
        /// <exception cref="FaultException{ChatFault}">La cuenta no es válida o es la propia, no son
        /// amigos, existe un bloqueo, el destinatario no está conectado o el texto no cumple el
        /// formato.</exception>
        /// <exception cref="FaultException{SessionFault}">El testigo falta, no es válido o pertenece a un
        /// invitado.</exception>
        /// <exception cref="FaultException{ServiceFault}">El servidor no pudo entregar el mensaje.</exception>
        [OperationContract]
        [FaultContract(typeof(ChatFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        ChatMessage SendMessage(int accountId, string text);
    }
}