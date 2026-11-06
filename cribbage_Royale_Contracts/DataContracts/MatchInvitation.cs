using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Invitación a partida entre dos amigos, con las dos cuentas involucradas.
    /// </summary>
    [DataContract]
    public class MatchInvitation
    {
        /// <summary>
        /// Identificador que el servidor asignó a la invitación.
        /// </summary>
        [DataMember(IsRequired = true)]
        public long InvitationId { get; set; }

        /// <summary>
        /// Cuenta que envió la invitación.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Sender { get; set; }

        /// <summary>
        /// Cuenta que recibió la invitación.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Recipient { get; set; }
    }
}