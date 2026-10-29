using System.Collections.Generic;
using cribbage_Royale_DataAccess;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public interface IScoreboardRepository
    {
        List<vista_estadisticas_jugador> GetRanking();
    }
}