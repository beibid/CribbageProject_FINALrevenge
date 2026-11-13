using System.Collections.Generic;
using CribbageRoyale.Rules;

namespace CribbageRoyale.Server.Matches
{
    public class MatchRoomRegistry
    {
        private readonly Dictionary<int, long> _matchesByAccount = new Dictionary<int, long>();

        private readonly Dictionary<long, CribbageMatch> _matches =
            new Dictionary<long, CribbageMatch>();

        private readonly object _syncRoot = new object();

        private long _lastMatchId;

        public bool IsInMatch(int accountId)
        {
            bool inMatch;

            lock (_syncRoot)
            {
                inMatch = _matchesByAccount.ContainsKey(accountId);
            }

            return inMatch;
        }

        public CribbageMatch StartMatch(int firstAccountId, int secondAccountId)
        {
            CribbageMatch match;

            lock (_syncRoot)
            {
                _lastMatchId++;
                match = new CribbageMatch(_lastMatchId, firstAccountId, secondAccountId);
                _matches[_lastMatchId] = match;
                _matchesByAccount[firstAccountId] = _lastMatchId;
                _matchesByAccount[secondAccountId] = _lastMatchId;
            }

            return match;
        }

        public void RemovePlayer(int accountId)
        {
            long matchId;

            lock (_syncRoot)
            {
                if (_matchesByAccount.TryGetValue(accountId, out matchId))
                {
                    _matchesByAccount.Remove(accountId);
                    RemoveMatchWhenEmpty(matchId);
                }
            }
        }

        private void RemoveMatchWhenEmpty(long matchId)
        {
            if (!_matchesByAccount.ContainsValue(matchId))
            {
                _matches.Remove(matchId);
            }
        }
    }
}