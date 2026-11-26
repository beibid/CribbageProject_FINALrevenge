using System;
using System.Linq;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class ReportRepository
    {
        private const int FolioLength = 12;

        public string FindExistingReportFolio(int reporterAccountId, int reportedAccountId, int? matchId)
        {
            string existingFolio;
            using (CribbageEntities context = new CribbageEntities())
            {
                report existingReport = context.report.FirstOrDefault(currentReport => currentReport.reporter_account_id == reporterAccountId && currentReport.reported_account_id == reportedAccountId && currentReport.match_id == matchId);
                if (existingReport == null)
                {
                    existingFolio = null;
                }
                else
                {
                    existingFolio = existingReport.reference_code;
                }
            }
            return existingFolio;
        }

        public string SubmitReport(int reporterAccountId, int reportedAccountId, int? matchId, string reason, string comment)
        {
            string generatedFolio;
            using (CribbageEntities context = new CribbageEntities())
            {
                generatedFolio = GenerateFolio();
                report newReport = new report();
                newReport.reference_code = generatedFolio;
                newReport.reporter_account_id = reporterAccountId;
                newReport.reported_account_id = reportedAccountId;
                newReport.match_id = matchId;
                newReport.reason = reason;
                newReport.comment = comment;
                newReport.reported_at = DateTime.Now;
                newReport.report_status = ReportStatus.Pending;
                context.report.Add(newReport);
                context.SaveChanges();
            }
            return generatedFolio;
        }

        private string GenerateFolio()
        {
            string rawFolio = Guid.NewGuid().ToString("N").ToUpper();
            string folio = rawFolio.Substring(0, FolioLength);
            return folio;
        }
    }
}