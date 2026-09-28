using System;

namespace cribbage_Royale_PROYECTO.Models
{
    public class SuspendedAccountSummary
    {
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string SuspensionReason { get; set; }
        public DateTime SuspensionEndDate { get; set; }
    }
}