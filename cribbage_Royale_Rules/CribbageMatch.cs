using System.Collections.Generic;
using CribbageRoyale.Contracts.DataContracts;

namespace CribbageRoyale.Rules
{
    public class CribbageMatch
    {
        private const int CardsPerPlayer = 6;
        private const int CardsToDiscard = 2;
        private const int CardsPerPlay = 1;
        private const int CribSize = 4;
        private const int MaximumCount = 31;
        private const int FaceCardValue = 10;
        private const int WinningScore = 61;
        private const int GoPoints = 1;
        private const int HisHeelsPoints = 2;

        private readonly Deck _deck;
        private readonly int _firstAccountId;
        private readonly int _secondAccountId;
        private readonly Dictionary<int, List<Card>> _handsByAccountId;
        private readonly Dictionary<int, int> _scoresByAccountId;
        private readonly List<int> _playersWhoDiscarded;
        private readonly List<Card> _crib;
        private readonly List<Card> _playedCards;
        private readonly object _syncRoot = new object();

        private MatchPhase _phase;
        private int _stateVersion;
        private int _turnAccountId;
        private int _peggingCount;
        private Card _cutCard;

        public CribbageMatch(long matchId, int firstAccountId, int secondAccountId)
        {
            MatchId = matchId;
            DealerAccountId = firstAccountId;
            _firstAccountId = firstAccountId;
            _secondAccountId = secondAccountId;
            _deck = new Deck();
            _handsByAccountId = new Dictionary<int, List<Card>>();
            _scoresByAccountId = new Dictionary<int, int>();
            _playersWhoDiscarded = new List<int>();
            _crib = new List<Card>();
            _playedCards = new List<Card>();
            _phase = MatchPhase.Discarding;
            _stateVersion = 1;
            _turnAccountId = 0;
            _peggingCount = 0;
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

        public int TurnAccountId
        {
            get { return _turnAccountId; }
        }

        public int PeggingCount
        {
            get { return _peggingCount; }
        }

        public bool HasCutCard
        {
            get { return _cutCard != null; }
        }

        public Card CutCard
        {
            get { return _cutCard; }
        }

        public int CribCardCount
        {
            get
            {
                int count;

                lock (_syncRoot)
                {
                    count = _crib.Count;
                }

                return count;
            }
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

            lock (_syncRoot)
            {
                if (_handsByAccountId.ContainsKey(accountId))
                {
                    hand.AddRange(_handsByAccountId[accountId]);
                }
            }

            return hand;
        }

        public List<Card> GetPlayedCards()
        {
            List<Card> played = new List<Card>();

            lock (_syncRoot)
            {
                played.AddRange(_playedCards);
            }

            return played;
        }

        public int GetScore(int accountId)
        {
            int score = 0;

            lock (_syncRoot)
            {
                if (_scoresByAccountId.ContainsKey(accountId))
                {
                    score = _scoresByAccountId[accountId];
                }
            }

            return score;
        }

        public bool HasDiscarded(int accountId)
        {
            bool hasDiscarded;

            lock (_syncRoot)
            {
                hasDiscarded = _playersWhoDiscarded.Contains(accountId);
            }

            return hasDiscarded;
        }

        public bool AcceptDiscard(int accountId, List<Card> cards)
        {
            bool isAccepted = false;

            lock (_syncRoot)
            {
                if (IsDiscardAllowed(accountId, cards))
                {
                    isAccepted = MoveToCrib(accountId, cards);
                }
            }

            return isAccepted;
        }

        public bool AcceptPlay(int accountId, Card card)
        {
            bool isAccepted = false;

            lock (_syncRoot)
            {
                if (IsPlayAllowed(accountId, card))
                {
                    isAccepted = PlayFromHand(accountId, card);
                }
            }

            return isAccepted;
        }

        private void DealHands()
        {
            _handsByAccountId[_firstAccountId] = _deck.Deal(CardsPerPlayer);
            _handsByAccountId[_secondAccountId] = _deck.Deal(CardsPerPlayer);
            _scoresByAccountId[_firstAccountId] = 0;
            _scoresByAccountId[_secondAccountId] = 0;
        }

        private bool IsDiscardAllowed(int accountId, List<Card> cards)
        {
            bool isAllowed = false;

            if (_phase == MatchPhase.Discarding && IsParticipant(accountId))
            {
                if (cards != null && cards.Count == CardsToDiscard)
                {
                    isAllowed = !_playersWhoDiscarded.Contains(accountId);
                }
            }

            return isAllowed;
        }

        private bool IsPlayAllowed(int accountId, Card card)
        {
            bool isAllowed = false;

            if (_phase == MatchPhase.Playing && accountId == _turnAccountId)
            {
                if (card != null)
                {
                    isAllowed = _peggingCount + GetCardValue(card.Rank) <= MaximumCount;
                }
            }

            return isAllowed;
        }

        private bool MoveToCrib(int accountId, List<Card> cards)
        {
            bool isMoved = false;
            List<Card> taken = TakeFromHand(accountId, cards);

            if (taken.Count == CardsToDiscard)
            {
                _crib.AddRange(taken);
                _playersWhoDiscarded.Add(accountId);
                RevealCutCardWhenReady();
                _stateVersion++;
                isMoved = true;
            }

            return isMoved;
        }

        private bool PlayFromHand(int accountId, Card card)
        {
            bool isPlayed = false;
            List<Card> requested = new List<Card>();
            requested.Add(card);

            List<Card> taken = TakeFromHand(accountId, requested);

            if (taken.Count == CardsPerPlay)
            {
                _playedCards.Add(taken[0]);
                _peggingCount += GetCardValue(taken[0].Rank);
                AddScore(accountId, PlayScorer.ScorePlay(_playedCards, _peggingCount));
                ContinueOrFinishPlay();
                _stateVersion++;
                isPlayed = true;
            }

            return isPlayed;
        }

        private void ContinueOrFinishPlay()
        {
            if (_phase == MatchPhase.Playing)
            {
                if (AreHandsEmpty())
                {
                    _phase = MatchPhase.Counting;
                    AwardGo(_turnAccountId);
                }
                else
                {
                    AdvanceTurn();
                }
            }
        }

        private void AdvanceTurn()
        {
            int rivalAccountId = GetOpponentId(_turnAccountId);

            if (HasPlayableCard(rivalAccountId))
            {
                _turnAccountId = rivalAccountId;
            }
            else if (!HasPlayableCard(_turnAccountId))
            {
                AwardGo(_turnAccountId);
                StartNewCount(rivalAccountId);
            }
        }

        private void AwardGo(int accountId)
        {
            if (_peggingCount < MaximumCount)
            {
                AddScore(accountId, GoPoints);
            }
        }

        private void AwardHisHeels()
        {
            if (_cutCard.Rank == CardRank.Jack)
            {
                AddScore(DealerAccountId, HisHeelsPoints);
            }
        }

        private void AddScore(int accountId, int points)
        {
            _scoresByAccountId[accountId] += points;

            if (_scoresByAccountId[accountId] >= WinningScore)
            {
                _scoresByAccountId[accountId] = WinningScore;
                _phase = MatchPhase.Finished;
            }
        }

        private void StartNewCount(int nextAccountId)
        {
            _playedCards.Clear();
            _peggingCount = 0;
            _turnAccountId = nextAccountId;
        }

        private bool HasPlayableCard(int accountId)
        {
            bool hasPlayable = false;

            foreach (Card card in _handsByAccountId[accountId])
            {
                if (_peggingCount + GetCardValue(card.Rank) <= MaximumCount)
                {
                    hasPlayable = true;
                }
            }

            return hasPlayable;
        }

        private bool AreHandsEmpty()
        {
            return _handsByAccountId[_firstAccountId].Count == 0
                && _handsByAccountId[_secondAccountId].Count == 0;
        }

        private List<Card> TakeFromHand(int accountId, List<Card> cards)
        {
            List<Card> remaining = new List<Card>(_handsByAccountId[accountId]);
            List<Card> taken = new List<Card>();

            foreach (Card card in cards)
            {
                int position = FindCard(remaining, card);

                if (position >= 0)
                {
                    taken.Add(remaining[position]);
                    remaining.RemoveAt(position);
                }
            }

            if (taken.Count == cards.Count)
            {
                _handsByAccountId[accountId] = remaining;
            }
            else
            {
                taken.Clear();
            }

            return taken;
        }

        private void RevealCutCardWhenReady()
        {
            if (_crib.Count == CribSize)
            {
                _cutCard = _deck.TakeCutCard();
                _turnAccountId = GetOpponentId(DealerAccountId);
                _phase = MatchPhase.Playing;
                AwardHisHeels();
            }
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