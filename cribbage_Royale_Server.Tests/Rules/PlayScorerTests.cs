using System.Collections.Generic;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CribbageRoyale.Server.Tests.Rules
{
    [TestClass]
    public class PlayScorerTests
    {
        [TestMethod]
        public void ScorePlay_CountReachesFifteen_Returns2()
        {
            List<Card> playedCards = new List<Card>
            {
                BuildCard(CardRank.Seven, CardSuit.Hearts),
                BuildCard(CardRank.Eight, CardSuit.Diamonds)
            };

            int points = PlayScorer.ScorePlay(playedCards, 15);

            Assert.AreEqual(2, points);
        }

        [TestMethod]
        public void ScorePlay_CountReachesThirtyOne_Returns2()
        {
            List<Card> playedCards = new List<Card>
            {
                BuildCard(CardRank.King, CardSuit.Hearts),
                BuildCard(CardRank.Queen, CardSuit.Diamonds),
                BuildCard(CardRank.Five, CardSuit.Clubs),
                BuildCard(CardRank.Six, CardSuit.Spades)
            };

            int points = PlayScorer.ScorePlay(playedCards, 31);

            Assert.AreEqual(2, points);
        }

        [TestMethod]
        public void ScorePlay_Pair_Returns2()
        {
            List<Card> playedCards = new List<Card>
            {
                BuildCard(CardRank.Five, CardSuit.Hearts),
                BuildCard(CardRank.Five, CardSuit.Diamonds)
            };

            int points = PlayScorer.ScorePlay(playedCards, 10);

            Assert.AreEqual(2, points);
        }

        [TestMethod]
        public void ScorePlay_ThreeOfAKind_Returns6()
        {
            List<Card> playedCards = new List<Card>
            {
                BuildCard(CardRank.Eight, CardSuit.Hearts),
                BuildCard(CardRank.Eight, CardSuit.Diamonds),
                BuildCard(CardRank.Eight, CardSuit.Clubs)
            };

            int points = PlayScorer.ScorePlay(playedCards, 24);

            Assert.AreEqual(6, points);
        }

        [TestMethod]
        public void ScorePlay_FourOfAKind_Returns12()
        {
            List<Card> playedCards = new List<Card>
            {
                BuildCard(CardRank.Two, CardSuit.Hearts),
                BuildCard(CardRank.Two, CardSuit.Diamonds),
                BuildCard(CardRank.Two, CardSuit.Clubs),
                BuildCard(CardRank.Two, CardSuit.Spades)
            };

            int points = PlayScorer.ScorePlay(playedCards, 8);

            Assert.AreEqual(12, points);
        }

        [TestMethod]
        public void ScorePlay_RunOfThree_Returns3()
        {
            List<Card> playedCards = new List<Card>
            {
                BuildCard(CardRank.Five, CardSuit.Hearts),
                BuildCard(CardRank.Six, CardSuit.Diamonds),
                BuildCard(CardRank.Seven, CardSuit.Clubs)
            };

            int points = PlayScorer.ScorePlay(playedCards, 18);

            Assert.AreEqual(3, points);
        }

        [TestMethod]
        public void ScorePlay_RunOfFour_Returns4()
        {
            List<Card> playedCards = new List<Card>
            {
                BuildCard(CardRank.Five, CardSuit.Hearts),
                BuildCard(CardRank.Six, CardSuit.Diamonds),
                BuildCard(CardRank.Seven, CardSuit.Clubs),
                BuildCard(CardRank.Eight, CardSuit.Spades)
            };

            int points = PlayScorer.ScorePlay(playedCards, 26);

            Assert.AreEqual(4, points);
        }

        [TestMethod]
        public void ScorePlay_RunOfFive_Returns5()
        {
            List<Card> playedCards = new List<Card>
            {
                BuildCard(CardRank.Three, CardSuit.Hearts),
                BuildCard(CardRank.Four, CardSuit.Diamonds),
                BuildCard(CardRank.Five, CardSuit.Clubs),
                BuildCard(CardRank.Six, CardSuit.Spades),
                BuildCard(CardRank.Seven, CardSuit.Hearts)
            };

            int points = PlayScorer.ScorePlay(playedCards, 25);

            Assert.AreEqual(5, points);
        }

        [TestMethod]
        public void ScorePlay_RunPlayedOutOfOrder_Returns3()
        {
            List<Card> playedCards = new List<Card>
            {
                BuildCard(CardRank.Seven, CardSuit.Hearts),
                BuildCard(CardRank.Five, CardSuit.Diamonds),
                BuildCard(CardRank.Six, CardSuit.Clubs)
            };

            int points = PlayScorer.ScorePlay(playedCards, 18);

            Assert.AreEqual(3, points);
        }

        [TestMethod]
        public void ScorePlay_RunBrokenByRepeatedRank_ReturnsZero()
        {
            List<Card> playedCards = new List<Card>
            {
                BuildCard(CardRank.Five, CardSuit.Hearts),
                BuildCard(CardRank.Six, CardSuit.Diamonds),
                BuildCard(CardRank.Seven, CardSuit.Clubs),
                BuildCard(CardRank.Six, CardSuit.Spades)
            };

            int points = PlayScorer.ScorePlay(playedCards, 24);

            Assert.AreEqual(0, points);
        }

        [TestMethod]
        public void ScorePlay_PlayWithoutPoints_ReturnsZero()
        {
            List<Card> playedCards = new List<Card>
            {
                BuildCard(CardRank.King, CardSuit.Hearts),
                BuildCard(CardRank.Two, CardSuit.Diamonds)
            };

            int points = PlayScorer.ScorePlay(playedCards, 12);

            Assert.AreEqual(0, points);
        }

        [TestMethod]
        public void ScorePlay_ThreeOfAKindReachingFifteen_Returns8()
        {
            List<Card> playedCards = new List<Card>
            {
                BuildCard(CardRank.Five, CardSuit.Hearts),
                BuildCard(CardRank.Five, CardSuit.Diamonds),
                BuildCard(CardRank.Five, CardSuit.Clubs)
            };

            int points = PlayScorer.ScorePlay(playedCards, 15);

            Assert.AreEqual(8, points);
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
