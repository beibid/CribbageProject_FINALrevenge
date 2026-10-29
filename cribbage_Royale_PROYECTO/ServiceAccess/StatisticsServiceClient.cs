using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class StatisticsServiceClient
    {
        private const string EndpointName = "StatisticsEndpoint";

        private readonly ILogger _logger;

        public StatisticsServiceClient(ILogger logger)
        {
            _logger = logger;
        }

        public Task<ScoreboardResult> GetScoreboardAsync()
        {
            return Task.Run(() => RequestScoreboard());
        }

        private ScoreboardResult RequestScoreboard()
        {
            ChannelFactory<IStatistics> factory = new ChannelFactory<IStatistics>(EndpointName);
            ScoreboardResult scoreboardResult;

            try
            {
                scoreboardResult = CallScoreboard(factory);
            }
            finally
            {
                CloseFactory(factory);
            }

            return scoreboardResult;
        }

        private ScoreboardResult CallScoreboard(ChannelFactory<IStatistics> factory)
        {
            ScoreboardResult scoreboardResult;

            try
            {
                IStatistics channel = factory.CreateChannel();
                List<ScoreboardEntry> entries = channel.GetScoreboard();
                scoreboardResult = new ScoreboardResult(true, ServiceFailureReason.None, entries);
            }
            catch (FaultException<ServiceFault> faultException)
            {
                scoreboardResult = BuildFailure(ServiceFailureReason.ServerError, faultException);
            }
            catch (EndpointNotFoundException endpointNotFoundException)
            {
                scoreboardResult = BuildFailure(ServiceFailureReason.ServerUnavailable, endpointNotFoundException);
            }
            catch (TimeoutException timeoutException)
            {
                scoreboardResult = BuildFailure(ServiceFailureReason.Timeout, timeoutException);
            }
            catch (CommunicationException communicationException)
            {
                scoreboardResult = BuildFailure(ServiceFailureReason.ServerUnavailable, communicationException);
            }

            return scoreboardResult;
        }

        private ScoreboardResult BuildFailure(ServiceFailureReason reason, Exception exception)
        {
            _logger.LogError(exception, "The scoreboard could not be retrieved. Reason: {Reason}.", reason);
            return new ScoreboardResult(false, reason, new List<ScoreboardEntry>());
        }

        private void CloseFactory(ChannelFactory<IStatistics> factory)
        {
            if (factory.State == CommunicationState.Faulted)
            {
                factory.Abort();
            }
            else
            {
                try
                {
                    factory.Close();
                }
                catch (CommunicationException communicationException)
                {
                    _logger.LogWarning(communicationException, "The statistics channel could not be closed.");
                    factory.Abort();
                }
                catch (TimeoutException timeoutException)
                {
                    _logger.LogWarning(timeoutException, "The statistics channel timed out while closing.");
                    factory.Abort();
                }
            }
        }
    }
}