using cribbage_Royale_PROYECTO.DataAccess;
using cribbage_Royale_PROYECTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cribbage_Royale_PROYECTO.DataAccess
{
    public class SupportRepository
    {
        public List<PendingReportSummary> GetPendingReports()
        {
            List<PendingReportSummary> pendingReports;
            using (CribbageEntities context = new CribbageEntities())
            {
                pendingReports = context.reporte
                    .Where(currentReport => currentReport.estado == ReportStatus.Pending)
                    .Select(currentReport => new PendingReportSummary
                    {
                        ReportId = (int)currentReport.id_reporte,
                        ReporterUsername = currentReport.usuario1.nombre_usuario,
                        ReportedAccountId = (int)currentReport.id_usuario_reportado,
                        ReportedUsername = currentReport.usuario2.nombre_usuario,
                        Reason = currentReport.motivo,
                        SubmittedDate = currentReport.fecha_reporte,
                        MatchId = (int?)currentReport.id_partida
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
                suspendedAccounts = context.sancion
                    .Where(currentSanction => currentSanction.tipo == SanctionType.Suspension && currentSanction.fecha_revocacion == null && currentSanction.fecha_fin > now)
                    .Select(currentSanction => new SuspendedAccountSummary
                    {
                        AccountId = currentSanction.id_usuario_sancionado,
                        Username = currentSanction.usuario.nombre_usuario,
                        SuspensionReason = currentSanction.motivo,
                        SuspensionEndDate = currentSanction.fecha_fin.Value
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
                sancion activeSanction = FindActiveSuspension(context, accountId);
                sanctionStatus = new AccountSanctionStatus();
                if (activeSanction == null)
                {
                    sanctionStatus.IsCurrentlySuspended = false;
                }
                else
                {
                    sanctionStatus.IsCurrentlySuspended = true;
                    sanctionStatus.SuspensionEndDate = activeSanction.fecha_fin;
                    sanctionStatus.SuspensionReason = activeSanction.motivo;
                }
            }
            return sanctionStatus;
        }

        public void ApplyNewSuspension(int accountId, int? reportId, string reason, DateTime suspensionEndDate, int supportAccountId)
        {
            using (CribbageEntities context = new CribbageEntities())
            {
                sancion newSanction = new sancion();
                newSanction.id_usuario_sancionado = accountId;
                newSanction.id_usuario_soporte = supportAccountId;
                newSanction.id_reporte = reportId;
                newSanction.tipo = SanctionType.Suspension;
                newSanction.motivo = reason;
                newSanction.fecha_inicio = DateTime.Now;
                newSanction.fecha_fin = suspensionEndDate;
                context.sancion.Add(newSanction);

                usuario sanctionedUser = context.usuario.First(currentUser => currentUser.id_usuario == accountId);
                sanctionedUser.estado_cuenta = AccountStatus.Suspended;

                RemoveActiveSessions(context, accountId);
                AddSystemLogEvent(context, accountId, "SUSPENSION_APLICADA", reason);
                context.SaveChanges();
            }
        }

        public void UpdateSuspensionDuration(int accountId, string reason, DateTime suspensionEndDate, int supportAccountId)
        {
            using (CribbageEntities context = new CribbageEntities())
            {
                sancion activeSanction = FindActiveSuspension(context, accountId);
                activeSanction.motivo = reason;
                activeSanction.fecha_fin = suspensionEndDate;
                activeSanction.id_usuario_soporte = supportAccountId;
                AddSystemLogEvent(context, accountId, "SUSPENSION_MODIFICADA", reason);
                context.SaveChanges();
            }
        }

        public void ReactivateAccount(int accountId, int supportAccountId)
        {
            using (CribbageEntities context = new CribbageEntities())
            {
                sancion activeSanction = FindActiveSuspension(context, accountId);
                activeSanction.fecha_revocacion = DateTime.Now;
                activeSanction.id_usuario_soporte = supportAccountId;

                usuario reactivatedUser = context.usuario.First(currentUser => currentUser.id_usuario == accountId);
                reactivatedUser.estado_cuenta = AccountStatus.Verified;

                AddSystemLogEvent(context, accountId, "CUENTA_REACTIVADA", string.Empty);
                context.SaveChanges();
            }
        }

        private sancion FindActiveSuspension(CribbageEntities context, int accountId)
        {
            DateTime now = DateTime.Now;
            sancion activeSanction = context.sancion.FirstOrDefault(currentSanction => currentSanction.id_usuario_sancionado == accountId && currentSanction.tipo == SanctionType.Suspension && currentSanction.fecha_revocacion == null && currentSanction.fecha_fin > now);
            return activeSanction;
        }

        private void RemoveActiveSessions(CribbageEntities context, int accountId)
        {
            List<sesion> activeSessions = context.sesion.Where(currentSession => currentSession.id_usuario == accountId).ToList();
            foreach (sesion currentSession in activeSessions)
            {
                context.sesion.Remove(currentSession);
            }
        }

        private void AddSystemLogEvent(CribbageEntities context, int accountId, string eventType, string description)
        {
            bitacora_evento newEvent = new bitacora_evento();
            newEvent.id_usuario = accountId;
            newEvent.tipo_evento = eventType;
            newEvent.descripcion = description;
            newEvent.fecha_evento = DateTime.Now;
            context.bitacora_evento.Add(newEvent);
        }
    }
}