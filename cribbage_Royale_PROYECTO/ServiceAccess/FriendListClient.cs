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
        private const string FriendsQueryFailureMessage = "The friends list could not be loaded. Reason: {Reason}.";

        private readonly ILogger _logger;
        private readonly ChannelCloser _channelCloser;

        public FriendListClient(ILogger logger)
        {
            _logger = logger;
            _channelCloser = new ChannelCloser(logger);
        }

        public Task<FriendListResult> GetFriendsAsync(string sessionToken)
        {
            return Task.Run(() => GetFriends(sessionToken));
        }

        private FriendListResult GetFriends(string sessionToken)
        {
            ChannelFactory<IFriendList> factory = new ChannelFactory<IFriendList>(EndpointName);
            IFriendList channel = factory.CreateChannel();

            FriendListResult result = RequestFriends(channel, sessionToken);

            _channelCloser.Release((ICommunicationObject)channel, result.Succeeded);
            _channelCloser.Release(factory, result.Succeeded);

            return result;
        }

        private FriendListResult RequestFriends(IFriendList channel, string sessionToken)
        {
            FriendListResult result;

            try
            {
                result = new FriendListResult(ServiceFailureReason.None, InvokeGetFriends(channel, sessionToken));
            }
            catch (FaultException<SessionFault>)
            {
                result = BuildFailedResult(ServiceFailureReason.SessionRejected);
            }
            catch (FaultException<FriendshipFault>)
            {
                result = BuildFailedResult(ServiceFailureReason.RequestRejected);
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

        private FriendSummary[] InvokeGetFriends(IFriendList channel, string sessionToken)
        {
            FriendSummary[] friends;

            using (OperationContextScope scope = new OperationContextScope((IContextChannel)channel))
            {
                SessionTokenHeader.Attach(OperationContext.Current, sessionToken);
                friends = channel.GetFriends();
            }

            return friends;
        }

        private FriendListResult RegisterFailure(ServiceFailureReason reason, Exception exception)
        {
            _logger.LogError(exception, FriendsQueryFailureMessage, reason);

            return BuildFailedResult(reason);
        }

        private FriendListResult BuildFailedResult(ServiceFailureReason reason)
        {
            return new FriendListResult(reason, new FriendSummary[0]);
        }
    }
}