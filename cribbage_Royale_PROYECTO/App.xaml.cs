using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using cribbage_Royale_PROYECTO.Localization;
using cribbage_Royale_PROYECTO.Logging;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.ServiceAccess;
using cribbage_Royale_PROYECTO.Views.Dialogs;
using Microsoft.Extensions.Logging;

namespace cribbage_Royale_PROYECTO
{
    public partial class App : Application
    {
        private const string ApplicationStartedMessage = "The application started.";

        private const string UnhandledInterfaceExceptionMessage =
            "An unhandled exception occurred in the user interface.";

        private const string UnobservedTaskExceptionMessage =
            "A task failed and its exception was never observed.";

        private const string UnhandledBackgroundExceptionMessage =
            "An unhandled exception occurred outside the user interface thread.";

        private ILogger _logger;
        private GameClientCallback _sessionCallback;
        private PlayerSessionClient _sessionClient;
        private FriendListClient _friendListClient;
        private FriendRequestsClient _friendRequestsClient;

        public GameClientCallback SessionCallback
        {
            get { return _sessionCallback; }
        }

        public PlayerSessionClient SessionClient
        {
            get { return _sessionClient; }
        }

        public FriendListClient FriendListClient
        {
            get { return _friendListClient; }
        }

        public FriendRequestsClient FriendRequestsClient
        {
            get { return _friendRequestsClient; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _logger = ApplicationLogging.CreateLogger<App>();
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            LanguageManager.ApplyCulture(LanguageManager.DefaultCulture);
            CreateSessionClient();
            CreateFriendListClient();
            CreateFriendRequestsClient();
            base.OnStartup(e);
            _logger.LogInformation(ApplicationStartedMessage);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ApplicationLogging.Shutdown();
            base.OnExit(e);
        }

        private void CreateSessionClient()
        {
            _sessionCallback = new GameClientCallback(Dispatcher);
            _sessionClient = new PlayerSessionClient(
                _sessionCallback,
                ApplicationLogging.CreateLogger<PlayerSessionClient>());
        }

        private void CreateFriendListClient()
        {
            _friendListClient = new FriendListClient(ApplicationLogging.CreateLogger<FriendListClient>());
        }

        private void CreateFriendRequestsClient()
        {
            _friendRequestsClient = new FriendRequestsClient(ApplicationLogging.CreateLogger<FriendRequestsClient>());
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.LogCritical(e.Exception, UnhandledInterfaceExceptionMessage);
            MessageDialog.ShowError(MainWindow, Lang.Application_UnexpectedErrorMessage);
            e.Handled = true;
            Shutdown();
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            _logger.LogError(e.Exception, UnobservedTaskExceptionMessage);
            e.SetObserved();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception;
            _logger.LogCritical(exception, UnhandledBackgroundExceptionMessage);
            ApplicationLogging.Shutdown();
        }
    }
}