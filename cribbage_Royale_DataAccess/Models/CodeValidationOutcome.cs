using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cribbage_Royale_DataAccess.Models
{
    public enum CodeValidationOutcome
    {
        Valid,
        InvalidCode,
        ExpiredCode,
        AccountAlreadyVerified,
        DatabaseError
    }
}
