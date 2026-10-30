using System;
using cribbage_Royale_DataAccess.DataAccess;

namespace CribbageRoyale.Server.Sessions
{
    public class Authenticator : IAuthenticator
    {
        private readonly IUserRepository _userRepository;

        public Authenticator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool TryVerifyCredentials(string userNameOrEmail, string password, out UserCredentials credentials)
        {
            UserCredentials storedCredentials = _userRepository.FindCredentials(userNameOrEmail);
            bool verified = false;

            if (storedCredentials != null && !IsLocked(storedCredentials))
            {
                verified = VerifyPassword(storedCredentials, password);
            }

            credentials = null;

            if (verified)
            {
                credentials = storedCredentials;
            }

            return verified;
        }

        private bool IsLocked(UserCredentials credentials)
        {
            bool locked = false;

            if (credentials.LockoutEndDate.HasValue && credentials.LockoutEndDate.Value > DateTime.Now)
            {
                locked = true;
            }

            return locked;
        }

        private bool VerifyPassword(UserCredentials credentials, string password)
        {
            bool verified = IsPasswordCorrect(password, credentials.PasswordHash);

            if (verified)
            {
                _userRepository.RegisterSuccessfulAccess(credentials.AccountId);
            }
            else
            {
                _userRepository.RegisterFailedAttempt(credentials.AccountId);
            }

            return verified;
        }

        private bool IsPasswordCorrect(string password, string storedHash)
        {
            bool isCorrect = false;

            if (!string.IsNullOrWhiteSpace(storedHash))
            {
                isCorrect = BCrypt.Net.BCrypt.Verify(password, storedHash);
            }

            return isCorrect;
        }
    }
}