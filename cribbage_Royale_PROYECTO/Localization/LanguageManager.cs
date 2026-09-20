using System.Globalization;
using System.Threading;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Localization
{
    // Centralizes how the game changes and reads its current language
    public static class LanguageManager
    {
        public const string SpanishCulture = "es-MX";
        public const string EnglishCulture = "en-US";
        public const string DefaultCulture = SpanishCulture;

        // Culture code currently in use (for example "es-MX")
        public static string CurrentCultureCode
        {
            get { return Thread.CurrentThread.CurrentUICulture.Name; }
        }

        // Changes the language of texts (UICulture) and of formats
        // such as dates and numbers (Culture)
        public static void ApplyCulture(string cultureCode)
        {
            CultureInfo culture = new CultureInfo(cultureCode);

            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;

            Lang.Culture = culture;
        }
    }
}