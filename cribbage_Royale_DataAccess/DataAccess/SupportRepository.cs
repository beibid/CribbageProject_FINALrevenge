using cribbage_Royale_DataAccess.DataAccess;
using cribbage_Royale_DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class SupportRepository
    {
        public List<PendingReportSummary> GetPendingReports()
        {
            List<PendingReportSummary> pendingReports;
            using (CribbageEntities context = new CribbageEntities())
            {
                pendingReports = context.report
                    .Where(currentReport => currentReport.report_status == ReportStatus.Pending)
                    .Select(currentReport => new PendingReportSummary
                    {
                        ReportId = (int)currentReport.report_id,
                        ReporterUsername = currentReport.account1.username,
                        ReportedAccountId = (int)currentReport.reported_account_id,
                        ReportedUsername = currentReport.account.username,
                        Reason = currentReport.reason,
                        SubmittedDate = currentReport.reported_at,
                        MatchId = (int?)currentReport.match_id
                    })
                    .ToList();
            }
            return pendingReports;
        }

        public List<SuspendedAccountSummary> GetActiveSuspensions()
        {
            List<SuspendedAccountSummary> suspendedAccounts;
            using (CribbageEntities context = new CribbageEntities())
            {
                DateTime now = DateTime.Now;
                suspendedAccounts = context.sanction
                    .Where(currentSanction => currentSanction.sanction_type == SanctionType.Suspension && currentSanction.revoked_at == null && currentSanction.ends_at > now)
                    .Select(currentSanction => new SuspendedAccountSummary
                    {
                        AccountId = currentSanction.sanctioned_account_id,
                        Username = currentSanction.account.username,
                        SuspensionReason = currentSanction.reason,
                        SuspensionEndDate = currentSanction.ends_at.Value
                    })
                    .ToList();
            }
            return suspendedAccounts;
        }

        public AccountSanctionStatus GetSanctionStatus(int accountId)
        {
            AccountSanctionStatus sanctionStatus;
            using (CribbageEntities context = new CribbageEntities())
            {
                sanction activeSanction = FindActiveSuspension(context, accountId);
                sanctionStatus = new AccountSanctionStatus();
                if (activeSanction == null)
                {
                    sanctionStatus.IsCurrentlySuspended = false;
                }
                else
                {
                    sanctionStatus.IsCurrentlySuspended = true;
                    sanctionStatus.SuspensionEndDate = activeSanction.ends_at;
                    sanctionStatus.SuspensionReason = activeSanction.reason;
                }
            }
            return sanctionStatus;
        }

        public void ApplyNewSuspension(int accountId, int? reportId, string reason, DateTime suspensionEndDate, int supportAccountId)
        {
            using (CribbageEntities context = new CribbageEntities())
            {
                sanction newSanction = new sanction();
                newSanction.sanctioned_account_id = accountId;
                newSanction.support_account_id = supportAccountId;
                newSanction.report_id = reportId;
                newSanction.sanction_type = SanctionType.Suspension;
                newSanction.reason = reason;
                newSanction.starts_at = DateTime.Now;
                newSanction.ends_at = suspensionEndDate;
                context.sanction.Add(newSanction);

                account sanctionedUser = context.account.First(currentUser => currentUser.account_id == accountId);
                sanctionedUser.account_status = AccountStatus.Suspended;

                RemoveActiveSessions(context, accountId);
                AddSystemLogEvent(context, accountId, "SUSPENSION_APPLIED", reason);
                context.SaveChanges();
            }
        }

        public void UpdateSuspensionDuration(int accountId, string reason, DateTime suspensionEndDate, int supportAccountId)
        {
            using (CribbageEntities context = new CribbageEntities())
            {
                sanction activeSanction = FindActiveSuspension(context, accountId);
                activeSanction.reason = reason;
                activeSanction.ends_at = suspensionEndDate;
                activeSanction.support_account_id = supportAccountId;
                AddSystemLogEvent(context, accountId, "SUSPENSION_MODIFIED", reason);
                context.SaveChanges();
            }
        }

        public void ReactivateAccount(int accountId, int supportAccountId)
        {
            using (CribbageEntities context = new CribbageEntities())
            {
                sanction activeSanction = FindActiveSuspension(context, accountId);
                activeSanction.revoked_at = DateTime.Now;
                activeSanction.support_account_id = supportAccountId;

                account reactivatedUser = context.account.First(currentUser => currentUser.account_id == accountId);
                reactivatedUser.account_status = AccountStatus.Verified;

                AddSystemLogEvent(context, accountId, "ACCOUNT_REACTIVATED", string.Empty);
                context.SaveChanges();
            }
        }

        private sanction FindActiveSuspension(CribbageEntities context, int accountId)
        {
            DateTime now = DateTime.Now;
            sanction activeSanction = context.sanction.FirstOrDefault(currentSanction => currentSanction.sanctioned_account_id == accountId && currentSanction.sanction_type == SanctionType.Suspension && currentSanction.revoked_at == null && currentSanction.ends_at > now);
            return activeSanction;
        }

        private void RemoveActiveSessions(CribbageEntities context, int accountId)
        {
            List<session> activeSessions = context.session.Where(currentSession => currentSession.account_id == accountId).ToList();
            foreach (session currentSession in activeSessions)
            {
                context.session.Remove(currentSession);
            }
        }

        private void AddSystemLogEvent(CribbageEntities context, int accountId, string eventType, string description)
        {
            event_log newEvent = new event_log();
            newEvent.account_id = accountId;
            newEvent.event_type = eventType;
            newEvent.description = description;
            newEvent.logged_at = DateTime.Now;
            context.event_log.Add(newEvent);
        }
    }
}