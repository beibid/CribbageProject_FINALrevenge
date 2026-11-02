using System;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class FriendRecord : PlayerRecord
    {
        public DateTime? FriendsSince { get; set; }
    }
}