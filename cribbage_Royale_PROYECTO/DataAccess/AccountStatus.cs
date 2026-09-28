using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cribbage_Royale_PROYECTO.DataAccess
{
    public static class AccountStatus
    {
        public const string PendingVerification = "PENDIENTE_VERIFICACION";
        public const string Verified = "VERIFICADA";
        public const string Suspended = "SUSPENDIDA";
        public const string Deleted = "ELIMINADA";
    }
}
