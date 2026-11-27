using System;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class ProfileRecord
    {
        public string UserName { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public byte[] AvatarImage { get; set; } = new byte[0];

        public DateTime RegisteredAt { get; set; }

        public int MatchesPlayed { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public int CurrentStreak { get; set; }
    }
}