using System;
using System.ServiceModel;
using CribbageRoyale.Server.Logging;
using CribbageRoyale.Server.Services;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server
{
    public class Program
    {
        private const string ServiceStartedMessage =
            "Server started. Press Enter to stop it.";

        private const string PortInUseMessage =
            "The port is already in use. Close the other instance of the server and try again.";

        private const string ServiceStartedLogMessage =
            "The Server started";

        private const string ServiceStoppedLogMessage =
            "The Server stopped";

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

            try
            {
                using (ServiceHost host = new ServiceHost(typeof(StatisticsService)))
                {
                    host.Open();
                    logger.LogInformation(ServiceStartedLogMessage);
                    Console.WriteLine(ServiceStartedMessage);
                    Console.ReadLine();
                    host.Close();
                    logger.LogInformation(ServiceStoppedLogMessage);
                }
            }
            catch (AddressAlreadyInUseException exceptionLogger)
            {
                logger.LogError(exceptionLogger, PortInUseMessage);
                Console.WriteLine(PortInUseMessage);
                Console.ReadLine();
            }
        }
    }
}