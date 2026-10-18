using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using CribbageRoyale.Contracts.ServiceContracts;
using CribbageRoyale.Contracts.DataContracts;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class StatisticsServiceClient
    {
        private const string EndpointConfigurationName = "StatisticsEndpoint";

        public async Task<List<ScoreboardEntry>> GetScoreboardAsync()
        {
            ChannelFactory<IStatistics> factory =
                new ChannelFactory<IStatistics>(EndpointConfigurationName);

            try
            {
                IStatistics channel = factory.CreateChannel();

                return await Task.Run(() => channel.GetScoreboard());
            }
            finally
            {
                CloseFactory(factory);
            }
        }

        private void CloseFactory(ChannelFactory<IStatistics> factory)
        {
            if (factory.State == CommunicationState.Faulted)
            {
                factory.Abort();
            }
            else
            {
                factory.Close();
            }
        }
    }
}