using cribbage_Royale_PROYECTO.Models;

namespace cribbage_Royale_PROYECTO.Session
{
    public static class CurrentSession
    {
        public static SessionKind Kind { get; private set; }

        public static string Username { get; private set; }

        public static string Email { get; private set; }

        public static void StartRegisteredSession(string username, string email)
        {
            Kind = SessionKind.Registered;
            Username = username;
            Email = email;
        }

        public static void StartGuestSession(string displayName)
        {
            Kind = SessionKind.Guest;
            Username = displayName;
            Email = string.Empty;
        }

        public static void EndSession()
        {
            Kind = SessionKind.None;
            Username = string.Empty;
            Email = string.Empty;
        }

        public static bool IsGuest()
        {
            return Kind == SessionKind.Guest;
        }
    }
}