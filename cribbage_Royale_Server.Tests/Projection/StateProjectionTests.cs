using System;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Server.Connections;
using CribbageRoyale.Server.Projection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CribbageRoyale.Server.Tests.Projection
{
    [TestClass]
    public class StateProjectionTests
    {
        private const int SenderAccountId = 7;
        private const string MessageText = "Hola";

        private static readonly DateTime SentAtUtc = new DateTime(2026, 11, 12, 18, 0, 0, DateTimeKind.Utc);

        private Mock<IPlayerChannel> _playerChannelMock;
        private StateProjection _stateProjection;

        [TestInitialize]
        public void SetUp()
        {
            _playerChannelMock = new Mock<IPlayerChannel>();
            _stateProjection = new StateProjection(_playerChannelMock.Object);
        }

        [TestMethod]
        public void BuildChatMessage_SenderIsConnected_ReturnsConnectedAccount()
        {
            AccountReference connectedAccount = BuildConnectedAccount();
            _playerChannelMock
                .Setup(channel => channel.TryFindAccount(SenderAccountId, out connectedAccount))
                .Returns(true);

            ChatMessage message = _stateProjection.BuildChatMessage(SenderAccountId, MessageText, SentAtUtc);

            Assert.AreSame(connectedAccount, message.Sender);
        }

        [TestMethod]
        public void BuildChatMessage_SenderIsDisconnected_ReturnsAccountWithSenderId()
        {
            AccountReference notFoundAccount = null;
            _playerChannelMock
                .Setup(channel => channel.TryFindAccount(SenderAccountId, out notFoundAccount))
                .Returns(false);

            ChatMessage message = _stateProjection.BuildChatMessage(SenderAccountId, MessageText, SentAtUtc);

            Assert.AreEqual(SenderAccountId, message.Sender.AccountId);
        }

        [TestMethod]
        public void BuildChatMessage_AnySender_LooksUpSenderOnce()
        {
            AccountReference ignoredAccount;

            _stateProjection.BuildChatMessage(SenderAccountId, MessageText, SentAtUtc);

            _playerChannelMock.Verify(
                channel => channel.TryFindAccount(SenderAccountId, out ignoredAccount), Times.Once);
        }

        [TestMethod]
        public void BuildChatMessage_ValidText_KeepsText()
        {
            string text = MessageText;

            ChatMessage message = _stateProjection.BuildChatMessage(SenderAccountId, text, SentAtUtc);

            Assert.AreEqual(text, message.Text);
        }

        private static AccountReference BuildConnectedAccount()
        {
            AccountReference account = new AccountReference();
            account.AccountId = SenderAccountId;

            return account;
        }
    }
}