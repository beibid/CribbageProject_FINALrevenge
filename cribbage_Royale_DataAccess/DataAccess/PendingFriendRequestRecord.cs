using System;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class PendingFriendRequestRecord : PlayerRecord
    {
        public long RequestId { get; set; }

        public bool IsReceived { get; set; }

        public DateTime? RequestedAt { get; set; }
    }
}