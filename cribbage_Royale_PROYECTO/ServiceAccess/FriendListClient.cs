using System.Threading.Tasks;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class FriendListClient
    {
        private const string EndpointName = "FriendListEndpoint";

        private readonly FriendshipServiceInvoker<IFriendList> _invoker;

        public FriendListClient(ILogger logger)
        {
            _invoker = new FriendshipServiceInvoker<IFriendList>(EndpointName, logger);
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
    }
}