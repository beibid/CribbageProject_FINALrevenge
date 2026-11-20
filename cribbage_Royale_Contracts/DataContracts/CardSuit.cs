using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Palo de una carta.
    /// </summary>
    [DataContract]
    public enum CardSuit
    {
        [EnumMember]
        Clubs = 0,

        [EnumMember]
        Diamonds = 1,

        [EnumMember]
        Hearts = 2,

        [EnumMember]
        Spades = 3
    }
}