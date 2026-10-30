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
        private const int TokenLifetimeHours = 10;

        private readonly ConcurrentDictionary<string, SessionToken> _tokens =
            new ConcurrentDictionary<string, SessionToken>();

        public string IssueToken(int accountId)
        {
            string token = CreateTokenValue();
            _tokens[token] = new SessionToken(accountId, DateTime.UtcNow.AddHours(TokenLifetimeHours));

            return token;
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

        public bool TryGetAccountId(string token, out int accountId)
        {
            accountId = 0;
            bool resolved = false;
            SessionToken storedToken;

            if (_tokens.TryGetValue(token, out storedToken))
            {
                if (storedToken.ExpiresAt > DateTime.UtcNow)
                {
                    accountId = storedToken.AccountId;
                    resolved = true;
                }
                else
                {
                    RevokeToken(token);
                }
            }

            return resolved;
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