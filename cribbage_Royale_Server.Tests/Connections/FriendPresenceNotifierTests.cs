using System;
using System.Collections.Generic;
using cribbage_Royale_DataAccess.DataAccess;
using CribbageRoyale.Contracts.ServiceContracts;
using CribbageRoyale.Exceptions;
using CribbageRoyale.Server.Connections;
using CribbageRoyale.Server.Projection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CribbageRoyale.Server.Tests.Connections
{
    [TestClass]
    public class FriendPresenceNotifierTests
    {
        private const int AccountId = 7;
        private const int FirstFriendId = 20;
        private const int SecondFriendId = 30;
        private const int GuestAccountId = -1;

        private Mock<IFriendshipRepository> _friendshipRepositoryMock;
        private Mock<IStateProjection> _stateProjectionMock;
        private Mock<IPlayerChannel> _playerChannelMock;
        private Mock<ILogger> _loggerMock;

        [TestInitialize]
        public void SetUp()
        {
            _friendshipRepositoryMock = new Mock<IFriendshipRepository>();
            _stateProjectionMock = new Mock<IStateProjection>();
            _playerChannelMock = new Mock<IPlayerChannel>();
            _loggerMock = new Mock<ILogger>();
            _friendshipRepositoryMock
                .Setup(repository => repository.GetFriendIds(AccountId))
                .Returns(new List<int> { FirstFriendId, SecondFriendId });

            FriendPresenceNotifier notifier = new FriendPresenceNotifier(
                _friendshipRepositoryMock.Object, _stateProjectionMock.Object, _loggerMock.Object);
            notifier.Attach(_playerChannelMock.Object);
        }

        [TestMethod]
        public void PlayerConnects_RegisteredAccount_NotifiesEveryFriend()
        {
            RaiseConnectionChanged(AccountId, true);

            VerifyNotificationSentTo(FirstFriendId, Times.Once());
            VerifyNotificationSentTo(SecondFriendId, Times.Once());
            _stateProjectionMock.Verify(projection => projection.BuildPresenceChange(AccountId, true), Times.Once);
        }

        [TestMethod]
        public void PlayerDisconnects_RegisteredAccount_BuildsOfflinePresence()
        {
            RaiseConnectionChanged(AccountId, false);

            _stateProjectionMock.Verify(projection => projection.BuildPresenceChange(AccountId, false), Times.Once);
            VerifyNotificationSentTo(FirstFriendId, Times.Once());
        }

        [TestMethod]
        public void PlayerConnects_Guest_DoesNotQueryFriends()
        {
            RaiseConnectionChanged(GuestAccountId, true);

            _friendshipRepositoryMock.Verify(repository => repository.GetFriendIds(It.IsAny<int>()), Times.Never);
            VerifyNoNotificationSent();
        }

        [TestMethod]
        public void PlayerConnects_RepositoryFails_LogsErrorWithoutNotifying()
        {
            _friendshipRepositoryMock
                .Setup(repository => repository.GetFriendIds(AccountId))
                .Throws(new DataAccessException());

            RaiseConnectionChanged(AccountId, true);

            VerifyNoNotificationSent();
            _loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<DataAccessException>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);
        }

        private void RaiseConnectionChanged(int accountId, bool isOnline)
        {
            _playerChannelMock.Raise(
                channel => channel.PlayerConnectionChanged += null,
                new PlayerConnectionChangedEventArgs(accountId, isOnline));
        }

        private void VerifyNotificationSentTo(int accountId, Times times)
        {
            _playerChannelMock.Verify(
                channel => channel.TrySendNotification(accountId, It.IsAny<Action<IGameClientCallback>>()),
                times);
        }

        private void VerifyNoNotificationSent()
        {
            _playerChannelMock.Verify(
                channel => channel.TrySendNotification(It.IsAny<int>(), It.IsAny<Action<IGameClientCallback>>()),
                Times.Never);
        }
    }
}
