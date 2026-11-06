using System.Collections.Generic;

namespace CribbageRoyale.Server.Matches
{
    public class MatchRoomRegistry
    {
        private readonly Dictionary<int, long> _matchesByAccount = new Dictionary<int, long>();

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

        public long StartMatch(int firstAccountId, int secondAccountId)
        {
            long matchId;

            lock (_syncRoot)
            {
                _lastMatchId++;
                matchId = _lastMatchId;
                _matchesByAccount[firstAccountId] = matchId;
                _matchesByAccount[secondAccountId] = matchId;
            }

            return matchId;
        }

        public void RemovePlayer(int accountId)
        {
            lock (_syncRoot)
            {
                _matchesByAccount.Remove(accountId);
            }
        }
    }
}