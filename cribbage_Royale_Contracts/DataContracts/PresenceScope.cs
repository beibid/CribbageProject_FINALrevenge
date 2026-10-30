using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Ámbito al que corresponde un cambio de presencia.
    /// </summary>
    [DataContract]
    public enum PresenceScope
    {
        /// <summary>
        /// El cambio corresponde a la lista de amigos del destinatario.
        /// </summary>
        [EnumMember]
        Friends,

        /// <summary>
        /// El cambio corresponde a una partida en la que participa el destinatario.
        /// </summary>
        [EnumMember]
        Match
    }
}