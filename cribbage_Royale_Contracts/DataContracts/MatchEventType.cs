using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Tipo de cambio ocurrido en una partida.
    /// </summary>
    [DataContract]
    public enum MatchEventType
    {
        [EnumMember]
        Searching = 0,

        [EnumMember]
        Started = 1,

        [EnumMember]
        HandDealt = 2,

        [EnumMember]
        RivalPlayed = 3,

        [EnumMember]
        CutCardRevealed = 4,

        [EnumMember]
        CountFinished = 5,

        [EnumMember]
        RoundFinished = 6,

        [EnumMember]
        Resumed = 7,

        [EnumMember]
        Finished = 8
    }
}