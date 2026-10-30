using System;

namespace CribbageRoyale.Server.Connections
{
    public class PlayerConnectionChangedEventArgs : EventArgs
    {
        private readonly int _accountId;
        private readonly bool _isOnline;

        public PlayerConnectionChangedEventArgs(int accountId, bool isOnline)
        {
            _accountId = accountId;
            _isOnline = isOnline;
        }

        public int AccountId
        {
            get { return _accountId; }
        }

        public bool IsOnline
        {
            get { return _isOnline; }
        }
    }
}