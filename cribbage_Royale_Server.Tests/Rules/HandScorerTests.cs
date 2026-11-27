using System.Collections.Generic;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CribbageRoyale.Server.Tests.Rules
{
    [TestClass]
    public class HandScorerTests
    {
        [TestMethod]
        public void ScoreHand_HighestPossibleHand_Returns29()
        {
            List<Card> hand = new List<Card>
            {
                BuildCard(CardRank.Jack, CardSuit.Hearts),
                BuildCard(CardRank.Five, CardSuit.Clubs),
                BuildCard(CardRank.Five, CardSuit.Diamonds),
                BuildCard(CardRank.Five, CardSuit.Spades)
            };
            Card cutCard = BuildCard(CardRank.Five, CardSuit.Hearts);

            int points = HandScorer.ScoreHand(hand, cutCard, false);

            Assert.AreEqual(29, points);
        }

        [TestMethod]
        public void ScoreHand_HandWithoutPoints_ReturnsZero()
        {
            List<Card> hand = new List<Card>
            {
                BuildCard(CardRank.Two, CardSuit.Hearts),
                BuildCard(CardRank.Four, CardSuit.Diamonds),
                BuildCard(CardRank.Six, CardSuit.Clubs),
                BuildCard(CardRank.Eight, CardSuit.Spades)
            };
            Card cutCard = BuildCard(CardRank.Ten, CardSuit.Hearts);

            int points = HandScorer.ScoreHand(hand, cutCard, false);

            Assert.AreEqual(0, points);
        }

        [TestMethod]
        public void ScoreHand_OnlyFifteens_Returns4()
        {
            List<Card> hand = new List<Card>
            {
                BuildCard(CardRank.Seven, CardSuit.Hearts),
                BuildCard(CardRank.Eight, CardSuit.Diamonds),
                BuildCard(CardRank.Two, CardSuit.Clubs),
                BuildCard(CardRank.Three, CardSuit.Spades)
            };
            Card cutCard = BuildCard(CardRank.King, CardSuit.Hearts);

            int points = HandScorer.ScoreHand(hand, cutCard, false);

            Assert.AreEqual(4, points);
        }

        [TestMethod]
        public void ScoreHand_OnlyPair_Returns2()
        {
            List<Card> hand = new List<Card>
            {
                BuildCard(CardRank.Two, CardSuit.Hearts),
                BuildCard(CardRank.Two, CardSuit.Diamonds),
                BuildCard(CardRank.Seven, CardSuit.Clubs),
                BuildCard(CardRank.Nine, CardSuit.Spades)
            };
            Card cutCard = BuildCard(CardRank.King, CardSuit.Hearts);

            int points = HandScorer.ScoreHand(hand, cutCard, false);

            Assert.AreEqual(2, points);
        }

        [TestMethod]
        public void ScoreHand_ThreeOfAKind_Returns6()
        {
            List<Card> hand = new List<Card>
            {
                BuildCard(CardRank.Four, CardSuit.Hearts),
                BuildCard(CardRank.Four, CardSuit.Diamonds),
                BuildCard(CardRank.Four, CardSuit.Clubs),
                BuildCard(CardRank.Nine, CardSuit.Spades)
            };
            Card cutCard = BuildCard(CardRank.King, CardSuit.Hearts);

            int points = HandScorer.ScoreHand(hand, cutCard, false);

            Assert.AreEqual(6, points);
        }

        [TestMethod]
        public void ScoreHand_FourOfAKind_Returns12()
        {
            List<Card> hand = new List<Card>
            {
                BuildCard(CardRank.Four, CardSuit.Hearts),
                BuildCard(CardRank.Four, CardSuit.Diamonds),
                BuildCard(CardRank.Four, CardSuit.Clubs),
                BuildCard(CardRank.Four, CardSuit.Spades)
            };
            Card cutCard = BuildCard(CardRank.Nine, CardSuit.Hearts);

            int points = HandScorer.ScoreHand(hand, cutCard, false);

            Assert.AreEqual(12, points);
        }

        [TestMethod]
        public void ScoreHand_OnlyRunOfThree_Returns3()
        {
            List<Card> hand = new List<Card>
            {
                BuildCard(CardRank.Jack, CardSuit.Hearts),
                BuildCard(CardRank.Queen, CardSuit.Diamonds),
                BuildCard(CardRank.King, CardSuit.Clubs),
                BuildCard(CardRank.Ace, CardSuit.Spades)
            };
            Card cutCard = BuildCard(CardRank.Three, CardSuit.Diamonds);

            int points = HandScorer.ScoreHand(hand, cutCard, false);

            Assert.AreEqual(3, points);
        }

        [TestMethod]
        public void ScoreHand_OnlyFourCardFlush_Returns4()
        {
            List<Card> hand = BuildHeartsFlushHand();
            Card cutCard = BuildCard(CardRank.King, CardSuit.Spades);

            int points = HandScorer.ScoreHand(hand, cutCard, false);

            Assert.AreEqual(4, points);
        }

        [TestMethod]
        public void ScoreHand_OnlyFiveCardFlush_Returns5()
        {
            List<Card> hand = BuildHeartsFlushHand();
            Card cutCard = BuildCard(CardRank.King, CardSuit.Hearts);

            int points = HandScorer.ScoreHand(hand, cutCard, false);

            Assert.AreEqual(5, points);
        }

        [TestMethod]
        public void ScoreHand_FourCardFlushInCrib_ReturnsZero()
        {
            List<Card> hand = BuildHeartsFlushHand();
            Card cutCard = BuildCard(CardRank.King, CardSuit.Spades);

            int points = HandScorer.ScoreHand(hand, cutCard, true);

            Assert.AreEqual(0, points);
        }

        [TestMethod]
        public void ScoreHand_OnlyNobs_Returns1()
        {
            List<Card> hand = new List<Card>
            {
                BuildCard(CardRank.Jack, CardSuit.Hearts),
                BuildCard(CardRank.Two, CardSuit.Diamonds),
                BuildCard(CardRank.Four, CardSuit.Clubs),
                BuildCard(CardRank.Six, CardSuit.Spades)
            };
            Card cutCard = BuildCard(CardRank.King, CardSuit.Hearts);

            int points = HandScorer.ScoreHand(hand, cutCard, false);

            Assert.AreEqual(1, points);
        }

        [TestMethod]
        public void ScoreHand_PairPlusRun_Returns8()
        {
            List<Card> hand = new List<Card>
            {
                BuildCard(CardRank.Jack, CardSuit.Hearts),
                BuildCard(CardRank.Queen, CardSuit.Diamonds),
                BuildCard(CardRank.Queen, CardSuit.Clubs),
                BuildCard(CardRank.King, CardSuit.Spades)
            };
            Card cutCard = BuildCard(CardRank.Ace, CardSuit.Diamonds);

            int points = HandScorer.ScoreHand(hand, cutCard, false);

            Assert.AreEqual(8, points);
        }

        [TestMethod]
        public void ScoreHand_FifteensPlusPair_Returns6()
        {
            List<Card> hand = new List<Card>
            {
                BuildCard(CardRank.Seven, CardSuit.Hearts),
                BuildCard(CardRank.Eight, CardSuit.Diamonds),
                BuildCard(CardRank.Seven, CardSuit.Clubs),
                BuildCard(CardRank.Two, CardSuit.Spades)
            };
            Card cutCard = BuildCard(CardRank.King, CardSuit.Spades);

            int points = HandScorer.ScoreHand(hand, cutCard, false);

            Assert.AreEqual(6, points);
        }

        [TestMethod]
        public void ScoreHand_RunOfFive_Returns9()
        {
            List<Card> hand = new List<Card>
            {
                BuildCard(CardRank.Three, CardSuit.Hearts),
                BuildCard(CardRank.Four, CardSuit.Diamonds),
                BuildCard(CardRank.Five, CardSuit.Clubs),
                BuildCard(CardRank.Six, CardSuit.Spades)
            };
            Card cutCard = BuildCard(CardRank.Seven, CardSuit.Hearts);

            int points = HandScorer.ScoreHand(hand, cutCard, false);

            Assert.AreEqual(9, points);
        }

        private static List<Card> BuildHeartsFlushHand()
        {
            List<Card> hand = new List<Card>
            {
                BuildCard(CardRank.Ace, CardSuit.Hearts),
                BuildCard(CardRank.Two, CardSuit.Hearts),
                BuildCard(CardRank.Eight, CardSuit.Hearts),
                BuildCard(CardRank.Nine, CardSuit.Hearts)
            };

            return hand;
        }

        private static Card BuildCard(CardRank rank, CardSuit suit)
        {
            Card card = new Card();
            card.Rank = rank;
            card.Suit = suit;

            return card;
        }
    }
}
