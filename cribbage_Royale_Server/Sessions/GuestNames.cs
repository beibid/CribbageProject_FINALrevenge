using System.Text.RegularExpressions;

namespace CribbageRoyale.Server.Sessions
{
    public static class GuestNames
    {
        public const string ReservedPrefix = "Guest-";

        private const string AllowedPattern = "^[A-Za-z0-9]{3,16}$";

        public static bool IsValid(string displayName)
        {
            bool valid = false;

            if (!string.IsNullOrWhiteSpace(displayName))
            {
                valid = Regex.IsMatch(displayName, AllowedPattern);
            }

            return valid;
        }

        public static string Build(string displayName)
        {
            return ReservedPrefix + displayName;
        }
    }
}