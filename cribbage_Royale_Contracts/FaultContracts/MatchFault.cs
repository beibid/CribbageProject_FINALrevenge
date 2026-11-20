using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Falla de negocio devuelta por el servicio de juego.
    /// </summary>
    [DataContract]
    public class MatchFault
    {
        /// <summary>
        /// Motivo por el que se rechazó la operación.
        /// </summary>
        [DataMember(IsRequired = true)]
        public MatchFailure Reason { get; set; }
    }
}