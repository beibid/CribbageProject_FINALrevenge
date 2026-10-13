using cribbage_Royale_PROYECTO.Localization;
using cribbage_Royale_PROYECTO.Logging;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace cribbage_Royale_PROYECTO
{
    public partial class App : Application
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<App>();

        protected override void OnStartup(StartupEventArgs e)
        {
            LanguageManager.ApplyCulture(LanguageManager.DefaultCulture);
            base.OnStartup(e);
            Logger.LogInformation("La aplicación inició");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ApplicationLogging.Shutdown();
            base.OnExit(e);
        }
    }
}