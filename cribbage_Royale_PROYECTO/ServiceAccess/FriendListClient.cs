using System;
using System.ServiceModel;
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
        private const string OperationFailureMessage =
            "The friend list operation could not be completed. Reason: {Reason}.";

        private readonly ILogger _logger;
        private readonly ChannelCloser _channelCloser;

        public FriendListClient(ILogger logger)
        {
            _logger = logger;
            _channelCloser = new ChannelCloser(logger);
        }

        public Task<FriendListResult> GetFriendsAsync(string sessionToken)
        {
            return Task.Run(() => Execute(
                sessionToken,
                channel => new FriendListResult(ServiceFailureReason.None, channel.GetFriends()),
                (reason, failure) => new FriendListResult(reason, new FriendSummary[0])));
        }

        public Task<PlayerSearchResult> FindPlayersAsync(string sessionToken, string searchText)
        {
            return Task.Run(() => Execute(
                sessionToken,
                channel => new PlayerSearchResult(ServiceFailureReason.None, null, channel.FindPlayers(searchText)),
                (reason, failure) => new PlayerSearchResult(reason, failure, new PlayerSummary[0])));
        }

        private TResult Execute<TResult>(
            string sessionToken,
            Func<IFriendList, TResult> operation,
            Func<ServiceFailureReason, FriendshipFailure?, TResult> buildFailedResult)
        {
            ChannelFactory<IFriendList> factory = new ChannelFactory<IFriendList>(EndpointName);
            IFriendList channel = factory.CreateChannel();
            bool callSucceeded;

            TResult result = Request(
                () => Invoke(channel, sessionToken, operation),
                buildFailedResult,
                out callSucceeded);

            _channelCloser.Release((ICommunicationObject)channel, callSucceeded);
            _channelCloser.Release(factory, callSucceeded);

            return result;
        }

        private TResult Request<TResult>(
            Func<TResult> invocation,
            Func<ServiceFailureReason, FriendshipFailure?, TResult> buildFailedResult,
            out bool callSucceeded)
        {
            TResult result;
            callSucceeded = false;

            try
            {
                result = invocation();
                callSucceeded = true;
            }
            catch (FaultException<FriendshipFault> friendshipFaultException)
            {
                result = buildFailedResult(
                    ServiceFailureReason.RequestRejected,
                    friendshipFaultException.Detail.Reason);
            }
            catch (FaultException<SessionFault>)
            {
                result = buildFailedResult(ServiceFailureReason.SessionRejected, null);
            }
            catch (FaultException<ServiceFault> serviceFaultException)
            {
                RegisterFailure(ServiceFailureReason.ServerError, serviceFaultException);
                result = buildFailedResult(ServiceFailureReason.ServerError, null);
            }
            catch (EndpointNotFoundException endpointNotFoundException)
            {
                RegisterFailure(ServiceFailureReason.ServerUnavailable, endpointNotFoundException);
                result = buildFailedResult(ServiceFailureReason.ServerUnavailable, null);
            }
            catch (TimeoutException timeoutException)
            {
                RegisterFailure(ServiceFailureReason.Timeout, timeoutException);
                result = buildFailedResult(ServiceFailureReason.Timeout, null);
            }
            catch (CommunicationException communicationException)
            {
                RegisterFailure(ServiceFailureReason.ServerUnavailable, communicationException);
                result = buildFailedResult(ServiceFailureReason.ServerUnavailable, null);
            }

            return result;
        }

        private TResult Invoke<TResult>(IFriendList channel, string sessionToken, Func<IFriendList, TResult> operation)
        {
            TResult result;

            using (OperationContextScope scope = new OperationContextScope((IContextChannel)channel))
            {
                SessionTokenHeader.Attach(OperationContext.Current, sessionToken);
                result = operation(channel);
            }

            return result;
        }

        private void RegisterFailure(ServiceFailureReason reason, Exception exception)
        {
            _logger.LogError(exception, OperationFailureMessage, reason);
        }
    }
}