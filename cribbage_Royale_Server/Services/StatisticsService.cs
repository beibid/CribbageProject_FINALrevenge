using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.ServiceModel;
using System.Threading.Tasks;
using cribbage_Royale_DataAccess;
using cribbage_Royale_DataAccess.DataAccess;
using CribbageRoyale.Contracts.Faults;
using CribbageRoyale.Contracts.Services;
using CribbageRoyale.Contracts.Transfer;
using CribbageRoyale.Server.Faults;
using CribbageRoyale.Server.Logging;
using CribbageRoyale.Server.Projection;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class StatisticsService : IStatistics
    {
        private const string ScoreboardQueryFailureMessage =
            "No se pudo consultar la vista de estadisticas del jugador";

        private readonly ScoreboardRepository _scoreboardRepository;
        private readonly ScoreboardProjection _scoreboardProjection;
        private readonly ILogger _logger;

        public StatisticsService()
        {
            _scoreboardRepository = new ScoreboardRepository();
            _scoreboardProjection = new ScoreboardProjection();
            _logger = ApplicationLogging.CreateLogger<StatisticsService>();
        }

        public async Task<List<ScoreboardEntry>> GetScoreboardAsync()
        {
            try
            {
                List<vista_estadisticas_jugador> ranking =
                    await _scoreboardRepository.GetRankingAsync();

                return _scoreboardProjection.BuildScoreboard(ranking);
            }
            catch (EntityException exception)
            {
                throw BuildDataAccessFault(exception);
            }
            catch (SqlException exception)
            {
                throw BuildDataAccessFault(exception);
            }
        }

        private FaultException<ServiceFault> BuildDataAccessFault(Exception exception)
        {
            _logger.LogError(exception, ScoreboardQueryFailureMessage);

            return ServiceFaultBuilder.Build(ServiceFaultCode.DataAccess, exception.Message);
        }
    }
}