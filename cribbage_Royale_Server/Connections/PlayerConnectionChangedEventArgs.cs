using System;

namespace CribbageRoyale.Server.Connections
{
    public class PlayerConnectionChangedEventArgs : EventArgs
    {
        public PlayerConnectionChangedEventArgs(int accountId, bool isOnline)
        {
            AccountId = accountId;
            IsOnline = isOnline;
        }

        public int AccountId { get; }

        public bool IsOnline { get; }
    }
}