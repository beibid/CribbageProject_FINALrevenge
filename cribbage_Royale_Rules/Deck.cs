using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using CribbageRoyale.Contracts.DataContracts;

namespace CribbageRoyale.Rules
{
    public class Deck
    {
        private const int TotalCards = 52;
        private const int RandomBufferLength = 4;

        private readonly List<Card> _cards;

        private int _nextCardIndex;

        public Deck()
        {
            _cards = BuildOrderedCards();
            _nextCardIndex = 0;
            Shuffle();
        }

        public int RemainingCards
        {
            get { return _cards.Count - _nextCardIndex; }
        }

        public List<Card> Deal(int count)
        {
            List<Card> dealt = new List<Card>();

            if (count > 0 && count <= RemainingCards)
            {
                for (int position = 0; position < count; position++)
                {
                    dealt.Add(_cards[_nextCardIndex]);
                    _nextCardIndex++;
                }
            }

            return dealt;
        }

        public Card TakeCutCard()
        {
            Card cutCard = _cards[_nextCardIndex];
            _nextCardIndex++;

            return cutCard;
        }

        private void Shuffle()
        {
            using (RandomNumberGenerator generator = RandomNumberGenerator.Create())
            {
                for (int position = _cards.Count - 1; position > 0; position--)
                {
                    int swapIndex = NextIndex(generator, position + 1);
                    Card temporary = _cards[position];
                    _cards[position] = _cards[swapIndex];
                    _cards[swapIndex] = temporary;
                }
            }
        }

        private static List<Card> BuildOrderedCards()
        {
            List<Card> cards = new List<Card>(TotalCards);

            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
                {
                    cards.Add(new Card { Rank = rank, Suit = suit });
                }
            }

            return cards;
        }

        private static int NextIndex(RandomNumberGenerator generator, int exclusiveMaximum)
        {
            byte[] randomBytes = new byte[RandomBufferLength];
            generator.GetBytes(randomBytes);

            int value = BitConverter.ToInt32(randomBytes, 0) & int.MaxValue;

            return value % exclusiveMaximum;
        }
    }
}