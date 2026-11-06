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

        private readonly BusinessServiceInvoker<IFriendRequests, FriendshipFault, FriendshipFailure> _invoker;

        public FriendRequestsClient(ILogger logger)
        {
            _invoker = new BusinessServiceInvoker<IFriendRequests, FriendshipFault, FriendshipFailure>(
                EndpointName,
                fault => fault.Reason,
                logger);
        }

        public Task<FriendshipOperationResult> SendFriendRequestAsync(string sessionToken, int accountId)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => Send(channel, accountId),
                BuildFailedResult));
        }

        public Task<FriendshipOperationResult> RespondToFriendRequestAsync(string sessionToken, long requestId, bool accepted)
        {
            return Task.Run(() => _invoker.Execute(
                sessionToken,
                channel => Respond(channel, requestId, accepted),
                BuildFailedResult));
        }

        public Task<FriendshipOperationResult> CancelFriendRequestAsync(string sessionToken, long requestId)
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

        private FriendshipOperationResult Send(IFriendRequests channel, int accountId)
        {
            channel.SendFriendRequest(accountId);

            return BuildSucceededResult();
        }

        private FriendshipOperationResult Respond(IFriendRequests channel, long requestId, bool accepted)
        {
            channel.RespondToFriendRequest(requestId, accepted);

            return BuildSucceededResult();
        }

        private FriendshipOperationResult Cancel(IFriendRequests channel, long requestId)
        {
            channel.CancelFriendRequest(requestId);

            return BuildSucceededResult();
        }

        private FriendshipOperationResult BuildSucceededResult()
        {
            return new FriendshipOperationResult(ServiceFailureReason.None, null);
        }

        private FriendshipOperationResult BuildFailedResult(ServiceFailureReason reason, FriendshipFailure? failure)
        {
            return new FriendshipOperationResult(reason, failure);
        }
    }
}