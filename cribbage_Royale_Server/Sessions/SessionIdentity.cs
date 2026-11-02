namespace CribbageRoyale.Server.Sessions
{
    public class SessionIdentity
    {
        public SessionIdentity(int accountId, bool isGuest)
        {
            AccountId = accountId;
            IsGuest = isGuest;
        }

        public int AccountId { get; }

        public bool IsGuest { get; }
    }
}