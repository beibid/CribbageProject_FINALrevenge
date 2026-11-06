using System.Threading.Tasks;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class FriendListClient
    {
        private const string EndpointName = "FriendListEndpoint";

        private readonly BusinessServiceInvoker<IFriendList, FriendshipFault, FriendshipFailure> _invoker;

        public FriendListClient(ILogger logger)
        {
            _invoker = new BusinessServiceInvoker<IFriendList, FriendshipFault, FriendshipFailure>(
                EndpointName,
                fault => fault.Reason,
                logger);
        }

        public Task<FriendListResult> GetFriendsAsync(string sessionToken)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => new FriendListResult(ServiceFailureReason.None, channel.GetFriends()),
                (reason, failure) => new FriendListResult(reason, new FriendSummary[0])));
        }

        public Task<PlayerSearchResult> FindPlayersAsync(string sessionToken, string searchText)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => new PlayerSearchResult(ServiceFailureReason.None, null, channel.FindPlayers(searchText)),
                (reason, failure) => new PlayerSearchResult(reason, failure, new PlayerSummary[0])));
        }

        public Task<FriendshipOperationResult> RemoveFriendAsync(string sessionToken, int accountId)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => Remove(channel, accountId),
                (reason, failure) => new FriendshipOperationResult(reason, failure)));
        }

        private FriendshipOperationResult Remove(IFriendList channel, int accountId)
        {
            channel.RemoveFriend(accountId);

            return new FriendshipOperationResult(ServiceFailureReason.None, null);
        }
    }
}