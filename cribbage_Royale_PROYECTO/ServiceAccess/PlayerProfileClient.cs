using System.Threading.Tasks;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class PlayerProfileClient
    {
        private const string EndpointName = "PlayerProfileEndpoint";

        private readonly BusinessServiceInvoker<IPlayerProfile, ProfileFault, ProfileFailure> _invoker;

        public PlayerProfileClient(ILogger logger)
        {
            _invoker = new BusinessServiceInvoker<IPlayerProfile, ProfileFault, ProfileFailure>(
                EndpointName,
                fault => fault.Reason,
                logger);
        }

        public Task<ProfileResult> GetOwnProfileAsync(string sessionToken)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => new ProfileResult(ServiceFailureReason.None, null, channel.GetOwnProfile()),
                BuildFailedResult));
        }

        public Task<ProfileResult> UpdateDisplayNameAsync(string sessionToken, string displayName)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => UpdateDisplayName(channel, displayName),
                BuildFailedResult));
        }

        public Task<ProfileResult> ChangePasswordAsync(string sessionToken, string currentPassword, string newPassword)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => ChangePassword(channel, currentPassword, newPassword),
                BuildFailedResult));
        }

        public Task<ProfileResult> UpdateAvatarAsync(string sessionToken, byte[] image)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => UpdateAvatar(channel, image),
                BuildFailedResult));
        }

        private ProfileResult UpdateDisplayName(IPlayerProfile channel, string displayName)
        {
            channel.UpdateDisplayName(displayName);

            return BuildSucceededResult();
        }

        private ProfileResult ChangePassword(IPlayerProfile channel, string currentPassword, string newPassword)
        {
            channel.ChangePassword(currentPassword, newPassword);

            return BuildSucceededResult();
        }

        private ProfileResult UpdateAvatar(IPlayerProfile channel, byte[] image)
        {
            channel.UpdateAvatar(image);

            return BuildSucceededResult();
        }

        private ProfileResult BuildSucceededResult()
        {
            return new ProfileResult(ServiceFailureReason.None, null, new OwnProfile());
        }

        private ProfileResult BuildFailedResult(ServiceFailureReason reason, ProfileFailure? failure)
        {
            return new ProfileResult(reason, failure, new OwnProfile());
        }
    }
}