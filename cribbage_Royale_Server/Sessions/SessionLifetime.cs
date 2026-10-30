using System;

namespace CribbageRoyale.Server.Sessions
{
    public class SessionLifetime
    {
        private readonly TimeSpan _duration;

        public SessionLifetime(TimeSpan duration)
        {
            _duration = duration;
        }

        public DateTime CalculateExpiration()
        {
            return DateTime.UtcNow.Add(_duration);
        }
    }
}