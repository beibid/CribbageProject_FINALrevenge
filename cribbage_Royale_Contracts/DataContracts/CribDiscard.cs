using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Petición de un jugador para enviar dos cartas de su mano al crib.
    /// </summary>
    [DataContract]
    public class CribDiscard
    {
        /// <summary>
        /// Identificador de la partida.
        /// </summary>
        [DataMember(IsRequired = true)]
        public long MatchId { get; set; }

        /// <summary>
        /// Las dos cartas que el jugador quiere descartar.
        /// </summary>
        [DataMember(IsRequired = true)]
        public List<Card> Cards { get; set; }

        /// <summary>
        /// Versión del estado que el jugador tenía al decidir el descarte.
        /// </summary>
        [DataMember(IsRequired = true)]
        public int ExpectedStateVersion { get; set; }
    }
}