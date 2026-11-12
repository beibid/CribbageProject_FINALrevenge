using System;
using System.Collections.Generic;
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

        private readonly Dictionary<int, IGameClientCallback> _callbacks =
            new Dictionary<int, IGameClientCallback>();

        private readonly object _syncRoot = new object();
        private readonly ILogger _logger;

        public PlayerChannel(ILogger logger)
        {
            _logger = logger;
        }

        public event EventHandler<PlayerConnectionChangedEventArgs> PlayerConnectionChanged;

        public void RegisterCurrentPlayer(int accountId)
        {
            IGameClientCallback callback = OperationContext.Current.GetCallbackChannel<IGameClientCallback>();
            IGameClientCallback replacedCallback = StoreCallback(accountId, callback);

            AttachChannelEvents(accountId, callback);
            RevokeReplacedCallback(replacedCallback);
            OnPlayerConnectionChanged(accountId, true);
        }

        public void RemovePlayer(int accountId)
        {
            bool removed;

            lock (_syncRoot)
            {
                removed = _callbacks.Remove(accountId);
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
                connected = _callbacks.ContainsKey(accountId);
            }

            return connected;
        }

        public bool TrySendNotification(int accountId, Action<IGameClientCallback> notification)
        {
            IGameClientCallback callback = FindCallback(accountId);
            bool sent = false;

            if (callback != null)
            {
                sent = TryDeliver(callback, notification);

                if (!sent)
                {
                    RemoveWhenOwner(accountId, callback);
                }
            }

            return sent;
        }

        private IGameClientCallback StoreCallback(int accountId, IGameClientCallback callback)
        {
            IGameClientCallback replacedCallback = null;

            lock (_syncRoot)
            {
                if (_callbacks.ContainsKey(accountId))
                {
                    replacedCallback = _callbacks[accountId];
                }

                _callbacks[accountId] = callback;
            }

            return replacedCallback;
        }

        private IGameClientCallback FindCallback(int accountId)
        {
            IGameClientCallback callback;

            lock (_syncRoot)
            {
                _callbacks.TryGetValue(accountId, out callback);
            }

            return callback;
        }

        private void RemoveWhenOwner(int accountId, IGameClientCallback callback)
        {
            bool removed = false;

            lock (_syncRoot)
            {
                IGameClientCallback storedCallback;

                if (_callbacks.TryGetValue(accountId, out storedCallback) && storedCallback == callback)
                {
                    _callbacks.Remove(accountId);
                    removed = true;
                }
            }

            if (removed)
            {
                OnPlayerConnectionChanged(accountId, false);
            }
        }

        private void AttachChannelEvents(int accountId, IGameClientCallback callback)
        {
            ICommunicationObject communicationObject = callback as ICommunicationObject;

            if (communicationObject != null)
            {
                communicationObject.Closed += (sender, arguments) => RemoveWhenOwner(accountId, callback);
                communicationObject.Faulted += (sender, arguments) => RemoveWhenOwner(accountId, callback);
            }
        }

        private void RevokeReplacedCallback(IGameClientCallback replacedCallback)
        {
            if (replacedCallback != null)
            {
                SessionRevocation revocation = new SessionRevocation();
                revocation.Reason = SessionRevocationReason.SignedInElsewhere;

                TryDeliver(replacedCallback, callback => callback.SessionRevoked(revocation));
                CloseCallback(replacedCallback);
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