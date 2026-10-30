using System.Collections.Concurrent;
using System.Linq;
using CribbageRoyale.Contracts.DataContracts;

namespace CribbageRoyale.Server.Sessions
{
    public class PlayerRegistry
    {
        private readonly ConcurrentDictionary<int, AccountReference> _accounts =
            new ConcurrentDictionary<int, AccountReference>();

        public void Register(AccountReference account)
        {
            _accounts[account.AccountId] = account;
        }

        public void Remove(int accountId)
        {
            AccountReference removedAccount;
            _accounts.TryRemove(accountId, out removedAccount);
        }

        public AccountReference Find(int accountId)
        {
            AccountReference account;
            _accounts.TryGetValue(accountId, out account);

            return account;
        }

        public bool IsDisplayNameInUse(string displayName)
        {
            return _accounts.Values.Any(account => account.DisplayName == displayName);
        }
    }
}