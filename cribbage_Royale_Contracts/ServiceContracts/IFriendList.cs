using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Contracts.ServiceContracts
{
    /// <summary>
    /// Búsqueda de jugadores y lista de amigos con su presencia. Exige una sesión de cuenta registrada,
    /// enviada en el encabezado SessionToken.
    /// </summary>
    [ServiceContract]
    public interface IFriendList
    {
        /// <summary>
        /// Busca jugadores por nombre de usuario. Omite a quien llama y a las cuentas con las que existe
        /// un bloqueo en cualquier sentido.
        /// </summary>
        /// <param name="searchText">Texto que debe aparecer en el nombre de usuario.</param>
        /// <returns>Los jugadores encontrados; un arreglo vacío si no hay coincidencias.</returns>
        /// <exception cref="FaultException{FriendshipFault}">El texto de búsqueda está vacío o no cumple el
        /// formato.</exception>
        /// <exception cref="FaultException{SessionFault}">El testigo falta, no es válido o pertenece a un
        /// invitado.</exception>
        /// <exception cref="FaultException{ServiceFault}">El servidor no pudo completar la búsqueda.</exception>
        [OperationContract]
        [FaultContract(typeof(FriendshipFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        PlayerSummary[] FindPlayers(string searchText);

        /// <summary>
        /// Obtiene los amigos de quien llama, con la presencia conocida por el servidor al consultar.
        /// </summary>
        /// <returns>Los amigos de quien llama; un arreglo vacío si no tiene ninguno.</returns>
        /// <exception cref="FaultException{FriendshipFault}">La operación fue rechazada por una regla de
        /// amistades.</exception>
        /// <exception cref="FaultException{SessionFault}">El testigo falta, no es válido o pertenece a un
        /// invitado.</exception>
        /// <exception cref="FaultException{ServiceFault}">El servidor no pudo completar la consulta.</exception>
        [OperationContract]
        [FaultContract(typeof(FriendshipFault))]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        FriendSummary[] GetFriends();
    }
}