using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Etapa en la que se encuentra una partida.
    /// </summary>
    [DataContract]
    public enum MatchPhase
    {
        [EnumMember]
        Searching = 0,

        [EnumMember]
        Discarding = 1,

        [EnumMember]
        Playing = 2,

        [EnumMember]
        Counting = 3,

        [EnumMember]
        Finished = 4
    }
}