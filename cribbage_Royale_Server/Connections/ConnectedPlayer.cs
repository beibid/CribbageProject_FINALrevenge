using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.ServiceContracts;

namespace CribbageRoyale.Server.Connections
{
    public class ConnectedPlayer
    {
        public ConnectedPlayer(AccountReference account, IGameClientCallback callback)
        {
            Account = account;
            Callback = callback;
        }

        public AccountReference Account { get; }

        public IGameClientCallback Callback { get; }
    }
}