using System;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Contracts.ServiceContracts;
using CribbageRoyale.Rules;
using CribbageRoyale.Server.Connections;
using CribbageRoyale.Server.Matches;
using CribbageRoyale.Server.Projection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CribbageRoyale.Server.Tests.Matches
{
    [TestClass]
    public class MatchDisconnectionWatcherTests
    {
        private const int LeavingAccountId = 7;
        private const int RivalAccountId = 8;
        private const int InvitedAccountId = 9;
        private const int InvitingAccountId = 10;

        private Mock<IPlayerChannel> _playerChannelMock;
        private Mock<IStateProjection> _stateProjectionMock;
        private Mock<IGameClientCallback> _callbackMock;
        private MatchInvitationRegistry _invitationRegistry;
        private MatchRoomRegistry _matchRoomRegistry;

        [TestInitialize]
        public void SetUp()
        {
            _playerChannelMock = new Mock<IPlayerChannel>();
            _stateProjectionMock = new Mock<IStateProjection>();
            _callbackMock = new Mock<IGameClientCallback>();
            _invitationRegistry = new MatchInvitationRegistry();
            _matchRoomRegistry = new MatchRoomRegistry();
            _playerChannelMock
                .Setup(channel => channel.TrySendNotification(It.IsAny<int>(), It.IsAny<Action<IGameClientCallback>>()))
                .Callback<int, Action<IGameClientCallback>>(
                    (accountId, notification) => notification(_callbackMock.Object))
                .Returns(true);

            MatchWatcherServices services = new MatchWatcherServices();
            services.PlayerChannel = _playerChannelMock.Object;
            services.StateProjection = _stateProjectionMock.Object;
            services.Logger = new Mock<ILogger>().Object;

            new MatchDisconnectionWatcher(_invitationRegistry, _matchRoomRegistry, services).Watch();
        }

        [TestMethod]
        public void PlayerDisconnects_DuringMatch_RivalWinsAndMatchCloses()
        {
            CribbageMatch match = _matchRoomRegistry.StartMatch(LeavingAccountId, RivalAccountId);

            RaiseConnectionChanged(LeavingAccountId, false);

            Assert.AreEqual(MatchPhase.Finished, match.Phase);
            Assert.AreEqual(RivalAccountId, match.WinnerAccountId);
            Assert.IsFalse(_matchRoomRegistry.IsInMatch(LeavingAccountId));
            Assert.IsFalse(_matchRoomRegistry.IsInMatch(RivalAccountId));
        }

        [TestMethod]
        public void PlayerDisconnects_DuringMatch_NotifiesRivalDisconnection()
        {
            _matchRoomRegistry.StartMatch(LeavingAccountId, RivalAccountId);

            RaiseConnectionChanged(LeavingAccountId, false);

            _playerChannelMock.Verify(
                channel => channel.TrySendNotification(RivalAccountId, It.IsAny<Action<IGameClientCallback>>()),
                Times.Once);
            _callbackMock.Verify(
                callback => callback.MatchStateChanged(MatchEventType.RivalDisconnected, It.IsAny<MatchState>()),
                Times.Once);
        }

        [TestMethod]
        public void PlayerConnects_DuringMatch_KeepsMatch()
        {
            CribbageMatch match = _matchRoomRegistry.StartMatch(LeavingAccountId, RivalAccountId);

            RaiseConnectionChanged(LeavingAccountId, true);

            Assert.AreEqual(MatchPhase.Discarding, match.Phase);
            Assert.IsTrue(_matchRoomRegistry.IsInMatch(LeavingAccountId));
            VerifyNoNotificationSent();
        }

        [TestMethod]
        public void PlayerDisconnects_WithoutMatchOrInvitations_SendsNothing()
        {
            RaiseConnectionChanged(LeavingAccountId, false);

            VerifyNoNotificationSent();
        }

        [TestMethod]
        public void PlayerDisconnects_FinishedMatch_ClosesMatchWithoutChangingWinner()
        {
            CribbageMatch match = _matchRoomRegistry.StartMatch(LeavingAccountId, RivalAccountId);
            match.AbandonBy(RivalAccountId);

            RaiseConnectionChanged(LeavingAccountId, false);

            Assert.AreEqual(LeavingAccountId, match.WinnerAccountId);
            Assert.IsFalse(_matchRoomRegistry.IsInMatch(LeavingAccountId));
            VerifyNoNotificationSent();
        }

        [TestMethod]
        public void PlayerDisconnects_WithPendingInvitations_DropsThemAndNotifiesCounterparts()
        {
            PendingInvitation ignoredInvitation;
            _invitationRegistry.TryCreate(LeavingAccountId, InvitedAccountId, out ignoredInvitation);
            _invitationRegistry.TryCreate(InvitingAccountId, LeavingAccountId, out ignoredInvitation);
            _invitationRegistry.TryCreate(InvitedAccountId, InvitingAccountId, out ignoredInvitation);

            RaiseConnectionChanged(LeavingAccountId, false);

            VerifyNotificationSentTo(InvitedAccountId);
            VerifyNotificationSentTo(InvitingAccountId);
            Assert.AreEqual(0, _invitationRegistry.TakeAllFor(LeavingAccountId).Count);
            Assert.AreEqual(1, _invitationRegistry.TakeAllFor(InvitedAccountId).Count);
        }

        private void RaiseConnectionChanged(int accountId, bool isOnline)
        {
            _playerChannelMock.Raise(
                channel => channel.PlayerConnectionChanged += null,
                new PlayerConnectionChangedEventArgs(accountId, isOnline));
        }

        private void VerifyNotificationSentTo(int accountId)
        {
            _playerChannelMock.Verify(
                channel => channel.TrySendNotification(accountId, It.IsAny<Action<IGameClientCallback>>()),
                Times.Once);
        }

        private void VerifyNoNotificationSent()
        {
            _playerChannelMock.Verify(
                channel => channel.TrySendNotification(It.IsAny<int>(), It.IsAny<Action<IGameClientCallback>>()),
                Times.Never);
        }
    }
}
