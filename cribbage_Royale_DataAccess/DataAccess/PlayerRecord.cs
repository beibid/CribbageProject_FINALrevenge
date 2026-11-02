namespace cribbage_Royale_DataAccess.DataAccess
{
    public class PlayerRecord
    {
        public int AccountId { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public bool HasProfileImage { get; set; }
    }
}