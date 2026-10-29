using System.Collections.Generic;
using System.ServiceModel;
using cribbage_Royale_DataAccess;
using cribbage_Royale_DataAccess.DataAccess;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using CribbageRoyale.Exceptions;
using CribbageRoyale.Server.Faults;
using CribbageRoyale.Server.Projection;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class StatisticsService : IStatistics
    {
        private const string ScoreboardQueryFailureMessage = "The player statistics view could not be queried.";

        private readonly IScoreboardRepository _scoreboardRepository;
        private readonly ScoreboardProjection _scoreboardProjection;
        private readonly ILogger _logger;

        public StatisticsService(
            IScoreboardRepository scoreboardRepository,
            ScoreboardProjection scoreboardProjection,
            ILogger logger)
        {
            _scoreboardRepository = scoreboardRepository;
            _scoreboardProjection = scoreboardProjection;
            _logger = logger;
        }

        public List<ScoreboardEntry> GetScoreboard()
        {
            List<ScoreboardEntry> scoreboardEntries;

            try
            {
                List<vista_estadisticas_jugador> ranking = _scoreboardRepository.GetRanking();
                scoreboardEntries = _scoreboardProjection.BuildScoreboard(ranking);
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildDataAccessFault(dataAccessException);
            }

            return scoreboardEntries;
        }

        private FaultException<ServiceFault> BuildDataAccessFault(DataAccessException dataAccessException)
        {
            _logger.LogError(dataAccessException, ScoreboardQueryFailureMessage);
            return ServiceFaultBuilder.Build(ServiceFaultCode.DataAccess, ScoreboardQueryFailureMessage);
        }
    }
}