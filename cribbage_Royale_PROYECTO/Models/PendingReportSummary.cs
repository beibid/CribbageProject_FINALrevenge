using System;

namespace cribbage_Royale_PROYECTO.Models
{
    public class PendingReportSummary
    {
        public int ReportId { get; set; }
        public string ReporterUsername { get; set; }
        public int ReportedAccountId { get; set; }
        public string ReportedUsername { get; set; }
        public string Reason { get; set; }
        public DateTime SubmittedDate { get; set; }
        public int? MatchId { get; set; }
    }
}