using System;
using System.Collections.Generic;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Rules;
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
        private const long MatchId = 1;
        private const int RivalAccountId = 8;
        private const int CardsPerPlayer = 6;
        private const int CribSize = 4;

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

        [TestMethod]
        public void BuildMatchState_NewMatch_SendsOnlyRivalCardCount()
        {
            CribbageMatch match = new CribbageMatch(MatchId, SenderAccountId, RivalAccountId);
            List<Card> rivalHand = match.GetHand(RivalAccountId);

            MatchState state = _stateProjection.BuildMatchState(match, SenderAccountId);

            Assert.AreEqual(CardsPerPlayer, state.RivalCardCount);
            Assert.AreEqual(CardsPerPlayer, state.OwnHand.Count);
            Assert.IsFalse(ContainsAnyCard(state.OwnHand, rivalHand));
        }

        [TestMethod]
        public void BuildMatchState_NewMatch_SendsOwnHand()
        {
            CribbageMatch match = new CribbageMatch(MatchId, SenderAccountId, RivalAccountId);
            List<Card> ownHand = match.GetHand(SenderAccountId);

            MatchState state = _stateProjection.BuildMatchState(match, SenderAccountId);

            Assert.AreEqual(ownHand.Count, state.OwnHand.Count);
            Assert.IsTrue(ContainsEveryCard(state.OwnHand, ownHand));
        }

        [TestMethod]
        public void BuildMatchState_FullCrib_SendsEmptyCribWithCount()
        {
            CribbageMatch match = new CribbageMatch(MatchId, SenderAccountId, RivalAccountId);
            DiscardFirstTwoCards(match, SenderAccountId);
            DiscardFirstTwoCards(match, RivalAccountId);

            MatchState state = _stateProjection.BuildMatchState(match, SenderAccountId);

            Assert.AreEqual(0, state.CribCards.Count);
            Assert.AreEqual(CribSize, state.CribCardCount);
            Assert.AreSame(match.CutCard, state.CutCard);
        }

        [TestMethod]
        public void BuildMatchState_BeforeCut_SendsNoCutCard()
        {
            CribbageMatch match = new CribbageMatch(MatchId, SenderAccountId, RivalAccountId);

            MatchState state = _stateProjection.BuildMatchState(match, SenderAccountId);

            Assert.IsNull(state.CutCard);
            Assert.AreEqual(0, state.CribCards.Count);
        }

        [TestMethod]
        public void BuildMatchState_AnyPlayer_SendsRivalAndMatchData()
        {
            CribbageMatch match = new CribbageMatch(MatchId, SenderAccountId, RivalAccountId);

            MatchState state = _stateProjection.BuildMatchState(match, RivalAccountId);

            Assert.AreEqual(SenderAccountId, state.Rival.AccountId);
            Assert.AreEqual(MatchId, state.MatchId);
            Assert.AreEqual(match.Phase, state.Phase);
            Assert.AreEqual(match.DealerAccountId, state.DealerAccountId);
        }

        private static void DiscardFirstTwoCards(CribbageMatch match, int accountId)
        {
            List<Card> hand = match.GetHand(accountId);

            match.AcceptDiscard(accountId, new List<Card> { hand[0], hand[1] });
        }

        private static bool ContainsAnyCard(List<Card> cards, List<Card> wantedCards)
        {
            bool isAnyContained = false;

            foreach (Card wanted in wantedCards)
            {
                if (ContainsCard(cards, wanted))
                {
                    isAnyContained = true;
                }
            }

            return isAnyContained;
        }

        private static bool ContainsEveryCard(List<Card> cards, List<Card> wantedCards)
        {
            bool isEveryContained = true;

            foreach (Card wanted in wantedCards)
            {
                if (!ContainsCard(cards, wanted))
                {
                    isEveryContained = false;
                }
            }

            return isEveryContained;
        }

        private static bool ContainsCard(List<Card> cards, Card wanted)
        {
            bool isContained = false;

            foreach (Card card in cards)
            {
                if (card.Rank == wanted.Rank && card.Suit == wanted.Suit)
                {
                    isContained = true;
                }
            }

            return isContained;
        }

        private static AccountReference BuildConnectedAccount()
        {
            AccountReference account = new AccountReference();
            account.AccountId = SenderAccountId;

            return account;
        }
    }
}