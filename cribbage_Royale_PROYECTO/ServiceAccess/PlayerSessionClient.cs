using System;
using System.ServiceModel;
using System.Threading.Tasks;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace cribbage_Royale_PROYECTO.ServiceAccess
{
    public class PlayerSessionClient
    {
        private const string EndpointName = "PlayerSessionEndpoint";
        private const int CloseTimeoutSeconds = 3;
        private const string SignOutFailureMessage = "The sign out could not be completed.";

        private readonly GameClientCallback _callback;
        private readonly ILogger _logger;

        private DuplexChannelFactory<IPlayerSession> _factory;
        private IPlayerSession _channel;
        private bool _sessionOpen;
        private bool _closingChannel;

        public PlayerSessionClient(GameClientCallback callback, ILogger logger)
        {
            _callback = callback;
            _logger = logger;
        }

        public event EventHandler ConnectionLost;

        public Task<SignInResult> SignInAsync(string userNameOrEmail, string password)
        {
            return Task.Run(() => SignIn(userNameOrEmail, password));
        }

        public Task<GuestSignInResult> SignInAsGuestAsync(string displayName)
        {
            return Task.Run(() => SignInAsGuest(displayName));
        }

        public Task SignOutAsync()
        {
            return Task.Run(() => SignOut());
        }

        public bool IsConnected()
        {
            ICommunicationObject communicationObject = _channel as ICommunicationObject;
            bool connected = false;

            if (communicationObject != null && communicationObject.State == CommunicationState.Opened)
            {
                connected = true;
            }

            return connected;
        }

        private SignInResult SignIn(string userNameOrEmail, string password)
        {
            return ExecuteSessionOperation(
                () => OpenSession(userNameOrEmail, password),
                (reason, failure) => new SignInResult(reason, failure, null));
        }

        private GuestSignInResult SignInAsGuest(string displayName)
        {
            return ExecuteSessionOperation(
                () => OpenGuestSession(displayName),
                (reason, failure) => new GuestSignInResult(reason, failure, null));
        }

        private TResult ExecuteSessionOperation<TResult>(
            Func<TResult> openSession,
            Func<ServiceFailureReason, SessionFailure?, TResult> buildFailedResult)
        {
            TResult result;

            try
            {
                result = openSession();
            }
            catch (FaultException<SessionFault> sessionFaultException)
            {
                CloseChannel();
                result = buildFailedResult(ServiceFailureReason.SessionRejected, sessionFaultException.Detail.Failure);
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

        private SignInResult OpenSession(string userNameOrEmail, string password)
        {
            CreateChannel();
            AuthenticatedSession session = _channel.LogIn(userNameOrEmail, password);
            _sessionOpen = true;

            return new SignInResult(ServiceFailureReason.None, null, session);
        }

        private GuestSignInResult OpenGuestSession(string displayName)
        {
            CreateChannel();
            GuestSession session = _channel.LogInAsGuest(displayName);
            _sessionOpen = true;

            return new GuestSignInResult(ServiceFailureReason.None, null, session);
        }

        private void CreateChannel()
        {
            CloseChannel();

            InstanceContext instanceContext = new InstanceContext(_callback);
            _factory = new DuplexChannelFactory<IPlayerSession>(instanceContext, EndpointName);
            _channel = _factory.CreateChannel();

            AttachChannelEvents();
        }

        private void AttachChannelEvents()
        {
            ICommunicationObject communicationObject = _channel as ICommunicationObject;

            if (communicationObject != null)
            {
                communicationObject.Faulted += CommunicationObject_Faulted;
                communicationObject.Closed += CommunicationObject_Closed;
            }
        }

        private void DetachChannelEvents()
        {
            ICommunicationObject communicationObject = _channel as ICommunicationObject;

            if (communicationObject != null)
            {
                communicationObject.Faulted -= CommunicationObject_Faulted;
                communicationObject.Closed -= CommunicationObject_Closed;
            }
        }

        private void CommunicationObject_Faulted(object sender, EventArgs e)
        {
            RaiseConnectionLost();
        }

        private void CommunicationObject_Closed(object sender, EventArgs e)
        {
            RaiseConnectionLost();
        }

        private void SignOut()
        {
            try
            {
                if (IsConnected())
                {
                    _channel.LogOut();
                }
            }
            catch (CommunicationException communicationException)
            {
                _logger.LogWarning(communicationException, SignOutFailureMessage);
            }
            catch (TimeoutException timeoutException)
            {
                _logger.LogWarning(timeoutException, SignOutFailureMessage);
            }

            CloseChannel();
        }

        private void CloseChannel()
        {
            _closingChannel = true;
            DetachChannelEvents();
            CloseCommunicationObject(_channel as ICommunicationObject);
            CloseCommunicationObject(_factory);

            _channel = null;
            _factory = null;
            _sessionOpen = false;
            _closingChannel = false;
        }

        private void CloseCommunicationObject(ICommunicationObject communicationObject)
        {
            if (communicationObject != null && communicationObject.State != CommunicationState.Closed)
            {
                try
                {
                    communicationObject.Close(TimeSpan.FromSeconds(CloseTimeoutSeconds));
                }
                catch (CommunicationException communicationException)
                {
                    _logger.LogWarning(communicationException, SignOutFailureMessage);
                    communicationObject.Abort();
                }
                catch (TimeoutException timeoutException)
                {
                    _logger.LogWarning(timeoutException, SignOutFailureMessage);
                    communicationObject.Abort();
                }
            }
        }

        private void RegisterFailure(ServiceFailureReason reason, Exception exception)
        {
            _logger.LogError(
                exception,
                "The sign in could not be completed. Reason: {Reason}.",
                reason);
            CloseChannel();
        }

        private void RaiseConnectionLost()
        {
            EventHandler handler = ConnectionLost;

            if (_sessionOpen && !_closingChannel && handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}