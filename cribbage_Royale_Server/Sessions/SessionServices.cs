using CribbageRoyale.Server.Connections;

namespace CribbageRoyale.Server.Sessions
{
    public class SessionServices
    {
        public IAuthenticator Authenticator { get; set; }

        public ISessionTokens SessionTokens { get; set; }

        public IPlayerChannel PlayerChannel { get; set; }

        public PlayerRegistry PlayerRegistry { get; set; }

        public GuestAccountIds GuestAccountIds { get; set; }

        public LogInGate LogInGate { get; set; }
    }
}