using System;

namespace cribbage_Royale_DataAccess.Models
{
    public class MatchInformationSummary
    {
        public long MatchId { get; set; }
        public int OpponentAccountId { get; set; }
        public string OpponentDisplayName { get; set; }
        public int OpponentSkillLevel { get; set; }
        public int ViewerScore { get; set; }
        public int OpponentScore { get; set; }
        public int HandNumber { get; set; }
        public string DealerDisplayName { get; set; }
        public string CutCardText { get; set; }
        public int RunningCount { get; set; }
        public int OpponentRemainingCards { get; set; }
        public DateTime? TurnDeadline { get; set; }
        public bool OpponentConnected { get; set; }
    }
}