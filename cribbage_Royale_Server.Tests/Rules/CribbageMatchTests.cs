using System;
using System.Collections.Generic;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CribbageRoyale.Server.Tests.Rules
{
    [TestClass]
    public class CribbageMatchTests
    {
        private const long MatchId = 1;
        private const int FirstAccountId = 7;
        private const int SecondAccountId = 8;
        private const int CardsPerPlayer = 6;
        private const int CardsAfterDiscard = 4;
        private const int CribSize = 4;
        private const int MaximumCount = 31;
        private const int FaceCardValue = 10;
        private const int WinningScore = 61;
        private const int MaximumActions = 5000;
        private const int FullMatchRepetitions = 20;

        [TestMethod]
        public void AcceptDiscard_CardNotInHand_IsRejected()
        {
            CribbageMatch match = new CribbageMatch(MatchId, FirstAccountId, SecondAccountId);
            List<Card> hand = match.GetHand(FirstAccountId);
            List<Card> discard = new List<Card> { hand[0], BuildCardMissingFrom(hand) };

            bool isAccepted = match.AcceptDiscard(FirstAccountId, discard);

            Assert.IsFalse(isAccepted);
            Assert.AreEqual(CardsPerPlayer, match.GetHand(FirstAccountId).Count);
            Assert.IsFalse(match.HasDiscarded(FirstAccountId));
            Assert.AreEqual(0, match.CribCardCount);
        }

        [TestMethod]
        public void AcceptDiscard_SecondDiscardBySamePlayer_IsRejected()
        {
            CribbageMatch match = new CribbageMatch(MatchId, FirstAccountId, SecondAccountId);
            DiscardFirstTwoCards(match, FirstAccountId);
            List<Card> hand = match.GetHand(FirstAccountId);
            List<Card> secondDiscard = new List<Card> { hand[0], hand[1] };

            bool isAccepted = match.AcceptDiscard(FirstAccountId, secondDiscard);

            Assert.IsFalse(isAccepted);
            Assert.AreEqual(CardsAfterDiscard, match.GetHand(FirstAccountId).Count);
            Assert.AreEqual(2, match.CribCardCount);
        }

        [TestMethod]
        public void AcceptDiscard_BothPlayersDiscard_FillsCribAndRevealsCutCard()
        {
            CribbageMatch match = new CribbageMatch(MatchId, FirstAccountId, SecondAccountId);

            DiscardFirstTwoCards(match, FirstAccountId);
            DiscardFirstTwoCards(match, SecondAccountId);

            Assert.AreEqual(CribSize, match.CribCardCount);
            Assert.IsTrue(match.HasCutCard);
            Assert.AreEqual(MatchPhase.Playing, match.Phase);
        }

        [TestMethod]
        public void AcceptPlay_OutOfTurn_IsRejected()
        {
            CribbageMatch match = BuildMatchInPlay();
            int waitingAccountId = match.GetOpponentId(match.TurnAccountId);
            Card card = match.GetHand(waitingAccountId)[0];

            bool isAccepted = match.AcceptPlay(waitingAccountId, card);

            Assert.IsFalse(isAccepted);
            Assert.AreEqual(0, match.PeggingCount);
            Assert.AreEqual(CardsAfterDiscard, match.GetHand(waitingAccountId).Count);
        }

        [TestMethod]
        public void AcceptPlay_CardNotInHand_IsRejected()
        {
            CribbageMatch match = BuildMatchInPlay();
            int turnAccountId = match.TurnAccountId;
            Card missingCard = BuildCardMissingFrom(match.GetHand(turnAccountId));

            bool isAccepted = match.AcceptPlay(turnAccountId, missingCard);

            Assert.IsFalse(isAccepted);
            Assert.AreEqual(0, match.PeggingCount);
            Assert.AreEqual(turnAccountId, match.TurnAccountId);
        }

        [TestMethod]
        public void AcceptPlay_SameCardTwice_IsRejected()
        {
            CribbageMatch match = BuildMatchInPlay();
            int firstPlayerId = match.TurnAccountId;
            Card repeatedCard = match.GetHand(firstPlayerId)[0];
            match.AcceptPlay(firstPlayerId, repeatedCard);
            int secondPlayerId = match.TurnAccountId;
            match.AcceptPlay(secondPlayerId, match.GetHand(secondPlayerId)[0]);

            bool isAccepted = match.AcceptPlay(firstPlayerId, repeatedCard);

            Assert.AreEqual(firstPlayerId, match.TurnAccountId);
            Assert.IsFalse(isAccepted);
        }

        [TestMethod]
        public void AcceptPlay_FirstPlay_NonDealerStarts()
        {
            CribbageMatch match = BuildMatchInPlay();

            int turnAccountId = match.TurnAccountId;

            Assert.AreEqual(SecondAccountId, turnAccountId);
            Assert.AreEqual(FirstAccountId, match.DealerAccountId);
        }

        [TestMethod]
        public void PlayHand_WithoutWinner_StartsNewHandWithRotatedDealer()
        {
            CribbageMatch match = new CribbageMatch(MatchId, FirstAccountId, SecondAccountId);

            PlayUntilDealerChanges(match);

            Assert.AreEqual(SecondAccountId, match.DealerAccountId);
            Assert.AreEqual(MatchPhase.Discarding, match.Phase);
            Assert.AreEqual(CardsPerPlayer, match.GetHand(FirstAccountId).Count);
            Assert.AreEqual(CardsPerPlayer, match.GetHand(SecondAccountId).Count);
            Assert.AreEqual(0, match.CribCardCount);
            Assert.IsFalse(match.HasCutCard);
        }

        [TestMethod]
        public void PlayMatch_UntilSomeoneReachesSixtyOne_FinishesWithWinner()
        {
            for (int repetition = 0; repetition < FullMatchRepetitions; repetition++)
            {
                CribbageMatch match = new CribbageMatch(MatchId, FirstAccountId, SecondAccountId);

                PlayUntilFinished(match);

                int loserAccountId = match.GetOpponentId(match.WinnerAccountId);
                Assert.AreEqual(MatchPhase.Finished, match.Phase);
                Assert.IsTrue(match.IsParticipant(match.WinnerAccountId));
                Assert.AreEqual(WinningScore, match.GetScore(match.WinnerAccountId));
                Assert.IsTrue(match.GetScore(loserAccountId) < WinningScore);
            }
        }

        [TestMethod]
        public void AbandonBy_MatchInProgress_RivalWinsAndMatchFinishes()
        {
            CribbageMatch match = new CribbageMatch(MatchId, FirstAccountId, SecondAccountId);

            bool isAbandoned = match.AbandonBy(FirstAccountId);

            Assert.IsTrue(isAbandoned);
            Assert.AreEqual(SecondAccountId, match.WinnerAccountId);
            Assert.AreEqual(MatchPhase.Finished, match.Phase);
        }

        [TestMethod]
        public void AbandonBy_FinishedMatch_KeepsWinner()
        {
            CribbageMatch match = new CribbageMatch(MatchId, FirstAccountId, SecondAccountId);
            match.AbandonBy(FirstAccountId);

            bool isAbandoned = match.AbandonBy(SecondAccountId);

            Assert.IsFalse(isAbandoned);
            Assert.AreEqual(SecondAccountId, match.WinnerAccountId);
        }

        private static CribbageMatch BuildMatchInPlay()
        {
            CribbageMatch match = new CribbageMatch(MatchId, FirstAccountId, SecondAccountId);
            DiscardFirstTwoCards(match, FirstAccountId);
            DiscardFirstTwoCards(match, SecondAccountId);

            return match;
        }

        private static void PlayUntilDealerChanges(CribbageMatch match)
        {
            int startingDealerId = match.DealerAccountId;

            for (int action = 0; action < MaximumActions && match.DealerAccountId == startingDealerId; action++)
            {
                PlayNextAction(match);
            }
        }

        private static void PlayUntilFinished(CribbageMatch match)
        {
            for (int action = 0; action < MaximumActions && match.Phase != MatchPhase.Finished; action++)
            {
                PlayNextAction(match);
            }
        }

        private static void PlayNextAction(CribbageMatch match)
        {
            if (match.Phase == MatchPhase.Discarding)
            {
                DiscardPendingPlayers(match);
            }
            else if (match.Phase == MatchPhase.Playing)
            {
                PlayFirstPlayableCard(match);
            }
        }

        private static void DiscardPendingPlayers(CribbageMatch match)
        {
            if (!match.HasDiscarded(FirstAccountId))
            {
                Assert.IsTrue(DiscardFirstTwoCards(match, FirstAccountId));
            }

            if (!match.HasDiscarded(SecondAccountId))
            {
                Assert.IsTrue(DiscardFirstTwoCards(match, SecondAccountId));
            }
        }

        private static void PlayFirstPlayableCard(CribbageMatch match)
        {
            int turnAccountId = match.TurnAccountId;
            List<Card> playableCards = new List<Card>();

            foreach (Card card in match.GetHand(turnAccountId))
            {
                if (match.PeggingCount + GetCardValue(card) <= MaximumCount)
                {
                    playableCards.Add(card);
                }
            }

            Assert.AreNotEqual(0, playableCards.Count, "The player in turn has no playable card.");
            Assert.IsTrue(match.AcceptPlay(turnAccountId, playableCards[0]));
            Assert.IsTrue(match.PeggingCount <= MaximumCount);
        }

        private static int GetCardValue(Card card)
        {
            return Math.Min((int)card.Rank, FaceCardValue);
        }

        private static bool DiscardFirstTwoCards(CribbageMatch match, int accountId)
        {
            List<Card> hand = match.GetHand(accountId);
            List<Card> discard = new List<Card> { hand[0], hand[1] };

            return match.AcceptDiscard(accountId, discard);
        }

        private static Card BuildCardMissingFrom(List<Card> hand)
        {
            List<Card> missingCards = new List<Card>();

            foreach (Card card in BuildAllCards())
            {
                if (!ContainsCard(hand, card))
                {
                    missingCards.Add(card);
                }
            }

            return missingCards[0];
        }

        private static List<Card> BuildAllCards()
        {
            List<Card> cards = new List<Card>();

            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
                {
                    cards.Add(new Card { Rank = rank, Suit = suit });
                }
            }

            return cards;
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
    }
}
