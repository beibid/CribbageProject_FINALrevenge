using System.Threading.Tasks;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class FriendRequestsClient
    {
        private const string EndpointName = "FriendRequestsEndpoint";

        private readonly FriendshipServiceInvoker<IFriendRequests> _invoker;

        public FriendRequestsClient(ILogger logger)
        {
            _invoker = new FriendshipServiceInvoker<IFriendRequests>(EndpointName, logger);
        }

        public Task<FriendRequestResult> SendFriendRequestAsync(string sessionToken, int accountId)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => Send(channel, accountId),
                BuildFailedResult));
        }

        public Task<FriendRequestResult> RespondToFriendRequestAsync(string sessionToken, long requestId, bool accepted)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => Respond(channel, requestId, accepted),
                BuildFailedResult));
        }

        public Task<FriendRequestResult> CancelFriendRequestAsync(string sessionToken, long requestId)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => Cancel(channel, requestId),
                BuildFailedResult));
        }

        public Task<PendingFriendRequestsResult> GetPendingFriendRequestsAsync(string sessionToken)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => new PendingFriendRequestsResult(
                    ServiceFailureReason.None,
                    channel.GetPendingFriendRequests()),
                (reason, failure) => new PendingFriendRequestsResult(reason, new FriendRequest[0])));
        }

        private FriendRequestResult Send(IFriendRequests channel, int accountId)
        {
            channel.SendFriendRequest(accountId);

            return BuildSucceededResult();
        }

        private FriendRequestResult Respond(IFriendRequests channel, long requestId, bool accepted)
        {
            channel.RespondToFriendRequest(requestId, accepted);

            return BuildSucceededResult();
        }

        private FriendRequestResult Cancel(IFriendRequests channel, long requestId)
        {
            channel.CancelFriendRequest(requestId);

            return BuildSucceededResult();
        }

        private FriendRequestResult BuildSucceededResult()
        {
            return new FriendRequestResult(ServiceFailureReason.None, null);
        }

        private FriendRequestResult BuildFailedResult(ServiceFailureReason reason, FriendshipFailure? failure)
        {
            return new FriendRequestResult(reason, failure);
        }
    }
}