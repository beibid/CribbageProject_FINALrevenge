using System;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class FriendRecord
    {
        public int AccountId { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public bool HasProfileImage { get; set; }

        public DateTime? FriendsSince { get; set; }
    }
}