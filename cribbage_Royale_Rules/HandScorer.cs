using System.Collections.Generic;
using CribbageRoyale.Contracts.DataContracts;

namespace CribbageRoyale.Rules
{
    public static class HandScorer
    {
        private const int FifteenTotal = 15;
        private const int FifteenPoints = 2;
        private const int PairPoints = 2;
        private const int NobsPoints = 1;
        private const int MinimumRunLength = 3;
        private const int HighestRank = 13;
        private const int FaceCardValue = 10;

        public static int ScoreHand(List<Card> hand, Card cutCard, bool isCrib)
        {
            List<Card> allCards = new List<Card>(hand);
            allCards.Add(cutCard);

            return ScoreFifteens(allCards)
                + ScorePairs(allCards)
                + ScoreRuns(allCards)
                + ScoreFlush(hand, cutCard, isCrib)
                + ScoreNobs(hand, cutCard);
        }

        private static int ScoreFifteens(List<Card> cards)
        {
            int points = 0;
            int combinations = 1 << cards.Count;

            for (int selection = 1; selection < combinations; selection++)
            {
                if (SumOf(cards, selection) == FifteenTotal)
                {
                    points += FifteenPoints;
                }
            }

            return points;
        }

        private static int SumOf(List<Card> cards, int selection)
        {
            int total = 0;

            for (int position = 0; position < cards.Count; position++)
            {
                if ((selection & (1 << position)) != 0)
                {
                    total += GetCardValue(cards[position].Rank);
                }
            }

            return total;
        }

        private static int ScorePairs(List<Card> cards)
        {
            int points = 0;

            for (int first = 0; first < cards.Count; first++)
            {
                for (int second = first + 1; second < cards.Count; second++)
                {
                    points += GetPairPoints(cards[first], cards[second]);
                }
            }

            return points;
        }

        private static int GetPairPoints(Card first, Card second)
        {
            int points = 0;

            if (first.Rank == second.Rank)
            {
                points = PairPoints;
            }

            return points;
        }

        private static int ScoreRuns(List<Card> cards)
        {
            int[] countByRank = BuildRankCounts(cards);
            int points = 0;
            int rank = 1;

            while (rank <= HighestRank)
            {
                int length = GetRunLength(countByRank, rank);

                if (length >= MinimumRunLength)
                {
                    points += length * GetRunMultiplier(countByRank, rank, length);
                }

                rank += GetNextRankStep(length);
            }

            return points;
        }

        private static int GetNextRankStep(int length)
        {
            int step = 1;

            if (length > 1)
            {
                step = length;
            }

            return step;
        }

        private static int[] BuildRankCounts(List<Card> cards)
        {
            int[] countByRank = new int[HighestRank + 1];

            foreach (Card card in cards)
            {
                countByRank[(int)card.Rank]++;
            }

            return countByRank;
        }

        private static int GetRunLength(int[] countByRank, int startRank)
        {
            int length = 0;
            int rank = startRank;

            while (rank <= HighestRank && countByRank[rank] > 0)
            {
                length++;
                rank++;
            }

            return length;
        }

        private static int GetRunMultiplier(int[] countByRank, int startRank, int length)
        {
            int multiplier = 1;

            for (int rank = startRank; rank < startRank + length; rank++)
            {
                multiplier *= countByRank[rank];
            }

            return multiplier;
        }

        private static int ScoreFlush(List<Card> hand, Card cutCard, bool isCrib)
        {
            int points = 0;

            if (hand.Count > 0 && IsSameSuit(hand))
            {
                points = hand.Count;

                if (cutCard.Suit == hand[0].Suit)
                {
                    points++;
                }
            }

            if (isCrib && points <= hand.Count)
            {
                points = 0;
            }

            return points;
        }

        private static bool IsSameSuit(List<Card> hand)
        {
            bool isSameSuit = true;

            foreach (Card card in hand)
            {
                if (card.Suit != hand[0].Suit)
                {
                    isSameSuit = false;
                }
            }

            return isSameSuit;
        }

        private static int ScoreNobs(List<Card> hand, Card cutCard)
        {
            int points = 0;

            foreach (Card card in hand)
            {
                if (card.Rank == CardRank.Jack && card.Suit == cutCard.Suit)
                {
                    points = NobsPoints;
                }
            }

            return points;
        }

        private static int GetCardValue(CardRank rank)
        {
            int value = (int)rank;

            if (value > FaceCardValue)
            {
                value = FaceCardValue;
            }

            return value;
        }
    }
}