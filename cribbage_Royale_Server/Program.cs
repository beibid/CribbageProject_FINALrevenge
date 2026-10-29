using cribbage_Royale_DataAccess.DataAccess;
using cribbage_Royale_Server.Hosting;
using CribbageRoyale.Server.Logging;
using CribbageRoyale.Server.Projection;
using CribbageRoyale.Server.Services;
using Microsoft.Extensions.Logging;
using System;
using System.ServiceModel;

namespace CribbageRoyale.Server
{
    public class Program
    {
        private const string ServiceStartedMessage = "Server started. Press Enter to stop it.";

        private const string PortInUseMessage =
            "The port is already in use. Close the other instance of the server and try again.";

        private const string ServiceStartedLogMessage = "The server started.";

        private const string ServiceStoppedLogMessage = "The server stopped.";

        private Program()
        {
        }

        public static void Main()
        {
            ApplicationLogging.Initialize();
            StartService();
            ApplicationLogging.Shutdown();
        }

        private static void StartService()
        {
            ILogger logger = ApplicationLogging.CreateLogger<Program>();
            ServiceHost host = CreateStatisticsHost();

            try
            {
                host.Open();
                logger.LogInformation(ServiceStartedLogMessage);
                Console.WriteLine(ServiceStartedMessage);
                Console.ReadLine();
                host.Close();
                logger.LogInformation(ServiceStoppedLogMessage);
            }
            catch (AddressAlreadyInUseException addressAlreadyInUseException)
            {
                logger.LogCritical(addressAlreadyInUseException, PortInUseMessage);
                host.Abort();
                Console.WriteLine(PortInUseMessage);
                Console.ReadLine();
            }
        }

        private static ServiceHost CreateStatisticsHost()
        {
            ServiceHost host = new ServiceHost(typeof(StatisticsService));
            host.Description.Behaviors.Add(new InstanceProviderBehavior(() => CreateStatisticsService()));
            return host;
        }

        private static StatisticsService CreateStatisticsService()
        {
            return new StatisticsService(
                new ScoreboardRepository(),
                new ScoreboardProjection(),
                ApplicationLogging.CreateLogger<StatisticsService>());
        }
    }
}