namespace CribbageRoyale.Server.Sessions
{
    public class SessionIdentity
    {
        private readonly int _accountId;
        private readonly bool _isGuest;

        public SessionIdentity(int accountId, bool isGuest)
        {
            _accountId = accountId;
            _isGuest = isGuest;
        }

        public int AccountId
        {
            get { return _accountId; }
        }

        public bool IsGuest
        {
            get { return _isGuest; }
        }
    }
}