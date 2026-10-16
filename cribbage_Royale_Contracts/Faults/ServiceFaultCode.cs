using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribbageRoyale.Contracts.Faults
{
    public enum ServiceFaultCode
    {
        InvalidSession,
        MatchNotFound,
        DataAccess,
        Unauthorized,
        InvalidRequest
    }
}
