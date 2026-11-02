using System;
using System.ServiceModel;
using Microsoft.Extensions.Logging;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class ChannelCloser
    {
        private const int CloseTimeoutSeconds = 3;
        private const string CloseFailureMessage = "The service channel could not be closed.";

        private readonly ILogger _logger;

        public ChannelCloser(ILogger logger)
        {
            _logger = logger;
        }

        public void Release(ICommunicationObject communicationObject, bool callSucceeded)
        {
            if (callSucceeded && communicationObject.State != CommunicationState.Faulted)
            {
                Close(communicationObject);
            }
            else
            {
                communicationObject.Abort();
            }
        }

        private void Close(ICommunicationObject communicationObject)
        {
            try
            {
                communicationObject.Close(TimeSpan.FromSeconds(CloseTimeoutSeconds));
            }
            catch (CommunicationException communicationException)
            {
                _logger.LogWarning(communicationException, CloseFailureMessage);
                communicationObject.Abort();
            }
            catch (TimeoutException timeoutException)
            {
                _logger.LogWarning(timeoutException, CloseFailureMessage);
                communicationObject.Abort();
            }
        }
    }
}