using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;

namespace CribbageRoyale.Contracts.ServiceContracts
{
    /// <summary>
    /// Permite a un jugador abrir y cerrar su sesión en el servidor.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IGameClientCallback), SessionMode = SessionMode.Required)]
    public interface IPlayerSession
    {
        /// <summary>
        /// Abre una sesión para un jugador registrado.
        /// </summary>
        /// <param name="userNameOrEmail">Nombre de usuario o correo del jugador.</param>
        /// <param name="password">Contraseña del jugador.</param>
        /// <returns>Sesión concedida al jugador.</returns>
        [OperationContract]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        AuthenticatedSession LogIn(string userNameOrEmail, string password);

        /// <summary>
        /// Abre una sesión temporal para un jugador sin cuenta registrada.
        /// </summary>
        /// <param name="displayName">Nombre con el que el invitado quiere mostrarse.</param>
        /// <returns>Sesión temporal concedida al invitado.</returns>
        [OperationContract]
        [FaultContract(typeof(SessionFault))]
        [FaultContract(typeof(ServiceFault))]
        GuestSession LogInAsGuest(string displayName);

        /// <summary>
        /// Cierra la sesión abierta por el cliente que invoca la operación.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        void LogOut();
    }
}