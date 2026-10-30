namespace CribbageRoyale.Server.Sessions
{
    public interface ISessionTokens
    {
        string IssueToken(int accountId);

        void RevokeToken(string token);

        void RevokeAllTokens(int accountId);

        bool TryGetAccountId(string token, out int accountId);
    }
}