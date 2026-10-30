using System;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class UserCredentials
    {
        public int AccountId { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string Status { get; set; }

        public DateTime? LockoutEndDate { get; set; }
    }
}