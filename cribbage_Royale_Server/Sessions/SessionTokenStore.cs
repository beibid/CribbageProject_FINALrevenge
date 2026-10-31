using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace CribbageRoyale.Server.Sessions
{
    public class SessionTokenStore : ISessionTokens
    {
        private const int TokenByteLength = 32;

        private readonly ConcurrentDictionary<string, SessionToken> _tokens =
            new ConcurrentDictionary<string, SessionToken>();

        private readonly GuestAccountIds _guestAccountIds;
        private readonly SessionLifetime _sessionLifetime;

        public SessionTokenStore(GuestAccountIds guestAccountIds, SessionLifetime sessionLifetime)
        {
            _guestAccountIds = guestAccountIds;
            _sessionLifetime = sessionLifetime;
        }

        public string IssueToken(int accountId)
        {
            return StoreToken(accountId, false);
        }

        public string IssueGuestToken(string displayName)
        {
            if (displayName == null)
            {
                throw new ArgumentNullException(nameof(displayName));
            }

            return StoreToken(_guestAccountIds.Next(), true);
        }

        public bool TryValidateToken(string token, out SessionIdentity identity)
        {
            identity = null;
            bool resolved = false;
            SessionToken storedToken;

            if (token != null && _tokens.TryGetValue(token, out storedToken))
            {
                if (storedToken.ExpiresAt > DateTime.UtcNow)
                {
                    identity = new SessionIdentity(storedToken.AccountId, storedToken.IsGuest);
                    resolved = true;
                }
                else
                {
                    RevokeToken(token);
                }
            }

            return resolved;
        }

        public void RevokeToken(string token)
        {
            SessionToken removedToken;
            _tokens.TryRemove(token, out removedToken);
        }

        public void RevokeAllTokens(int accountId)
        {
            List<string> ownedTokens = _tokens
                .Where(entry => entry.Value.AccountId == accountId)
                .Select(entry => entry.Key)
                .ToList();

            foreach (string token in ownedTokens)
            {
                RevokeToken(token);
            }
        }

        private string StoreToken(int accountId, bool isGuest)
        {
            string token = CreateTokenValue();
            DateTime expiresAt = _sessionLifetime.CalculateExpiration();

            _tokens[token] = new SessionToken(accountId, expiresAt, isGuest);

            return token;
        }

        private string CreateTokenValue()
        {
            byte[] buffer = new byte[TokenByteLength];

            using (RandomNumberGenerator generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(buffer);
            }

            return Convert.ToBase64String(buffer);
        }
    }
}