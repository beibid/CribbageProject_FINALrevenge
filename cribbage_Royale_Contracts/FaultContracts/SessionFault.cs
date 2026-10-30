using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Falla de negocio ocurrida al abrir o cerrar una sesión de jugador.
    /// </summary>
    [DataContract]
    public class SessionFault
    {
        /// <summary>
        /// Motivo por el cual la operación de sesión fue rechazada.
        /// </summary>
        [DataMember(IsRequired = true)]
        public SessionFailure Failure { get; set; }
    }
}