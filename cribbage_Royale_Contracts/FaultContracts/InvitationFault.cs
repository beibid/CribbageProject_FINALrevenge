using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Falla de negocio devuelta por el servicio de invitaciones a partida.
    /// </summary>
    [DataContract]
    public class InvitationFault
    {
        /// <summary>
        /// Motivo por el que se rechazó la operación.
        /// </summary>
        [DataMember(IsRequired = true)]
        public InvitationFailure Reason { get; set; }
    }
}