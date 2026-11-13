using System.Collections.Generic;
using CribbageRoyale.Contracts.DataContracts;

namespace CribbageRoyale.Rules
{
    public class CribbageMatch
    {
        private const int CardsPerPlayer = 6;
        private const int CardsToDiscard = 2;
        private const int CribSize = 4;

        private readonly Deck _deck;
        private readonly int _firstAccountId;
        private readonly int _secondAccountId;
        private readonly Dictionary<int, List<Card>> _hands;
        private readonly Dictionary<int, int> _scores;
        private readonly List<int> _playersWhoDiscarded;
        private readonly List<Card> _crib;

        private MatchPhase _phase;
        private int _stateVersion;
        private Card _cutCard;

        public CribbageMatch(long matchId, int firstAccountId, int secondAccountId)
        {
            MatchId = matchId;
            DealerAccountId = firstAccountId;
            _firstAccountId = firstAccountId;
            _secondAccountId = secondAccountId;
            _deck = new Deck();
            _hands = new Dictionary<int, List<Card>>();
            _scores = new Dictionary<int, int>();
            _playersWhoDiscarded = new List<int>();
            _crib = new List<Card>();
            _phase = MatchPhase.Discarding;
            _stateVersion = 1;
            DealHands();
        }

        public long MatchId { get; }

        public int DealerAccountId { get; }

        public int StateVersion
        {
            get { return _stateVersion; }
        }

        public MatchPhase Phase
        {
            get { return _phase; }
        }

        public bool IsParticipant(int accountId)
        {
            return accountId == _firstAccountId || accountId == _secondAccountId;
        }

        public int GetOpponentId(int accountId)
        {
            int opponentId = _firstAccountId;
            if (accountId == _firstAccountId)
            {
                opponentId = _secondAccountId;
            }
            return opponentId;
        }

        public List<Card> GetHand(int accountId)
        {
            List<Card> hand = new List<Card>();
            if (_hands.ContainsKey(accountId))
            {
                hand.AddRange(_hands[accountId]);
            }
            return hand;
        }

        public int GetScore(int accountId)
        {
            int score = 0;
            if (_scores.ContainsKey(accountId))
            {
                score = _scores[accountId];
            }
            return score;
        }

        public bool HasDiscarded(int accountId)
        {
            return _playersWhoDiscarded.Contains(accountId);
        }

        public bool TryGetCutCard(out Card cutCard)
        {
            cutCard = _cutCard;
            return _cutCard != null;
        }

        public bool TryDiscard(int accountId, List<Card> cards)
        {
            bool discarded = false;
            List<Card> taken;
            if (CanDiscard(accountId, cards))
            {
                if (TryTakeFromHand(accountId, cards, out taken))
                {
                    _crib.AddRange(taken);
                    _playersWhoDiscarded.Add(accountId);
                    RevealCutCardWhenReady();
                    _stateVersion++;
                    discarded = true;
                }
            }
            return discarded;
        }

        private void DealHands()
        {
            _hands[_firstAccountId] = _deck.Deal(CardsPerPlayer);
            _hands[_secondAccountId] = _deck.Deal(CardsPerPlayer);
            _scores[_firstAccountId] = 0;
            _scores[_secondAccountId] = 0;
        }

        private bool CanDiscard(int accountId, List<Card> cards)
        {
            bool allowed = false;
            if (_phase == MatchPhase.Discarding && IsParticipant(accountId))
            {
                if (cards != null && cards.Count == CardsToDiscard)
                {
                    allowed = !HasDiscarded(accountId);
                }
            }
            return allowed;
        }

        private bool TryTakeFromHand(int accountId, List<Card> cards, out List<Card> taken)
        {
            List<Card> remaining = new List<Card>(_hands[accountId]);
            taken = new List<Card>();
            foreach (Card card in cards)
            {
                int position = FindCard(remaining, card);
                if (position >= 0)
                {
                    taken.Add(remaining[position]);
                    remaining.RemoveAt(position);
                }
            }
            bool complete = taken.Count == cards.Count;
            if (complete)
            {
                _hands[accountId] = remaining;
            }
            return complete;
        }

        private void RevealCutCardWhenReady()
        {
            Card cut;
            if (_crib.Count == CribSize)
            {
                if (_deck.TryCut(out cut))
                {
                    _cutCard = cut;
                    _phase = MatchPhase.Playing;
                }
            }
        }

        private static int FindCard(List<Card> cards, Card wanted)
        {
            int found = -1;
            for (int position = 0; position < cards.Count; position++)
            {
                if (found < 0 && IsSameCard(cards[position], wanted))
                {
                    found = position;
                }
            }
            return found;
        }

        private static bool IsSameCard(Card first, Card second)
        {
            return first.Rank == second.Rank && first.Suit == second.Suit;
        }
    }
}