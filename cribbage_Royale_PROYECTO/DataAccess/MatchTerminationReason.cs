using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cribbage_Royale_PROYECTO.DataAccess
{
    public static class MatchTerminationReason
    {
        public const string PointsVictory = "VICTORIA_POR_PUNTUACION";
        public const string Abandonment = "ABANDONO";
        public const string Inactivity = "INACTIVIDAD";
        public const string Disconnection = "DESCONEXION";
        public const string Sanction = "SANCION";
    }
}
