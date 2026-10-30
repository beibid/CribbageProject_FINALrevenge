namespace cribbage_Royale_DataAccess.DataAccess
{
    public interface IUserRepository
    {
        UserCredentials FindCredentials(string userNameOrEmail);

        void RegisterFailedAttempt(int accountId);

        void RegisterSuccessfulAccess(int accountId);
    }
}