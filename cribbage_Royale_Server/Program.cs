using System;
using System.ServiceModel;
using cribbage_Royale_DataAccess.DataAccess;
using cribbage_Royale_Server.Hosting;
using CribbageRoyale.Server.Connections;
using CribbageRoyale.Server.Logging;
using CribbageRoyale.Server.Matches;
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
        private static FriendRequestsServices _friendRequestsServices;
        private static FriendChatServices _friendChatServices;
        private static MatchInvitationServices _matchInvitationServices;
        private static MatchPlayServices _matchPlayServices;
        private static MatchInvitationRegistry _invitationRegistry;
        private static MatchRoomRegistry _matchRooms;

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
            ServiceHost friendRequestsHost = CreateFriendRequestsHost();
            ServiceHost friendChatHost = CreateFriendChatHost();
            ServiceHost matchInvitationHost = CreateMatchInvitationHost();
            ServiceHost matchPlayHost = CreateMatchPlayHost();

            WatchDisconnections(_sessionServices);

            try
            {
                statisticsHost.Open();
                sessionHost.Open();
                friendListHost.Open();
                friendRequestsHost.Open();
                friendChatHost.Open();
                matchInvitationHost.Open();
                matchPlayHost.Open();
                logger.LogInformation(ServiceStartedLogMessage);
                Console.WriteLine(ServiceStartedMessage);
                Console.ReadLine();
                matchPlayHost.Close();
                matchInvitationHost.Close();
                friendChatHost.Close();
                friendRequestsHost.Close();
                friendListHost.Close();
                sessionHost.Close();
                statisticsHost.Close();
                logger.LogInformation(ServiceStoppedLogMessage);
            }
            catch (AddressAlreadyInUseException addressAlreadyInUseException)
            {
                logger.LogCritical(addressAlreadyInUseException, PortInUseMessage);
                matchPlayHost.Abort();
                matchInvitationHost.Abort();
                friendChatHost.Abort();
                friendRequestsHost.Abort();
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

        private static ServiceHost CreateFriendRequestsHost()
        {
            _friendRequestsServices = CreateFriendRequestsServices(_sessionServices);
            ServiceHost host = new ServiceHost(typeof(FriendRequestsService));
            host.Description.Behaviors.Add(new InstanceProviderBehavior(() => CreateFriendRequestsService()));

            return host;
        }

        private static ServiceHost CreateFriendChatHost()
        {
            _friendChatServices = CreateFriendChatServices(_sessionServices);
            ServiceHost host = new ServiceHost(typeof(FriendChatService));
            host.Description.Behaviors.Add(new InstanceProviderBehavior(() => CreateFriendChatService()));

            return host;
        }

        private static ServiceHost CreateMatchInvitationHost()
        {
            _matchInvitationServices = CreateMatchInvitationServices(_sessionServices);
            ServiceHost host = new ServiceHost(typeof(MatchInvitationService));
            host.Description.Behaviors.Add(new InstanceProviderBehavior(() => CreateMatchInvitationService()));

            return host;
        }

        private static ServiceHost CreateMatchPlayHost()
        {
            _matchPlayServices = CreateMatchPlayServices(_sessionServices);
            ServiceHost host = new ServiceHost(typeof(MatchPlayService));
            host.Description.Behaviors.Add(new InstanceProviderBehavior(() => CreateMatchPlayService()));

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
            services.LogInGate = new LogInGate();

            return services;
        }

        private static FriendListServices CreateFriendListServices(SessionServices sessionServices)
        {
            FriendshipRepository friendshipRepository = new FriendshipRepository();

            FriendPresenceNotifier presenceNotifier = new FriendPresenceNotifier(
                friendshipRepository,
                new StateProjection(sessionServices.PlayerChannel),
                ApplicationLogging.CreateLogger<FriendPresenceNotifier>());
            presenceNotifier.Attach(sessionServices.PlayerChannel);

            FriendListServices services = new FriendListServices();
            services.CallerSession = new CallerSession(sessionServices.SessionTokens);
            services.FriendshipRepository = friendshipRepository;
            services.PlayerChannel = sessionServices.PlayerChannel;

            return services;
        }

        private static FriendRequestsServices CreateFriendRequestsServices(SessionServices sessionServices)
        {
            FriendRequestsServices services = new FriendRequestsServices();
            services.CallerSession = new CallerSession(sessionServices.SessionTokens);
            services.FriendshipRepository = new FriendshipRepository();
            services.PlayerChannel = sessionServices.PlayerChannel;
            services.StateProjection = new StateProjection(sessionServices.PlayerChannel);

            return services;
        }

        private static FriendChatServices CreateFriendChatServices(SessionServices sessionServices)
        {
            FriendChatServices services = new FriendChatServices();
            services.CallerSession = new CallerSession(sessionServices.SessionTokens);
            services.FriendshipRepository = new FriendshipRepository();
            services.PlayerChannel = sessionServices.PlayerChannel;
            services.StateProjection = new StateProjection(sessionServices.PlayerChannel);

            return services;
        }

        private static MatchInvitationServices CreateMatchInvitationServices(SessionServices sessionServices)
        {
            _invitationRegistry = new MatchInvitationRegistry();
            _matchRooms = new MatchRoomRegistry();

            MatchInvitationServices services = new MatchInvitationServices();
            services.CallerSession = new CallerSession(sessionServices.SessionTokens);
            services.FriendshipRepository = new FriendshipRepository();
            services.PlayerChannel = sessionServices.PlayerChannel;
            services.StateProjection = new StateProjection(sessionServices.PlayerChannel);
            services.InvitationRegistry = _invitationRegistry;
            services.MatchRooms = _matchRooms;

            return services;
        }

        private static MatchPlayServices CreateMatchPlayServices(SessionServices sessionServices)
        {
            MatchPlayServices services = new MatchPlayServices();
            services.CallerSession = new CallerSession(sessionServices.SessionTokens);
            services.PlayerChannel = sessionServices.PlayerChannel;
            services.StateProjection = new StateProjection(sessionServices.PlayerRegistry);
            services.MatchRooms = _matchRooms;

            return services;
        }

        private static void WatchDisconnections(SessionServices sessionServices)
        {
            MatchWatcherServices services = new MatchWatcherServices();
            services.PlayerChannel = sessionServices.PlayerChannel;
            services.StateProjection = new StateProjection(sessionServices.PlayerChannel);
            services.Logger = ApplicationLogging.CreateLogger<MatchDisconnectionWatcher>();

            MatchDisconnectionWatcher watcher = new MatchDisconnectionWatcher(
                _invitationRegistry,
                _matchRooms,
                services);

            watcher.Watch();
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

        private static FriendRequestsService CreateFriendRequestsService()
        {
            return new FriendRequestsService(
                _friendRequestsServices,
                ApplicationLogging.CreateLogger<FriendRequestsService>());
        }

        private static FriendChatService CreateFriendChatService()
        {
            return new FriendChatService(
                _friendChatServices,
                ApplicationLogging.CreateLogger<FriendChatService>());
        }

        private static MatchInvitationService CreateMatchInvitationService()
        {
            return new MatchInvitationService(
                _matchInvitationServices,
                ApplicationLogging.CreateLogger<MatchInvitationService>());
        }

        private static MatchPlayService CreateMatchPlayService()
        {
            return new MatchPlayService(
                _matchPlayServices,
                ApplicationLogging.CreateLogger<MatchPlayService>());
        }
    }
}