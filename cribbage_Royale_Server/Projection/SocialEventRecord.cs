using System;

namespace CribbageRoyale.Server.Projection
{
    public class SocialEventRecord
    {
        public SocialEventRecord(int counterpartAccountId, long? friendRequestId, DateTime occurredAtUtc)
        {
            CounterpartAccountId = counterpartAccountId;
            FriendRequestId = friendRequestId;
            OccurredAtUtc = occurredAtUtc;
        }

        public int CounterpartAccountId { get; }

        public long? FriendRequestId { get; }

        public DateTime OccurredAtUtc { get; }
    }
}