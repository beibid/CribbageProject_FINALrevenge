using System.Collections.Generic;
using System.Linq;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class ScoreboardRepository
    {
        public List<vista_estadisticas_jugador> GetRanking()
        {
            using (CribbageEntities context = new CribbageEntities())
            {
                return context.vista_estadisticas_jugador
                    .OrderByDescending(record => record.nivel_habilidad)
                    .ThenByDescending(record => record.victorias)
                    .ToList();
            }
        }
    }
}