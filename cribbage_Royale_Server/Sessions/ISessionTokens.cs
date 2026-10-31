namespace CribbageRoyale.Server.Sessions
{
    public interface ISessionTokens
    {
        string IssueToken(int accountId);

        string IssueGuestToken(string displayName);

        bool TryValidateToken(string token, out SessionIdentity identity);

        void RevokeToken(string token);

        void RevokeAllTokens(int accountId);
    }
}