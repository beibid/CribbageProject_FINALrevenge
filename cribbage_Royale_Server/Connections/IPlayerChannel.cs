using System;

namespace CribbageRoyale.Server.Connections
{
    public interface IPlayerChannel
    {
        event EventHandler<PlayerConnectionChangedEventArgs> PlayerConnectionChanged;

        void RegisterCurrentPlayer(int accountId);

        void RemovePlayer(int accountId);

        bool IsPlayerConnected(int accountId);

        bool TrySendNotification(int accountId, Notification notification);
    }
}