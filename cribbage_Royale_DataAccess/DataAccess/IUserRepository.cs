namespace cribbage_Royale_DataAccess.DataAccess
{
    public interface IUserRepository
    {
        UserCredentials FindCredentials(string userNameOrEmail);

        void RegisterFailedAttempt(int accountId);

        void RegisterSuccessfulAccess(int accountId);

        ProfileRecord GetOwnProfile(int accountId);

        void UpdateDisplayName(int accountId, string displayName);

        bool ChangePassword(int accountId, string currentPassword, string newPassword);

        void UpdateAvatar(int accountId, byte[] image);
    }
}