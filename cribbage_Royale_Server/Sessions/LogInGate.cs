using System.Collections.Concurrent;

namespace CribbageRoyale.Server.Sessions
{
    public class LogInGate
    {
        private readonly ConcurrentDictionary<int, object> _locks = new ConcurrentDictionary<int, object>();

        public object GetLock(int accountId)
        {
            return _locks.GetOrAdd(accountId, key => new object());
        }
    }
}