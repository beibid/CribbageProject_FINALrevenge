using System;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;
using System.Linq;
using cribbage_Royale_DataAccess.Models;
using CribbageRoyale.Exceptions;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class UserRepository : IUserRepository
    {
        private const string VerifiedAccountStatus = "VERIFIED";
        private const string SuspendedAccountStatus = "SUSPENDED";
        private const int MaximumFailedAttempts = 5;
        private const int LockoutMinutes = 15;
        private const string PendingVerificationStatus = "PENDING_VERIFICATION";
        private const string DefaultAvatar = "default_avatar";
        private const string DefaultLanguage = "es-MX";
        private const string DefaultColorBlindMode = "DISABLED";
        private const byte DefaultMusicVolume = 70;
        private const byte DefaultEffectsVolume = 80;
        private const short InitialSkillLevel = 1000;
        private const string CredentialsQueryFailureMessage = "The user credentials could not be queried.";
        private const string FailedAttemptUpdateFailureMessage = "The failed attempt could not be registered.";
        private const string SuccessfulAccessUpdateFailureMessage = "The successful access could not be registered.";
        private const string ProfileQueryFailureMessage = "The profile could not be queried.";
        private const string ProfileUpdateFailureMessage = "The profile could not be updated.";
        private const string PasswordUpdateFailureMessage = "The password could not be updated.";

        public AuthenticationResult AuthenticateUser(string usernameOrEmail, string password)
        {
            AuthenticationResult result;

            try
            {
                result = EvaluateCredentials(usernameOrEmail, password);
            }
            catch (EntityException entityException)
            {
                Trace.WriteLine(entityException.Message);
                result = BuildFailedResult(AuthenticationOutcome.DatabaseError);
            }
            catch (SqlException sqlException)
            {
                Trace.WriteLine(sqlException.Message);
                result = BuildFailedResult(AuthenticationOutcome.DatabaseError);
            }

            return result;
        }

        public RegistrationOutcome RegisterAccount(string username, string email, string password)
        {
            RegistrationOutcome outcome;

            try
            {
                outcome = CreateAccount(username, email, password);
            }
            catch (EntityException entityException)
            {
                Trace.WriteLine(entityException.Message);
                outcome = RegistrationOutcome.DatabaseError;
            }
            catch (SqlException sqlException)
            {
                Trace.WriteLine(sqlException.Message);
                outcome = RegistrationOutcome.DatabaseError;
            }

            return outcome;
        }

        public int GetUserId(string email)
        {
            int userId = 0;

            try
            {
                using (CribbageEntities context = new CribbageEntities())
                {
                    account account = context.account.FirstOrDefault(user => user.email == email);

                    if (account != null)
                    {
                        userId = account.account_id;
                    }
                }
            }
            catch (EntityException entityException)
            {
                Trace.WriteLine(entityException.Message);
            }
            catch (SqlException sqlException)
            {
                Trace.WriteLine(sqlException.Message);
            }

            return userId;
        }

        public UserCredentials FindCredentials(string userNameOrEmail)
        {
            UserCredentials credentials;

            try
            {
                using (CribbageEntities context = new CribbageEntities())
                {
                    credentials = BuildCredentials(context, userNameOrEmail);
                }
            }
            catch (EntityException entityException)
            {
                throw new DataAccessException(CredentialsQueryFailureMessage, entityException);
            }
            catch (SqlException sqlException)
            {
                throw new DataAccessException(CredentialsQueryFailureMessage, sqlException);
            }

            return credentials;
        }

        public void RegisterFailedAttempt(int accountId)
        {
            try
            {
                using (CribbageEntities context = new CribbageEntities())
                {
                    account account = context.account.FirstOrDefault(user => user.account_id == accountId);

                    if (account != null)
                    {
                        ApplyFailedAttempt(context, account);
                    }
                }
            }
            catch (EntityException entityException)
            {
                throw new DataAccessException(FailedAttemptUpdateFailureMessage, entityException);
            }
            catch (SqlException sqlException)
            {
                throw new DataAccessException(FailedAttemptUpdateFailureMessage, sqlException);
            }
        }

        public void RegisterSuccessfulAccess(int accountId)
        {
            try
            {
                using (CribbageEntities context = new CribbageEntities())
                {
                    account account = context.account.FirstOrDefault(user => user.account_id == accountId);

                    if (account != null)
                    {
                        ApplySuccessfulAccess(context, account);
                    }
                }
            }
            catch (EntityException entityException)
            {
                throw new DataAccessException(SuccessfulAccessUpdateFailureMessage, entityException);
            }
            catch (SqlException sqlException)
            {
                throw new DataAccessException(SuccessfulAccessUpdateFailureMessage, sqlException);
            }
        }

        public ProfileRecord GetOwnProfile(int accountId)
        {
            ProfileRecord record = new ProfileRecord();

            try
            {
                using (CribbageEntities context = new CribbageEntities())
                {
                    account account = context.account.FirstOrDefault(user => user.account_id == accountId);
                    player_profile profile = context.player_profile.FirstOrDefault(
                        item => item.account_id == accountId);

                    if (account != null && profile != null)
                    {
                        FillProfileRecord(record, account, profile);
                    }
                }
            }
            catch (EntityException entityException)
            {
                throw new DataAccessException(ProfileQueryFailureMessage, entityException);
            }
            catch (SqlException sqlException)
            {
                throw new DataAccessException(ProfileQueryFailureMessage, sqlException);
            }

            return record;
        }

        public void UpdateDisplayName(int accountId, string displayName)
        {
            try
            {
                using (CribbageEntities context = new CribbageEntities())
                {
                    player_profile profile = context.player_profile.FirstOrDefault(
                        item => item.account_id == accountId);

                    if (profile != null)
                    {
                        profile.display_name = displayName;
                        context.SaveChanges();
                    }
                }
            }
            catch (EntityException entityException)
            {
                throw new DataAccessException(ProfileUpdateFailureMessage, entityException);
            }
            catch (SqlException sqlException)
            {
                throw new DataAccessException(ProfileUpdateFailureMessage, sqlException);
            }
            catch (DbUpdateException updateException)
            {
                throw new DataAccessException(ProfileUpdateFailureMessage, updateException);
            }
        }

        public bool ChangePassword(int accountId, string currentPassword, string newPassword)
        {
            bool isChanged = false;

            try
            {
                using (CribbageEntities context = new CribbageEntities())
                {
                    account account = context.account.FirstOrDefault(user => user.account_id == accountId);

                    if (account != null && IsPasswordCorrect(currentPassword, account.password_hash))
                    {
                        account.password_hash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                        context.SaveChanges();
                        isChanged = true;
                    }
                }
            }
            catch (EntityException entityException)
            {
                throw new DataAccessException(PasswordUpdateFailureMessage, entityException);
            }
            catch (SqlException sqlException)
            {
                throw new DataAccessException(PasswordUpdateFailureMessage, sqlException);
            }
            catch (DbUpdateException updateException)
            {
                throw new DataAccessException(PasswordUpdateFailureMessage, updateException);
            }

            return isChanged;
        }

        public void UpdateAvatar(int accountId, byte[] image)
        {
            try
            {
                using (CribbageEntities context = new CribbageEntities())
                {
                    player_profile profile = context.player_profile.FirstOrDefault(
                        item => item.account_id == accountId);

                    if (profile != null)
                    {
                        profile.avatar_image = image;
                        context.SaveChanges();
                    }
                }
            }
            catch (EntityException entityException)
            {
                throw new DataAccessException(ProfileUpdateFailureMessage, entityException);
            }
            catch (SqlException sqlException)
            {
                throw new DataAccessException(ProfileUpdateFailureMessage, sqlException);
            }
            catch (DbUpdateException updateException)
            {
                throw new DataAccessException(ProfileUpdateFailureMessage, updateException);
            }
        }

        private UserCredentials BuildCredentials(CribbageEntities context, string userNameOrEmail)
        {
            UserCredentials credentials = null;

            account account = context.account.FirstOrDefault(
                user => user.username == userNameOrEmail || user.email == userNameOrEmail);

            if (account != null)
            {
                int accountId = account.account_id;
                player_profile profile = context.player_profile.FirstOrDefault(
                    record => record.account_id == accountId);
                player_settings settings = context.player_settings.FirstOrDefault(
                    record => record.account_id == accountId);

                credentials = new UserCredentials
                {
                    AccountId = accountId,
                    DisplayName = ResolveDisplayName(account, profile),
                    Email = account.email,
                    PreferredLocaleCode = ResolveLocaleCode(settings),
                    PasswordHash = account.password_hash,
                    Status = account.account_status,
                    LockoutEndDate = account.lockout_ends_at
                };
            }

            return credentials;
        }

        private string ResolveDisplayName(account account, player_profile profile)
        {
            string displayName = account.username;

            if (profile != null)
            {
                displayName = profile.display_name;
            }

            return displayName;
        }

        private string ResolveLocaleCode(player_settings settings)
        {
            string localeCode = DefaultLanguage;

            if (settings != null)
            {
                localeCode = settings.language;
            }

            return localeCode;
        }

        private void FillProfileRecord(ProfileRecord record, account account, player_profile profile)
        {
            record.UserName = account.username;
            record.Email = account.email;
            record.RegisteredAt = account.registered_at;
            record.DisplayName = profile.display_name;
            record.MatchesPlayed = profile.matches_played;
            record.Wins = profile.wins;
            record.Losses = profile.losses;
            record.CurrentStreak = profile.current_streak;

            if (profile.avatar_image != null)
            {
                record.AvatarImage = profile.avatar_image;
            }
        }
        private AuthenticationResult EvaluateCredentials(string usernameOrEmail, string password)
        {
            AuthenticationResult result;

            using (CribbageEntities context = new CribbageEntities())
            {
                account account = context.account.FirstOrDefault(
                    user => user.username == usernameOrEmail || user.email == usernameOrEmail);

                if (account == null)
                {
                    result = BuildFailedResult(AuthenticationOutcome.UserNotFound);
                }
                else
                {
                    result = EvaluateAccount(context, account, password);
                }
            }

            return result;
        }

        private AuthenticationResult EvaluateAccount(CribbageEntities context, account account, string password)
        {
            AuthenticationResult result;

            if (IsAccountLocked(account))
            {
                result = BuildLockedResult(account);
            }
            else if (!IsPasswordCorrect(password, account.password_hash))
            {
                ApplyFailedAttempt(context, account);
                result = BuildFailedResult(AuthenticationOutcome.InvalidPassword);
            }
            else if (account.account_status == SuspendedAccountStatus)
            {
                result = BuildAccountResult(account, AuthenticationOutcome.AccountSuspended);
            }
            else if (account.account_status != VerifiedAccountStatus)
            {
                result = BuildAccountResult(account, AuthenticationOutcome.AccountNotVerified);
            }
            else
            {
                ApplySuccessfulAccess(context, account);
                result = BuildAccountResult(account, AuthenticationOutcome.Success);
            }

            return result;
        }

        private bool IsAccountLocked(account account)
        {
            bool isLocked = false;

            if (account.lockout_ends_at.HasValue
                && account.lockout_ends_at.Value > DateTime.Now)
            {
                isLocked = true;
            }

            return isLocked;
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

        private void ApplyFailedAttempt(CribbageEntities context, account account)
        {
            account.failed_attempts = (byte)(account.failed_attempts + 1);

            if (account.failed_attempts >= MaximumFailedAttempts)
            {
                account.failed_attempts = 0;
                account.lockout_ends_at = DateTime.Now.AddMinutes(LockoutMinutes);
            }

            context.SaveChanges();
        }

        private void ApplySuccessfulAccess(CribbageEntities context, account account)
        {
            account.failed_attempts = 0;
            account.lockout_ends_at = null;
            context.SaveChanges();
        }

        private int CalculateRemainingLockoutMinutes(account account)
        {
            TimeSpan remainingTime = account.lockout_ends_at.Value - DateTime.Now;

            return (int)Math.Ceiling(remainingTime.TotalMinutes);
        }

        private AuthenticationResult BuildFailedResult(AuthenticationOutcome outcome)
        {
            return new AuthenticationResult
            {
                Outcome = outcome,
                Username = string.Empty,
                Email = string.Empty,
                LockoutRemainingMinutes = 0
            };
        }

        private AuthenticationResult BuildAccountResult(account account, AuthenticationOutcome outcome)
        {
            return new AuthenticationResult
            {
                Outcome = outcome,
                Username = account.username,
                Email = account.email,
                LockoutRemainingMinutes = 0
            };
        }

        private AuthenticationResult BuildLockedResult(account account)
        {
            AuthenticationResult result = BuildAccountResult(account, AuthenticationOutcome.AccountLocked);
            result.LockoutRemainingMinutes = CalculateRemainingLockoutMinutes(account);

            return result;
        }

        private RegistrationOutcome CreateAccount(string username, string email, string password)
        {
            RegistrationOutcome outcome;

            using (CribbageEntities context = new CribbageEntities())
            {
                if (IsUsernameTaken(context, username))
                {
                    outcome = RegistrationOutcome.UsernameAlreadyTaken;
                }
                else
                {
                    outcome = EvaluateEmailAvailability(context, username, email, password);
                }
            }

            return outcome;
        }

        private bool IsUsernameTaken(CribbageEntities context, string username)
        {
            return context.account.Any(user => user.username == username);
        }

        private RegistrationOutcome EvaluateEmailAvailability(
            CribbageEntities context, string username, string email, string password)
        {
            RegistrationOutcome outcome;

            account existingAccount = context.account.FirstOrDefault(user => user.email == email);

            if (existingAccount == null)
            {
                outcome = SaveNewAccount(context, username, email, password);
            }
            else if (existingAccount.account_status == PendingVerificationStatus)
            {
                outcome = RegistrationOutcome.EmailPendingVerification;
            }
            else
            {
                outcome = RegistrationOutcome.EmailAlreadyRegistered;
            }

            return outcome;
        }

        private RegistrationOutcome SaveNewAccount(
            CribbageEntities context, string username, string email, string password)
        {
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                account account = BuildAccount(username, email, password);
                context.account.Add(account);
                context.SaveChanges();

                context.player_profile.Add(BuildProfile(account.account_id, username));
                context.player_settings.Add(BuildSettings(account.account_id));
                context.SaveChanges();

                transaction.Commit();
            }

            return RegistrationOutcome.Success;
        }

        private account BuildAccount(string username, string email, string password)
        {
            return new account
            {
                username = username,
                email = email,
                password_hash = BCrypt.Net.BCrypt.HashPassword(password),
                account_status = PendingVerificationStatus,
                registered_at = DateTime.Now,
                failed_attempts = 0
            };
        }

        private player_profile BuildProfile(int userId, string username)
        {
            return new player_profile
            {
                account_id = userId,
                display_name = username,
                avatar = DefaultAvatar,
                is_tutorial_completed = false,
                skill_level = InitialSkillLevel,
                matches_played = 0,
                wins = 0,
                losses = 0,
                current_streak = 0
            };
        }

        private player_settings BuildSettings(int userId)
        {
            return new player_settings
            {
                account_id = userId,
                music_volume = DefaultMusicVolume,
                effects_volume = DefaultEffectsVolume,
                language = DefaultLanguage,
                color_blind_mode = DefaultColorBlindMode,
                should_show_animations = true,
                should_notify_requests = true
            };
        }
    }
}
