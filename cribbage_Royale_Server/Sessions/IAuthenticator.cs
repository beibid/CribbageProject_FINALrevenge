using cribbage_Royale_DataAccess.DataAccess;

namespace CribbageRoyale.Server.Sessions
{
    public interface IAuthenticator
    {
        bool TryVerifyCredentials(string userNameOrEmail, string password, out UserCredentials credentials);
    }
}