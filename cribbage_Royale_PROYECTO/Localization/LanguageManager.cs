using System.Globalization;
using System.Threading;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Localization
{
    public static class LanguageManager
    {
        public const string SpanishCulture = "es-MX";
        public const string EnglishCulture = "en-US";
        public const string DefaultCulture = SpanishCulture;

        public static string CurrentCultureCode
        {
            get { return Thread.CurrentThread.CurrentUICulture.Name; }
        }

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