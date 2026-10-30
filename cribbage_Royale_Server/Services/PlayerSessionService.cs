using System.ServiceModel;
using cribbage_Royale_DataAccess.DataAccess;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using CribbageRoyale.Exceptions;
using CribbageRoyale.Server.Faults;
using CribbageRoyale.Server.Sessions;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class PlayerSessionService : IPlayerSession
    {
        private const string LogInFailureMessage =
            "The log in could not be completed. CorrelationId: {CorrelationId}";

        private readonly SessionServices _services;
        private readonly ILogger _logger;

        private AccountReference _currentAccount;
        private string _currentToken;

        public PlayerSessionService(SessionServices services, ILogger logger)
        {
            _services = services;
            _logger = logger;
        }

        public AuthenticatedSession LogIn(string userNameOrEmail, string password)
        {
            AuthenticatedSession session;

            try
            {
                session = OpenSession(userNameOrEmail, password);
            }
            catch (DataAccessException dataAccessException)
            {
                throw BuildServiceFault(dataAccessException);
            }

            return session;
        }

        public GuestSession LogInAsGuest(string displayName)
        {
            ReleaseCurrentSession();

            if (!GuestNames.IsValid(displayName))
            {
                throw SessionFaultBuilder.Build(SessionFailure.InvalidGuestName);
            }

            string guestName = GuestNames.Build(displayName);

            if (_services.PlayerRegistry.IsDisplayNameInUse(guestName))
            {
                throw SessionFaultBuilder.Build(SessionFailure.GuestNameUnavailable);
            }

            return GrantGuestSession(guestName);
        }

        public void LogOut()
        {
            ReleaseCurrentSession();
        }

        private AuthenticatedSession OpenSession(string userNameOrEmail, string password)
        {
            ReleaseCurrentSession();

            UserCredentials credentials;

            if (!_services.Authenticator.TryVerifyCredentials(userNameOrEmail, password, out credentials))
            {
                throw SessionFaultBuilder.Build(SessionFailure.InvalidCredentials);
            }

            EnsureAccountIsUsable(credentials);

            return GrantSession(credentials);
        }

        private void EnsureAccountIsUsable(UserCredentials credentials)
        {
            if (credentials.Status == AccountStatus.Suspended)
            {
                throw SessionFaultBuilder.Build(SessionFailure.AccountSuspended);
            }

            if (credentials.Status == AccountStatus.Deleted)
            {
                throw SessionFaultBuilder.Build(SessionFailure.AccountDeleted);
            }

            if (credentials.Status != AccountStatus.Verified)
            {
                throw SessionFaultBuilder.Build(SessionFailure.AccountNotVerified);
            }
        }

        private AuthenticatedSession GrantSession(UserCredentials credentials)
        {
            AccountReference account = new AccountReference();
            account.AccountId = credentials.AccountId;
            account.DisplayName = credentials.DisplayName;

            string token = StartExclusiveSession(account);

            AuthenticatedSession session = new AuthenticatedSession();
            session.Account = account;
            session.Email = credentials.Email;
            session.SessionToken = token;

            return session;
        }

        private GuestSession GrantGuestSession(string guestName)
        {
            AccountReference account = new AccountReference();
            account.AccountId = _services.GuestAccountIds.Next();
            account.DisplayName = guestName;

            string token = StartExclusiveSession(account);

            GuestSession session = new GuestSession();
            session.Account = account;
            session.SessionToken = token;

            return session;
        }

        private string StartExclusiveSession(AccountReference account)
        {
            string token;

            lock (_services.LogInGate.GetLock(account.AccountId))
            {
                _services.SessionTokens.RevokeAllTokens(account.AccountId);
                token = _services.SessionTokens.IssueToken(account.AccountId);

                _currentAccount = account;
                _currentToken = token;
                _services.PlayerRegistry.Register(account);

                _services.PlayerChannel.RegisterCurrentPlayer(account.AccountId);
            }

            return token;
        }

        private void ReleaseCurrentSession()
        {
            if (_currentAccount != null)
            {
                _services.SessionTokens.RevokeToken(_currentToken);
                _services.PlayerRegistry.Remove(_currentAccount.AccountId);
                _services.PlayerChannel.RemovePlayer(_currentAccount.AccountId);

                _currentAccount = null;
                _currentToken = null;
            }
        }

        private FaultException<ServiceFault> BuildServiceFault(DataAccessException dataAccessException)
        {
            FaultException<ServiceFault> faultException =
                ServiceFaultBuilder.Build(ServiceFaultCode.DataAccessFailure);

            _logger.LogError(dataAccessException, LogInFailureMessage, faultException.Detail.CorrelationId);

            return faultException;
        }
    }
}