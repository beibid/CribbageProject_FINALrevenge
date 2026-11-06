using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Aviso de que una partida comenzó, con los datos que cada jugador necesita para entrar.
    /// </summary>
    [DataContract]
    public class MatchStart
    {
        /// <summary>
        /// Identificador que el servidor asignó a la partida.
        /// </summary>
        [DataMember(IsRequired = true)]
        public long MatchId { get; set; }

        /// <summary>
        /// El otro jugador de la mesa, visto desde quien recibe el aviso.
        /// </summary>
        [DataMember(IsRequired = true)]
        public AccountReference Opponent { get; set; }

        /// <summary>
        /// Indica si quien recibe el aviso es el anfitrión de la partida.
        /// </summary>
        [DataMember(IsRequired = true)]
        public bool IsHost { get; set; }
    }
}