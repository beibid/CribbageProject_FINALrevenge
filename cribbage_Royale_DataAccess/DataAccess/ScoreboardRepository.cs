using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace cribbage_Royale_DataAccess.DataAccess
{
    public class ScoreboardRepository
    {
        public async Task<List<vista_estadisticas_jugador>> GetRankingAsync()
        {
            using (CribbageEntities context = new CribbageEntities())
            {
                return await context.vista_estadisticas_jugador
                    .OrderByDescending(record => record.nivel_habilidad)
                    .ThenByDescending(record => record.victorias)
                    .ToListAsync();
            }
        }
    }
}