using System;

namespace CribbageRoyale.Server.Sessions
{
    public class SessionToken
    {
        private readonly int _accountId;
        private readonly DateTime _expiresAt;
        private readonly bool _isGuest;

        public SessionToken(int accountId, DateTime expiresAt, bool isGuest)
        {
            _accountId = accountId;
            _expiresAt = expiresAt;
            _isGuest = isGuest;
        }

        public int AccountId
        {
            get { return _accountId; }
        }

        public DateTime ExpiresAt
        {
            get { return _expiresAt; }
        }

        public bool IsGuest
        {
            get { return _isGuest; }
        }
    }
}