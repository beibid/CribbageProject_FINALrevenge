using System;
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

            return GrantGuestSession(GuestNames.Build(displayName));
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

            DateTime expiresAtUtc = _services.SessionLifetime.CalculateExpiration();
            string token = StartExclusiveSession(account);

            AuthenticatedSession session = new AuthenticatedSession();
            session.SessionToken = token;
            session.Account = account;
            session.EmailAddress = credentials.Email;
            session.PreferredLocaleCode = credentials.PreferredLocaleCode;
            session.ExpiresAtUtc = expiresAtUtc;
            session.IsSupportAccount = false;
            session.UnfinishedMatchId = null;

            return session;
        }

        private GuestSession GrantGuestSession(string guestName)
        {
            DateTime expiresAtUtc = _services.SessionLifetime.CalculateExpiration();
            string token = _services.SessionTokens.IssueGuestToken(guestName);

            AccountReference account = new AccountReference();
            account.AccountId = ResolveAccountId(token);
            account.DisplayName = guestName;

            if (!_services.PlayerChannel.TryRegisterCurrentGuest(account))
            {
                _services.SessionTokens.RevokeToken(token);

                throw SessionFaultBuilder.Build(SessionFailure.GuestNameUnavailable);
            }

            _currentAccount = account;
            _currentToken = token;

            GuestSession session = new GuestSession();
            session.SessionToken = token;
            session.DisplayName = guestName;
            session.ExpiresAtUtc = expiresAtUtc;

            return session;
        }

        private int ResolveAccountId(string token)
        {
            SessionIdentity identity;

            if (!_services.SessionTokens.TryValidateToken(token, out identity))
            {
                throw SessionFaultBuilder.Build(SessionFailure.InvalidSession);
            }

            return identity.AccountId;
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
                _services.PlayerChannel.RegisterCurrentPlayer(account);
            }

            return token;
        }

        private void ReleaseCurrentSession()
        {
            if (_currentAccount != null)
            {
                _services.SessionTokens.RevokeToken(_currentToken);
                _services.PlayerChannel.RemovePlayer(_currentAccount.AccountId);

                _currentAccount = null;
                _currentToken = null;
            }
        }

        private FaultException<ServiceFault> BuildServiceFault(DataAccessException dataAccessException)
        {
            FaultException<ServiceFault> faultException =
                ServiceFaultBuilder.Build(ServiceFaultCode.DataAccessFailure);

            _logger.LogError(
                dataAccessException,
                "The log in could not be completed. CorrelationId: {CorrelationId}.",
                faultException.Detail.CorrelationId);

            return faultException;
        }
    }
}