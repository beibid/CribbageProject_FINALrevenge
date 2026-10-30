using System.Threading;

namespace CribbageRoyale.Server.Sessions
{
    public class GuestAccountIds
    {
        private int _lastAccountId;

        public int Next()
        {
            return Interlocked.Decrement(ref _lastAccountId);
        }
    }
}