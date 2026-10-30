using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Sesión concedida a un jugador que inició sesión con sus credenciales.
    /// </summary>
    [DataContract]
    public class AuthenticatedSession
    {
        /// <summary>
        /// Jugador al que pertenece la sesión.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Account { get; set; }

        /// <summary>
        /// Correo electrónico de la cuenta, devuelto únicamente a su propio dueño.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string Email { get; set; }

        /// <summary>
        /// Credencial que el cliente debe enviar en el encabezado de cada operación posterior.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string SessionToken { get; set; }
    }
}