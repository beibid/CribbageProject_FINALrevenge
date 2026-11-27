using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public static class MatchTerminationReason
    {
        public const string PointsVictory = "SCORE_VICTORY";
        public const string Abandonment = "ABANDONMENT";
        public const string Inactivity = "INACTIVITY";
        public const string Disconnection = "DISCONNECTION";
        public const string Sanction = "SANCTION";
    }
}
