using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public static class AccountStatus
    {
        public const string PendingVerification = "PENDING_VERIFICATION";
        public const string Verified = "VERIFIED";
        public const string Suspended = "SUSPENDED";
        public const string Deleted = "DELETED";
    }
}
