using System;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.ServiceContracts;

namespace CribbageRoyale.Server.Connections
{
    public interface IPlayerChannel
    {
        event EventHandler<PlayerConnectionChangedEventArgs> PlayerConnectionChanged;

        void RegisterCurrentPlayer(AccountReference account);

        bool TryRegisterCurrentGuest(AccountReference account);

        void RemovePlayer(int accountId);

        bool IsPlayerConnected(int accountId);

        bool TryFindAccount(int accountId, out AccountReference account);

        bool TrySendNotification(int accountId, Action<IGameClientCallback> notification);
    }
}