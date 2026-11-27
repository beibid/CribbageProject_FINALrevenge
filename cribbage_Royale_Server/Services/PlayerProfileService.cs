using System.ServiceModel;
using System.Text.RegularExpressions;
using cribbage_Royale_DataAccess.DataAccess;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using CribbageRoyale.Exceptions;
using CribbageRoyale.Server.Faults;
using CribbageRoyale.Server.Projection;
using CribbageRoyale.Server.Sessions;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class PlayerProfileService : IPlayerProfile
    {
        private const int MaximumDisplayNameLength = 20;
        private const int MaximumImageBytes = 15000;
        private const string PasswordPattern = @"^(?=.*[A-Za-z])(?=.*\d).{8,64}$";
        private const string ProfileQueryFailureMessage =
            "The profile could not be returned. CorrelationId: {CorrelationId}.";
        private const string ProfileUpdateFailureMessage =
            "The profile could not be updated. CorrelationId: {CorrelationId}.";

        private readonly CallerSession _callerSession;
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public PlayerProfileService(CallerSession callerSession, IUserRepository userRepository, ILogger logger)
        {
            _callerSession = callerSession;
            _userRepository = userRepository;
            _logger = logger;
        }

        public OwnProfile GetOwnProfile()
        {
            SessionIdentity caller = _callerSession.RequireRegisteredAccount();
            OwnProfile profile;

            try
            {
                profile = BuildOwnProfile(_userRepository.GetOwnProfile(caller.AccountId));
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildServiceFault(dataAccessException, ProfileQueryFailureMessage);
            }

            return profile;
        }

        public void UpdateDisplayName(string displayName)
        {
            SessionIdentity caller = _callerSession.RequireRegisteredAccount();
            EnsureDisplayNameIsValid(displayName);

            try
            {
                _userRepository.UpdateDisplayName(caller.AccountId, displayName.Trim());
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildServiceFault(dataAccessException, ProfileUpdateFailureMessage);
            }
        }

        public void ChangePassword(string currentPassword, string newPassword)
        {
            SessionIdentity caller = _callerSession.RequireRegisteredAccount();
            EnsureCurrentPasswordWasSent(currentPassword);
            EnsureNewPasswordIsValid(newPassword);

            try
            {
                EnsurePasswordWasChanged(
                    _userRepository.ChangePassword(caller.AccountId, currentPassword, newPassword));
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildServiceFault(dataAccessException, ProfileUpdateFailureMessage);
            }
        }

        public void UpdateAvatar(byte[] image)
        {
            SessionIdentity caller = _callerSession.RequireRegisteredAccount();
            EnsureImageIsValid(image);

            try
            {
                _userRepository.UpdateAvatar(caller.AccountId, image);
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildServiceFault(dataAccessException, ProfileUpdateFailureMessage);
            }
        }

        private void EnsureDisplayNameIsValid(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName) || displayName.Trim().Length > MaximumDisplayNameLength)
            {
                throw BuildProfileFault(ProfileFailure.InvalidDisplayName);
            }
        }

        private void EnsureCurrentPasswordWasSent(string currentPassword)
        {
            if (string.IsNullOrEmpty(currentPassword))
            {
                throw BuildProfileFault(ProfileFailure.WrongPassword);
            }
        }

        private void EnsureNewPasswordIsValid(string newPassword)
        {
            if (newPassword == null || !Regex.IsMatch(newPassword, PasswordPattern))
            {
                throw BuildProfileFault(ProfileFailure.InvalidPassword);
            }
        }

        private void EnsurePasswordWasChanged(bool isChanged)
        {
            if (!isChanged)
            {
                throw BuildProfileFault(ProfileFailure.WrongPassword);
            }
        }

        private void EnsureImageIsValid(byte[] image)
        {
            if (image == null || image.Length > MaximumImageBytes || !IsJpeg(image))
            {
                throw BuildProfileFault(ProfileFailure.InvalidImage);
            }
        }

        private static bool IsJpeg(byte[] image)
        {
            return image.Length > 3 && image[0] == 0xFF && image[1] == 0xD8 && image[2] == 0xFF;
        }

        private static OwnProfile BuildOwnProfile(ProfileRecord record)
        {
            OwnProfile profile = new OwnProfile();
            profile.UserName = record.UserName;
            profile.DisplayName = record.DisplayName;
            profile.Email = record.Email;
            profile.AvatarImage = record.AvatarImage;
            profile.RegisteredAtUtc = RecordProjection.ToUtc(record.RegisteredAt);
            profile.MatchesPlayed = record.MatchesPlayed;
            profile.Wins = record.Wins;
            profile.Losses = record.Losses;
            profile.CurrentStreak = record.CurrentStreak;

            return profile;
        }

        private FaultException<ProfileFault> BuildProfileFault(ProfileFailure reason)
        {
            ProfileFault fault = new ProfileFault();
            fault.Reason = reason;

            _logger.LogInformation(
                "A profile operation was rejected. Reason: {Reason}.",
                reason);

            return new FaultException<ProfileFault>(fault, reason.ToString());
        }

        private FaultException<ServiceFault> BuildServiceFault(DataAccessException dataAccessException, string message)
        {
            FaultException<ServiceFault> faultException =
                ServiceFaultBuilder.Build(ServiceFaultCode.DataAccessFailure);

            _logger.LogError(dataAccessException, message, faultException.Detail.CorrelationId);

            return faultException;
        }
    }
}