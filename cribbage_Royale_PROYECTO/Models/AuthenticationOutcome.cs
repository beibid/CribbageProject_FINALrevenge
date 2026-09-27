using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cribbage_Royale_PROYECTO.Models
{
    public enum AuthenticationOutcome
    {
        Success,
        InvalidCredentials,
        AccountNotVerified,
        AccountSuspended,
        AccountLocked,
        DatabaseError
    }
}
