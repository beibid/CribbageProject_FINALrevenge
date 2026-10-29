using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.ServiceModel;
using cribbage_Royale_DataAccess;
using cribbage_Royale_DataAccess.DataAccess;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using CribbageRoyale.Contracts.DataContracts;
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

        public List<ScoreboardEntry> GetScoreboard()
        {
            try
            {
                List<vista_estadisticas_jugador> ranking = _scoreboardRepository.GetRanking();

                return _scoreboardProjection.BuildScoreboard(ranking);
            }
            catch (EntityException entityException)
            {
                throw BuildDataAccessFault(entityException);
            }
            catch (SqlException sqlException)
            {
                throw BuildDataAccessFault(sqlException);
            }
        }

        private FaultException<ServiceFault> BuildDataAccessFault(Exception exception)
        {
            _logger.LogError(exception, ScoreboardQueryFailureMessage);

            return ServiceFaultBuilder.Build(ServiceFaultCode.DataAccess, ScoreboardQueryFailureMessage);
        }
    }
}