using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using CribbageRoyale.Contracts.DataContracts;

namespace CribbageRoyale.Rules
{
    public class Deck
    {
        private const int TotalCards = 52;

        private static readonly RNGCryptoServiceProvider RandomGenerator =
            new RNGCryptoServiceProvider();

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

        public bool TryCut(out Card cutCard)
        {
            bool cut = false;
            cutCard = null;
            if (RemainingCards > 0)
            {
                cutCard = _cards[_nextCardIndex];
                _nextCardIndex++;
                cut = true;
            }
            return cut;
        }

        private void Shuffle()
        {
            for (int position = _cards.Count - 1; position > 0; position--)
            {
                int swapIndex = NextIndex(position + 1);
                Card temporary = _cards[position];
                _cards[position] = _cards[swapIndex];
                _cards[swapIndex] = temporary;
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

        private static int NextIndex(int exclusiveMaximum)
        {
            byte[] buffer = new byte[4];
            RandomGenerator.GetBytes(buffer);
            int value = BitConverter.ToInt32(buffer, 0) & int.MaxValue;
            return value % exclusiveMaximum;
        }
    }
}