using System.Windows;
using cribbage_Royale_PROYECTO.Localization;

namespace cribbage_Royale_PROYECTO
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // The game starts in Spanish, the default language in the database
            LanguageManager.ApplyCulture(LanguageManager.DefaultCulture);
            base.OnStartup(e);
        }
    }
}