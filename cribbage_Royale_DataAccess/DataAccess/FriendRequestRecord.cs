namespace cribbage_Royale_DataAccess.DataAccess
{
    public class FriendRequestRecord
    {
        public long RequestId { get; set; }

        public int SenderAccountId { get; set; }

        public int RecipientAccountId { get; set; }
    }
}