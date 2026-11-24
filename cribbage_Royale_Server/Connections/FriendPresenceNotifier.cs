using System.Collections.Generic;
using cribbage_Royale_DataAccess.DataAccess;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Exceptions;
using CribbageRoyale.Server.Projection;
using Microsoft.Extensions.Logging;

namespace CribbageRoyale.Server.Connections
{
    public class FriendPresenceNotifier
    {
        private const int FirstRegisteredAccountId = 1;

        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IStateProjection _stateProjection;
        private readonly ILogger _logger;

        private IPlayerChannel _playerChannel;

        public FriendPresenceNotifier(
            IFriendshipRepository friendshipRepository,
            IStateProjection stateProjection,
            ILogger logger)
        {
            _friendshipRepository = friendshipRepository;
            _stateProjection = stateProjection;
            _logger = logger;
        }

        public void Attach(IPlayerChannel playerChannel)
        {
            _playerChannel = playerChannel;
            _playerChannel.PlayerConnectionChanged += PlayerChannel_PlayerConnectionChanged;
        }

        private void PlayerChannel_PlayerConnectionChanged(object sender, PlayerConnectionChangedEventArgs e)
        {
            if (e.AccountId >= FirstRegisteredAccountId)
            {
                NotifyFriends(e.AccountId, e.IsOnline);
            }
        }

        private void NotifyFriends(int accountId, bool isConnected)
        {
            try
            {
                List<int> friendIds = _friendshipRepository.GetFriendIds(accountId);
                PresenceChange presence = _stateProjection.BuildPresenceChange(accountId, isConnected);

                foreach (int friendId in friendIds)
                {
                    _playerChannel.TrySendNotification(friendId, callback => callback.PresenceChanged(presence));
                }
            }
            catch (DataAccessException dataAccessException)
            {
                _logger.LogError(
                    dataAccessException,
                    "The friends of account {AccountId} could not be notified of its presence.",
                    accountId);
            }
        }
    }
}