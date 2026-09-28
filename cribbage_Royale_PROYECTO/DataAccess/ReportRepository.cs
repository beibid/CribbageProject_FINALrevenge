using System;
using System.Linq;

namespace cribbage_Royale_PROYECTO.DataAccess
{
    public class ReportRepository
    {
        private const int FolioLength = 12;

        public string FindExistingReportFolio(int reporterAccountId, int reportedAccountId, int? matchId)
        {
            string existingFolio;
            using (CribbageEntities context = new CribbageEntities())
            {
                reporte existingReport = context.reporte.FirstOrDefault(currentReport => currentReport.id_usuario_reportante == reporterAccountId && currentReport.id_usuario_reportado == reportedAccountId && currentReport.id_partida == matchId);
                if (existingReport == null)
                {
                    existingFolio = null;
                }
                else
                {
                    existingFolio = existingReport.folio;
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
                reporte newReport = new reporte();
                newReport.folio = generatedFolio;
                newReport.id_usuario_reportante = reporterAccountId;
                newReport.id_usuario_reportado = reportedAccountId;
                newReport.id_partida = matchId;
                newReport.motivo = reason;
                newReport.comentario = comment;
                newReport.fecha_reporte = DateTime.Now;
                newReport.estado = ReportStatus.Pending;
                context.reporte.Add(newReport);
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