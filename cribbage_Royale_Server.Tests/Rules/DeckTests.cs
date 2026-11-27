using System;
using System.Collections.Generic;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CribbageRoyale.Server.Tests.Rules
{
    [TestClass]
    public class DeckTests
    {
        private const int DeckSize = 52;
        private const int CardsPerPlayer = 6;
        private const int CardsAfterDealingTwoHands = 40;
        private const int ShuffleRepetitions = 10;
        private const int SuitKeyFactor = 100;

        [TestMethod]
        public void Constructor_NewDeck_Has52Cards()
        {
            Deck deck = new Deck();

            Assert.AreEqual(DeckSize, deck.RemainingCards);
        }

        [TestMethod]
        public void Constructor_NewDeck_ContainsEachSuitAndRankExactlyOnce()
        {
            Deck deck = new Deck();

            List<int> dealtKeys = BuildCardKeys(deck.Deal(DeckSize));

            CollectionAssert.AllItemsAreUnique(dealtKeys);
            CollectionAssert.AreEquivalent(BuildUnshuffledKeys(), dealtKeys);
        }

        [TestMethod]
        public void Constructor_ShuffledDeck_KeepsSameCardsInDifferentOrder()
        {
            List<int> unshuffledKeys = BuildUnshuffledKeys();
            Deck deck = new Deck();

            List<int> dealtKeys = BuildCardKeys(deck.Deal(DeckSize));

            Assert.AreEqual(DeckSize, dealtKeys.Count);
            CollectionAssert.AreEquivalent(unshuffledKeys, dealtKeys);
            CollectionAssert.AreNotEqual(unshuffledKeys, dealtKeys);
        }

        [TestMethod]
        public void Constructor_TwoIndependentDecks_HaveDifferentOrder()
        {
            for (int repetition = 0; repetition < ShuffleRepetitions; repetition++)
            {
                Deck firstDeck = new Deck();
                Deck secondDeck = new Deck();

                List<int> firstKeys = BuildCardKeys(firstDeck.Deal(DeckSize));
                List<int> secondKeys = BuildCardKeys(secondDeck.Deal(DeckSize));

                CollectionAssert.AreNotEqual(firstKeys, secondKeys);
            }
        }

        [TestMethod]
        public void Deal_SixCardsToEachOfTwoPlayers_Leaves40Cards()
        {
            Deck deck = new Deck();

            deck.Deal(CardsPerPlayer);
            deck.Deal(CardsPerPlayer);

            Assert.AreEqual(CardsAfterDealingTwoHands, deck.RemainingCards);
        }

        [TestMethod]
        public void Deal_SixCardsToEachOfTwoPlayers_DealtCardsAreNoLongerInDeck()
        {
            Deck deck = new Deck();
            List<int> handKeys = BuildCardKeys(deck.Deal(CardsPerPlayer));
            handKeys.AddRange(BuildCardKeys(deck.Deal(CardsPerPlayer)));

            HashSet<int> remainingKeys = new HashSet<int>(BuildCardKeys(deck.Deal(deck.RemainingCards)));

            Assert.AreEqual(CardsAfterDealingTwoHands, remainingKeys.Count);
            Assert.IsFalse(remainingKeys.Overlaps(handKeys));
        }

        [TestMethod]
        public void TakeCutCard_AfterDealingHands_TakesCardFromDeck()
        {
            Deck deck = new Deck();
            List<int> handKeys = BuildCardKeys(deck.Deal(CardsPerPlayer));
            handKeys.AddRange(BuildCardKeys(deck.Deal(CardsPerPlayer)));

            Card cutCard = deck.TakeCutCard();

            Assert.AreEqual(CardsAfterDealingTwoHands - 1, deck.RemainingCards);
            CollectionAssert.DoesNotContain(handKeys, BuildCardKey(cutCard));
        }

        [TestMethod]
        public void Deal_MoreCardsThanRemaining_ReturnsEmptyListAndKeepsDeck()
        {
            Deck deck = new Deck();
            deck.Deal(DeckSize - 2);

            List<Card> dealt = deck.Deal(3);

            Assert.AreEqual(0, dealt.Count);
            Assert.AreEqual(2, deck.RemainingCards);
        }

        private static List<int> BuildUnshuffledKeys()
        {
            List<int> keys = new List<int>();

            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
                {
                    keys.Add(((int)suit * SuitKeyFactor) + (int)rank);
                }
            }

            return keys;
        }

        private static List<int> BuildCardKeys(List<Card> cards)
        {
            List<int> keys = new List<int>();

            foreach (Card card in cards)
            {
                keys.Add(BuildCardKey(card));
            }

            return keys;
        }

        private static int BuildCardKey(Card card)
        {
            return ((int)card.Suit * SuitKeyFactor) + (int)card.Rank;
        }
    }
}
