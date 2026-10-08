using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cribbage_Royale_DataAccess.Models
{
    public class AuthenticationResult
    {
        public AuthenticationOutcome Outcome { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public int LockoutRemainingMinutes { get; set; }
    }
}
