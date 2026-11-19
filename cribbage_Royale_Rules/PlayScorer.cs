using System.Collections.Generic;
using CribbageRoyale.Contracts.DataContracts;

namespace CribbageRoyale.Rules
{
    public static class PlayScorer
    {
        private const int FifteenCount = 15;
        private const int MaximumCount = 31;
        private const int CountPoints = 2;
        private const int PairPoints = 2;
        private const int TripletPoints = 6;
        private const int QuadrupletPoints = 12;
        private const int MinimumRunLength = 3;

        public static int ScorePlay(List<Card> playedCards, int peggingCount)
        {
            int points = 0;

            if (playedCards != null && playedCards.Count > 0)
            {
                points = ScoreCount(peggingCount)
                    + ScorePairs(playedCards)
                    + ScoreRun(playedCards);
            }

            return points;
        }

        private static int ScoreCount(int peggingCount)
        {
            int points = 0;

            if (peggingCount == FifteenCount || peggingCount == MaximumCount)
            {
                points = CountPoints;
            }

            return points;
        }

        private static int ScorePairs(List<Card> playedCards)
        {
            int points;

            switch (CountTrailingSameRank(playedCards))
            {
                case 2:
                    points = PairPoints;
                    break;
                case 3:
                    points = TripletPoints;
                    break;
                case 4:
                    points = QuadrupletPoints;
                    break;
                default:
                    points = 0;
                    break;
            }

            return points;
        }

        private static int CountTrailingSameRank(List<Card> playedCards)
        {
            int count = 0;
            int position = playedCards.Count - 1;
            CardRank lastRank = playedCards[position].Rank;

            while (position >= 0 && playedCards[position].Rank == lastRank)
            {
                count++;
                position--;
            }

            return count;
        }

        private static int ScoreRun(List<Card> playedCards)
        {
            int points = 0;

            for (int length = MinimumRunLength; length <= playedCards.Count; length++)
            {
                if (IsRun(playedCards, length))
                {
                    points = length;
                }
            }

            return points;
        }

        private static bool IsRun(List<Card> playedCards, int length)
        {
            List<int> ranks = GetLastRanks(playedCards, length);
            ranks.Sort();

            return IsConsecutive(ranks);
        }

        private static List<int> GetLastRanks(List<Card> playedCards, int length)
        {
            List<int> ranks = new List<int>();

            for (int position = playedCards.Count - length; position < playedCards.Count; position++)
            {
                ranks.Add((int)playedCards[position].Rank);
            }

            return ranks;
        }

        private static bool IsConsecutive(List<int> ranks)
        {
            bool isConsecutive = true;

            for (int position = 1; position < ranks.Count; position++)
            {
                if (ranks[position] != ranks[position - 1] + 1)
                {
                    isConsecutive = false;
                }
            }

            return isConsecutive;
        }
    }
}