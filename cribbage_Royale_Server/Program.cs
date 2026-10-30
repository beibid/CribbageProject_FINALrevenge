using System;
using System.ServiceModel;
using cribbage_Royale_DataAccess.DataAccess;
using cribbage_Royale_Server.Hosting;
using CribbageRoyale.Server.Connections;
using CribbageRoyale.Server.Logging;
using CribbageRoyale.Server.Projection;
using CribbageRoyale.Server.Services;
using CribbageRoyale.Server.Sessions;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server
{
    public class Program
    {
        private const string ServiceStartedMessage = "Server started. Press Enter to stop it.";

        private const string PortInUseMessage =
            "The port is already in use. Close the other instance of the server and try again.";

        private const string ServiceStartedLogMessage = "The server started.";

        private const string ServiceStoppedLogMessage = "The server stopped.";

        private const int SessionLifetimeHours = 10;

        private static SessionServices _sessionServices;
        private static FriendListServices _friendListServices;

        private Program()
        {
        }

        public static void Main()
        {
            ApplicationLogging.Initialize();
            StartServices();
            ApplicationLogging.Shutdown();
        }

        private static void StartServices()
        {
            ILogger logger = ApplicationLogging.CreateLogger<Program>();
            ServiceHost statisticsHost = CreateStatisticsHost();
            ServiceHost sessionHost = CreateSessionHost();
            ServiceHost friendListHost = CreateFriendListHost();

            try
            {
                statisticsHost.Open();
                sessionHost.Open();
                friendListHost.Open();
                logger.LogInformation(ServiceStartedLogMessage);
                Console.WriteLine(ServiceStartedMessage);
                Console.ReadLine();
                friendListHost.Close();
                sessionHost.Close();
                statisticsHost.Close();
                logger.LogInformation(ServiceStoppedLogMessage);
            }
            catch (AddressAlreadyInUseException addressAlreadyInUseException)
            {
                logger.LogCritical(addressAlreadyInUseException, PortInUseMessage);
                friendListHost.Abort();
                sessionHost.Abort();
                statisticsHost.Abort();
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

        private static ServiceHost CreateSessionHost()
        {
            _sessionServices = CreateSessionServices();
            ServiceHost host = new ServiceHost(typeof(PlayerSessionService));
            host.Description.Behaviors.Add(new InstanceProviderBehavior(() => CreateSessionService()));

            return host;
        }

        private static ServiceHost CreateFriendListHost()
        {
            _friendListServices = CreateFriendListServices(_sessionServices);
            ServiceHost host = new ServiceHost(typeof(FriendListService));
            host.Description.Behaviors.Add(new InstanceProviderBehavior(() => CreateFriendListService()));

            return host;
        }

        private static SessionServices CreateSessionServices()
        {
            SessionLifetime sessionLifetime = new SessionLifetime(TimeSpan.FromHours(SessionLifetimeHours));

            SessionServices services = new SessionServices();
            services.Authenticator = new Authenticator(new UserRepository());
            services.SessionTokens = new SessionTokenStore(new GuestAccountIds(), sessionLifetime);
            services.SessionLifetime = sessionLifetime;
            services.PlayerChannel = new PlayerChannel(ApplicationLogging.CreateLogger<PlayerChannel>());
            services.PlayerRegistry = new PlayerRegistry();
            services.LogInGate = new LogInGate();

            return services;
        }

        private static FriendListServices CreateFriendListServices(SessionServices sessionServices)
        {
            FriendshipRepository friendshipRepository = new FriendshipRepository();

            FriendPresenceNotifier presenceNotifier = new FriendPresenceNotifier(
                friendshipRepository,
                new StateProjection(sessionServices.PlayerRegistry),
                ApplicationLogging.CreateLogger<FriendPresenceNotifier>());
            presenceNotifier.Attach(sessionServices.PlayerChannel);

            FriendListServices services = new FriendListServices();
            services.CallerSession = new CallerSession(sessionServices.SessionTokens);
            services.FriendshipRepository = friendshipRepository;
            services.PlayerChannel = sessionServices.PlayerChannel;

            return services;
        }

        private static StatisticsService CreateStatisticsService()
        {
            return new StatisticsService(
                new ScoreboardRepository(),
                new ScoreboardProjection(),
                ApplicationLogging.CreateLogger<StatisticsService>());
        }

        private static PlayerSessionService CreateSessionService()
        {
            return new PlayerSessionService(
                _sessionServices,
                ApplicationLogging.CreateLogger<PlayerSessionService>());
        }

        private static FriendListService CreateFriendListService()
        {
            return new FriendListService(
                _friendListServices,
                ApplicationLogging.CreateLogger<FriendListService>());
        }
    }
}