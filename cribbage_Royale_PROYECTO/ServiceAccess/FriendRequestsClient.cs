using System;
using System.ServiceModel;
using System.Threading.Tasks;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class FriendRequestsClient
    {
        private const string EndpointName = "FriendRequestsEndpoint";
        private const string SendFailureMessage = "The friend request could not be sent. Reason: {Reason}.";

        private readonly ILogger _logger;
        private readonly ChannelCloser _channelCloser;

        public FriendRequestsClient(ILogger logger)
        {
            _logger = logger;
            _channelCloser = new ChannelCloser(logger);
        }

        public Task<FriendRequestResult> SendFriendRequestAsync(string sessionToken, int accountId)
        {
            return Task.Run(() => SendFriendRequest(sessionToken, accountId));
        }

        private FriendRequestResult SendFriendRequest(string sessionToken, int accountId)
        {
            ChannelFactory<IFriendRequests> factory = new ChannelFactory<IFriendRequests>(EndpointName);
            IFriendRequests channel = factory.CreateChannel();

            FriendRequestResult result = RequestSend(channel, sessionToken, accountId);

            _channelCloser.Release((ICommunicationObject)channel, result.Succeeded);
            _channelCloser.Release(factory, result.Succeeded);

            return result;
        }

        private FriendRequestResult RequestSend(IFriendRequests channel, string sessionToken, int accountId)
        {
            FriendRequestResult result;

            try
            {
                InvokeSend(channel, sessionToken, accountId);
                result = new FriendRequestResult(ServiceFailureReason.None, null);
            }
            catch (FaultException<FriendshipFault> friendshipFaultException)
            {
                result = new FriendRequestResult(
                    ServiceFailureReason.RequestRejected,
                    friendshipFaultException.Detail.Reason);
            }
            catch (FaultException<SessionFault>)
            {
                result = new FriendRequestResult(ServiceFailureReason.SessionRejected, null);
            }
            catch (FaultException<ServiceFault> serviceFaultException)
            {
                result = RegisterFailure(ServiceFailureReason.ServerError, serviceFaultException);
            }
            catch (EndpointNotFoundException endpointNotFoundException)
            {
                result = RegisterFailure(ServiceFailureReason.ServerUnavailable, endpointNotFoundException);
            }
            catch (TimeoutException timeoutException)
            {
                result = RegisterFailure(ServiceFailureReason.Timeout, timeoutException);
            }
            catch (CommunicationException communicationException)
            {
                result = RegisterFailure(ServiceFailureReason.ServerUnavailable, communicationException);
            }

            return result;
        }

        private void InvokeSend(IFriendRequests channel, string sessionToken, int accountId)
        {
            using (OperationContextScope scope = new OperationContextScope((IContextChannel)channel))
            {
                SessionTokenHeader.Attach(OperationContext.Current, sessionToken);
                channel.SendFriendRequest(accountId);
            }
        }

        private FriendRequestResult RegisterFailure(ServiceFailureReason reason, Exception exception)
        {
            _logger.LogError(exception, SendFailureMessage, reason);

            return new FriendRequestResult(reason, null);
        }
    }
}