using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Carta de la baraja inglesa. Es el único tipo de carta del sistema:
    /// se usa tanto en los contratos como en el proyecto de reglas.
    /// </summary>
    [DataContract]
    public class Card
    {
        /// <summary>
        /// Rango de la carta.
        /// </summary>
        [DataMember]
        public CardRank Rank { get; set; }

        /// <summary>
        /// Palo de la carta.
        /// </summary>
        [DataMember]
        public CardSuit Suit { get; set; }
    }
}