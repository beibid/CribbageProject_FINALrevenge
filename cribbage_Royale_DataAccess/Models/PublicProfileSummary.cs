using System;

namespace cribbage_Royale_DataAccess.Models
{
    public class PublicProfileSummary
    {
        public int AccountId { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int MatchesPlayed { get; set; }
        public int Wins { get; set; }
        public decimal WinPercentage { get; set; }
        public int SkillLevel { get; set; }
        public string RelationshipStatus { get; set; }
    }
}