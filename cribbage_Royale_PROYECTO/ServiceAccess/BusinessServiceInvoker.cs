using System;
using System.ServiceModel;
using CribbageRoyale.Contracts.FaultContracts;
using Microsoft.Extensions.Logging;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class BusinessServiceInvoker<TChannel, TFault, TFailure>
        where TFailure : struct
    {
        private readonly string _endpointName;
        private readonly Func<TFault, TFailure> _resolveFailure;
        private readonly ILogger _logger;
        private readonly ChannelCloser _channelCloser;

        public BusinessServiceInvoker(string endpointName, Func<TFault, TFailure> resolveFailure, ILogger logger)
        {
            _endpointName = endpointName;
            _resolveFailure = resolveFailure;
            _logger = logger;
            _channelCloser = new ChannelCloser(logger);
        }

        public TResult Execute<TResult>(
            string sessionToken,
            Func<TChannel, TResult> operation,
            Func<ServiceFailureReason, TFailure?, TResult> buildFailedResult)
        {
            ChannelFactory<TChannel> factory = new ChannelFactory<TChannel>(_endpointName);
            TChannel channel = factory.CreateChannel();
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
            Func<ServiceFailureReason, TFailure?, TResult> buildFailedResult,
            out bool callSucceeded)
        {
            TResult result;
            callSucceeded = false;

            try
            {
                result = invocation();
                callSucceeded = true;
            }
            catch (FaultException<TFault> businessFaultException)
            {
                result = buildFailedResult(
                    ServiceFailureReason.RequestRejected,
                    _resolveFailure(businessFaultException.Detail));
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

        private TResult Invoke<TResult>(TChannel channel, string sessionToken, Func<TChannel, TResult> operation)
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
            _logger.LogError(
                exception,
                "The operation on {Endpoint} could not be completed. Reason: {Reason}.",
                _endpointName,
                reason);
        }
    }
}