using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server.Connections
{
    public class PlayerChannel : IPlayerChannel
    {
        private const int CloseTimeoutSeconds = 3;
        private const string CloseFailureMessage = "The replaced callback channel could not be closed.";
        private const string NotificationFailureMessage = "The notification could not be delivered.";

        private readonly Dictionary<int, ConnectedPlayer> _players = new Dictionary<int, ConnectedPlayer>();
        private readonly object _syncRoot = new object();
        private readonly ILogger _logger;

        public PlayerChannel(ILogger logger)
        {
            _logger = logger;
        }

        public event EventHandler<PlayerConnectionChangedEventArgs> PlayerConnectionChanged;

        public void RegisterCurrentPlayer(AccountReference account)
        {
            ConnectedPlayer player = new ConnectedPlayer(account, GetCurrentCallback());
            ConnectedPlayer replacedPlayer = StorePlayer(player);

            Connect(player, replacedPlayer);
        }

        public bool TryRegisterCurrentGuest(AccountReference account)
        {
            ConnectedPlayer player = new ConnectedPlayer(account, GetCurrentCallback());
            bool registered;

            lock (_syncRoot)
            {
                registered = !IsDisplayNameInUse(account.DisplayName);

                if (registered)
                {
                    _players[account.AccountId] = player;
                }
            }

            if (registered)
            {
                Connect(player, null);
            }

            return registered;
        }

        public void RemovePlayer(int accountId)
        {
            bool removed;

            lock (_syncRoot)
            {
                removed = _players.Remove(accountId);
            }

            if (removed)
            {
                OnPlayerConnectionChanged(accountId, false);
            }
        }

        public bool IsPlayerConnected(int accountId)
        {
            bool connected;

            lock (_syncRoot)
            {
                connected = _players.ContainsKey(accountId);
            }

            return connected;
        }

        public bool TryFindAccount(int accountId, out AccountReference account)
        {
            ConnectedPlayer player;
            bool found = TryFindPlayer(accountId, out player);

            account = found ? player.Account : null;

            return found;
        }

        public bool TrySendNotification(int accountId, Action<IGameClientCallback> notification)
        {
            ConnectedPlayer player;
            bool sent = false;

            if (TryFindPlayer(accountId, out player))
            {
                sent = TryDeliver(player.Callback, notification);

                if (!sent)
                {
                    RemoveWhenOwner(player);
                }
            }

            return sent;
        }

        private IGameClientCallback GetCurrentCallback()
        {
            return OperationContext.Current.GetCallbackChannel<IGameClientCallback>();
        }

        private ConnectedPlayer StorePlayer(ConnectedPlayer player)
        {
            ConnectedPlayer replacedPlayer;

            lock (_syncRoot)
            {
                _players.TryGetValue(player.Account.AccountId, out replacedPlayer);
                _players[player.Account.AccountId] = player;
            }

            return replacedPlayer;
        }

        private bool IsDisplayNameInUse(string displayName)
        {
            return _players.Values.Any(player => player.Account.DisplayName == displayName);
        }

        private bool TryFindPlayer(int accountId, out ConnectedPlayer player)
        {
            bool found;

            lock (_syncRoot)
            {
                found = _players.TryGetValue(accountId, out player);
            }

            return found;
        }

        private void Connect(ConnectedPlayer player, ConnectedPlayer replacedPlayer)
        {
            AttachChannelEvents(player);
            RevokeReplacedPlayer(replacedPlayer);
            OnPlayerConnectionChanged(player.Account.AccountId, true);
        }

        private void RemoveWhenOwner(ConnectedPlayer player)
        {
            bool removed = false;

            lock (_syncRoot)
            {
                ConnectedPlayer storedPlayer;

                if (_players.TryGetValue(player.Account.AccountId, out storedPlayer) && storedPlayer == player)
                {
                    _players.Remove(player.Account.AccountId);
                    removed = true;
                }
            }

            if (removed)
            {
                OnPlayerConnectionChanged(player.Account.AccountId, false);
            }
        }

        private void AttachChannelEvents(ConnectedPlayer player)
        {
            ICommunicationObject communicationObject = player.Callback as ICommunicationObject;

            if (communicationObject != null)
            {
                communicationObject.Closed += (sender, arguments) => RemoveWhenOwner(player);
                communicationObject.Faulted += (sender, arguments) => RemoveWhenOwner(player);
            }
        }

        private void RevokeReplacedPlayer(ConnectedPlayer replacedPlayer)
        {
            if (replacedPlayer != null)
            {
                SessionRevocation revocation = new SessionRevocation();
                revocation.Reason = SessionRevocationReason.SignedInElsewhere;

                TryDeliver(replacedPlayer.Callback, callback => callback.SessionRevoked(revocation));
                CloseCallback(replacedPlayer.Callback);
            }
        }

        private bool TryDeliver(IGameClientCallback callback, Action<IGameClientCallback> notification)
        {
            bool sent = false;

            try
            {
                notification(callback);
                sent = true;
            }
            catch (CommunicationException communicationException)
            {
                _logger.LogWarning(communicationException, NotificationFailureMessage);
            }
            catch (TimeoutException timeoutException)
            {
                _logger.LogWarning(timeoutException, NotificationFailureMessage);
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                _logger.LogWarning(objectDisposedException, NotificationFailureMessage);
            }

            return sent;
        }

        private void CloseCallback(IGameClientCallback callback)
        {
            ICommunicationObject communicationObject = callback as ICommunicationObject;

            if (communicationObject != null)
            {
                try
                {
                    communicationObject.Close(TimeSpan.FromSeconds(CloseTimeoutSeconds));
                }
                catch (CommunicationException communicationException)
                {
                    _logger.LogWarning(communicationException, CloseFailureMessage);
                    communicationObject.Abort();
                }
                catch (TimeoutException timeoutException)
                {
                    _logger.LogWarning(timeoutException, CloseFailureMessage);
                    communicationObject.Abort();
                }
            }
        }

        private void OnPlayerConnectionChanged(int accountId, bool isOnline)
        {
            EventHandler<PlayerConnectionChangedEventArgs> handler = PlayerConnectionChanged;

            if (handler != null)
            {
                handler(this, new PlayerConnectionChangedEventArgs(accountId, isOnline));
            }
        }
    }
}