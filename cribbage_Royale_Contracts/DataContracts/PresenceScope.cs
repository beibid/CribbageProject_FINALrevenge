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
        /// El cambio corresponde al rival de la partida en curso.
        /// </summary>
        [EnumMember]
        Rival,

        /// <summary>
        /// El cambio corresponde a un amigo del destinatario.
        /// </summary>
        [EnumMember]
        Friend
    }
}