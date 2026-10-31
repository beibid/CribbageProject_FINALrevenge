using System.ServiceModel;
using System.ServiceModel.Channels;
using CribbageRoyale.Contracts.FaultContracts;
using CribbageRoyale.Server.Faults;
using CribbageRoyale.Server.Sessions;

namespace CribbageRoyale.Server.Services
{
    public class CallerSession
    {
        private const string HeaderName = "SessionToken";
        private const string HeaderNamespace = "urn:cribbageroyale:services:v1";
        private const int MissingHeaderIndex = -1;

        private readonly ISessionTokens _sessionTokens;

        public CallerSession(ISessionTokens sessionTokens)
        {
            _sessionTokens = sessionTokens;
        }

        public SessionIdentity RequireRegisteredAccount()
        {
            SessionIdentity identity = RequireSession();

            if (identity.IsGuest)
            {
                throw SessionFaultBuilder.Build(SessionFailure.GuestNotAllowed);
            }

            return identity;
        }

        public SessionIdentity RequireSession()
        {
            string token = ReadToken();

            if (string.IsNullOrWhiteSpace(token))
            {
                throw SessionFaultBuilder.Build(SessionFailure.MissingSessionToken);
            }

            SessionIdentity identity;

            if (!_sessionTokens.TryValidateToken(token, out identity))
            {
                throw SessionFaultBuilder.Build(SessionFailure.InvalidSession);
            }

            return identity;
        }

        private string ReadToken()
        {
            string token = null;
            MessageHeaders headers = OperationContext.Current.IncomingMessageHeaders;
            int headerIndex = headers.FindHeader(HeaderName, HeaderNamespace);

            if (headerIndex != MissingHeaderIndex)
            {
                token = headers.GetHeader<string>(headerIndex);
            }

            return token;
        }
    }
}