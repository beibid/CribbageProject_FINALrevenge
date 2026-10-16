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
            "Servicio iniciado. Presiona Enter para detenerlo.";

        private const string PortInUseMessage =
            "El puerto ya esta en uso. Cierra la otra instancia del servidor y vuelve a intentarlo.";

        private const string ServiceStartedLogMessage =
            "El servidor quedo escuchando";

        private const string ServiceStoppedLogMessage =
            "El servidor se detuvo";

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