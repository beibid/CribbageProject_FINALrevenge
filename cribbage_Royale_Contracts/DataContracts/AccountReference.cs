using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Identifica a un jugador dentro del servidor sin exponer sus datos de cuenta.
    /// </summary>
    [DataContract]
    public class AccountReference
    {
        /// <summary>
        /// Identificador de la cuenta del jugador.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int AccountId { get; set; }

        /// <summary>
        /// Nombre con el que el jugador se muestra a los demás.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string DisplayName { get; set; }
    }
}