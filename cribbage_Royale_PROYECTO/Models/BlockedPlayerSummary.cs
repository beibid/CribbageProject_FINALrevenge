using System;

namespace cribbage_Royale_PROYECTO.Models
{
    public class BlockedPlayerSummary
    {
        public int AccountId { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public DateTime BlockDate { get; set; }
    }
}