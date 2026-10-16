using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using CribbageRoyale.Contracts.Services;
using CribbageRoyale.Contracts.Transfer;

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

                return await channel.GetScoreboardAsync();
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