using System;

namespace cribbage_Royale_DataAccess.Models
{
    public class AccountSanctionStatus
    {
        public bool IsCurrentlySuspended { get; set; }
        public DateTime? SuspensionEndDate { get; set; }
        public string SuspensionReason { get; set; }
    }
}