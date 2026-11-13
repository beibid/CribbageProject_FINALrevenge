using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.FaultContracts
{
    /// <summary>
    /// Motivos por los que el servidor rechaza una acción dentro de una partida.
    /// </summary>
    [DataContract]
    public enum MatchFailure
    {
        /// <summary>
        /// No existe una partida en curso con ese identificador.
        /// </summary>
        [EnumMember]
        MatchNotFound,

        /// <summary>
        /// Quien llama no es uno de los dos jugadores de la partida.
        /// </summary>
        [EnumMember]
        NotAParticipant,

        /// <summary>
        /// La partida no está en la etapa que permite esta acción.
        /// </summary>
        [EnumMember]
        MatchNotInProgress,

        /// <summary>
        /// El descarte no es válido: no son dos cartas o no están en la mano.
        /// </summary>
        [EnumMember]
        InvalidDiscard
    }
}