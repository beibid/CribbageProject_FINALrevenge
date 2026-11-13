using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Estado de una partida visto por un jugador. El servidor arma uno
    /// distinto para cada jugador y nunca incluye las cartas del rival,
    /// el crib antes del conteo ni el mazo restante.
    /// </summary>
    [DataContract]
    public class MatchState
    {
        /// <summary>
        /// Identificador de la partida.
        /// </summary>
        [DataMember]
        public long MatchId { get; set; }

        /// <summary>
        /// Número de versión del estado, usado para descartar acciones atrasadas.
        /// </summary>
        [DataMember]
        public int StateVersion { get; set; }

        /// <summary>
        /// Etapa actual de la partida.
        /// </summary>
        [DataMember]
        public MatchPhase Phase { get; set; }

        /// <summary>
        /// Jugador rival.
        /// </summary>
        [DataMember]
        public AccountReference Rival { get; set; }

        /// <summary>
        /// Cartas en la mano del jugador que recibe este estado.
        /// </summary>
        [DataMember]
        public List<Card> OwnHand { get; set; }

        /// <summary>
        /// Cartas jugadas en la ronda actual.
        /// </summary>
        [DataMember]
        public List<Card> PlayedCards { get; set; }

        /// <summary>
        /// Cartas del crib. Permanece vacía hasta la etapa de conteo.
        /// </summary>
        [DataMember]
        public List<Card> CribCards { get; set; }

        /// <summary>
        /// Carta de corte. Es nula mientras no se ha volteado.
        /// </summary>
        [DataMember]
        public Card CutCard { get; set; }

        /// <summary>
        /// Puntos del jugador que recibe este estado.
        /// </summary>
        [DataMember]
        public int OwnScore { get; set; }

        /// <summary>
        /// Puntos del rival.
        /// </summary>
        [DataMember]
        public int RivalScore { get; set; }

        /// <summary>
        /// Identificador de la cuenta que reparte.
        /// </summary>
        [DataMember]
        public int DealerAccountId { get; set; }

        /// <summary>
        /// Suma de las cartas jugadas en la ronda actual.
        /// </summary>
        [DataMember]
        public int PeggingCount { get; set; }
    }
}