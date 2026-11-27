using System;
using System.Runtime.Serialization;

namespace CribbageRoyale.Contracts.DataContracts
{
    /// <summary>
    /// Profile data of the player who owns the session.
    /// </summary>
    [DataContract]
    public class OwnProfile
    {
        /// <summary>
        /// Unique username. It cannot be changed.
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// Name shown to other players.
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Email of the account.
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// JPEG image of the avatar; an empty array if the player has not uploaded one.
        /// </summary>
        [DataMember]
        public byte[] AvatarImage { get; set; }

        /// <summary>
        /// Date the account was registered, in UTC.
        /// </summary>
        [DataMember]
        public DateTime RegisteredAtUtc { get; set; }

        /// <summary>
        /// Number of matches played.
        /// </summary>
        [DataMember]
        public int MatchesPlayed { get; set; }

        /// <summary>
        /// Number of matches won.
        /// </summary>
        [DataMember]
        public int Wins { get; set; }

        /// <summary>
        /// Number of matches lost.
        /// </summary>
        [DataMember]
        public int Losses { get; set; }

        /// <summary>
        /// Number of matches won in a row.
        /// </summary>
        [DataMember]
        public int CurrentStreak { get; set; }
    }
}