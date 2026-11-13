using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Rango de una carta. El valor numérico corresponde al orden de la carta
    /// dentro del palo y se usa para formar escaleras.
    /// </summary>
    [DataContract]
    public enum CardRank
    {
        [EnumMember]
        Ace = 1,

        [EnumMember]
        Two = 2,

        [EnumMember]
        Three = 3,

        [EnumMember]
        Four = 4,

        [EnumMember]
        Five = 5,

        [EnumMember]
        Six = 6,

        [EnumMember]
        Seven = 7,

        [EnumMember]
        Eight = 8,

        [EnumMember]
        Nine = 9,

        [EnumMember]
        Ten = 10,

        [EnumMember]
        Jack = 11,

        [EnumMember]
        Queen = 12,

        [EnumMember]
        King = 13
    }
}