using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Server.Sessions;

namespace CribbageRoyale.Server.Projection
{
    public class StateProjection : IStateProjection
    {
        private readonly PlayerRegistry _playerRegistry;

        public StateProjection(PlayerRegistry playerRegistry)
        {
            _playerRegistry = playerRegistry;
        }

        public PresenceChange BuildPresenceChange(int accountId, bool isConnected)
        {
            PresenceChange presence = new PresenceChange();
            presence.Account = ResolveAccount(accountId);
            presence.Scope = PresenceScope.Friend;
            presence.IsConnected = isConnected;
            presence.MatchId = null;

            return presence;
        }

        private AccountReference ResolveAccount(int accountId)
        {
            AccountReference account = _playerRegistry.Find(accountId);

            if (account == null)
            {
                account = new AccountReference();
                account.AccountId = accountId;
            }

            return account;
        }
    }
}