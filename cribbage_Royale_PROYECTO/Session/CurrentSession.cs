using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_DataAccess.Models;

namespace cribbage_Royale_PROYECTO.Session
{
    public static class CurrentSession
    {

        public enum SessionKind
        {
            None,
            Registered,
            Guest,
            Support
        }
        public static SessionKind Kind { get; private set; }

        public static string Username { get; private set; }

        public static string Email { get; private set; }

        public static string SessionToken { get; private set; }

        public static int SupportAccountId { get; private set; }

        public static string SupportUsername { get; private set; }

        public static void StartRegisteredSession(string username, string email, string sessionToken)
        {
            Kind = SessionKind.Registered;
            Username = username;
            Email = email;
            SessionToken = sessionToken;
        }

        public static void StartGuestSession(string displayName, string sessionToken)
        {
            Kind = SessionKind.Guest;
            Username = displayName;
            Email = string.Empty;
            SessionToken = sessionToken;
        }

        public static void StartSupportSession(int supportAccountId, string supportUsername)
        {
            Kind = SessionKind.Support;
            SupportAccountId = supportAccountId;
            SupportUsername = supportUsername;
        }

        public static void EndSession()
        {
            Kind = SessionKind.None;
            Username = string.Empty;
            Email = string.Empty;
            SessionToken = string.Empty;
            SupportAccountId = 0;
            SupportUsername = string.Empty;
        }

        public static bool IsGuest()
        {
            return Kind == SessionKind.Guest;
        }
    }
}