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

        public bool TryGetMatch(long matchId, out CribbageMatch match)
        {
            bool found;

            lock (_syncRoot)
            {
                found = _matches.TryGetValue(matchId, out match);
            }

            return found;
        }

        public bool FindMatchOf(int accountId, out CribbageMatch match)
        {
            bool found = false;
            long matchId;
            match = null;

            lock (_syncRoot)
            {
                if (_matchesByAccount.TryGetValue(accountId, out matchId))
                {
                    found = _matches.TryGetValue(matchId, out match);
                }
            }

            return found;
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

        public void CloseMatch(long matchId, int firstAccountId, int secondAccountId)
        {
            lock (_syncRoot)
            {
                _matchesByAccount.Remove(firstAccountId);
                _matchesByAccount.Remove(secondAccountId);
                _matches.Remove(matchId);
            }
        }
    }
}