using System;
using CribbageRoyale.Contracts.ServiceContracts;

namespace CribbageRoyale.Server.Connections
{
    public interface IPlayerChannel
    {
        event EventHandler<PlayerConnectionChangedEventArgs> PlayerConnectionChanged;

        void RegisterCurrentPlayer(int accountId);

        void RemovePlayer(int accountId);

        bool IsPlayerConnected(int accountId);

        bool TrySendNotification(int accountId, Action<IGameClientCallback> notification);
    }
}