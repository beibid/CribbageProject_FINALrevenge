using System;

namespace CribbageRoyale.Server.Sessions
{
    public class SessionToken
    {
        private readonly int _accountId;
        private readonly DateTime _expiresAt;

        public SessionToken(int accountId, DateTime expiresAt)
        {
            _accountId = accountId;
            _expiresAt = expiresAt;
        }

        public int AccountId
        {
            get { return _accountId; }
        }

        public DateTime ExpiresAt
        {
            get { return _expiresAt; }
        }
    }
}