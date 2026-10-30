using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Sesión concedida a un jugador que entra sin cuenta registrada.
    /// </summary>
    [DataContract]
    public class GuestSession
    {
        /// <summary>
        /// Identidad temporal asignada al invitado.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Account { get; set; }

        /// <summary>
        /// Credencial que el cliente debe enviar en el encabezado de cada operación posterior.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string SessionToken { get; set; }
    }
}