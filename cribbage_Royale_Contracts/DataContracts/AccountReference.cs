using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Referencia pública de una cuenta. Se reutiliza en todos los objetos que necesitan nombrar a un
    /// jugador.
    /// </summary>
    [DataContract]
    public class AccountReference
    {
        /// <summary>
        /// Identificador estable de la cuenta.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int AccountId { get; set; }

        /// <summary>
        /// Nombre de usuario público.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string UserName { get; set; }

        /// <summary>
        /// Nombre visible elegido por el jugador.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string DisplayName { get; set; }
    }
}