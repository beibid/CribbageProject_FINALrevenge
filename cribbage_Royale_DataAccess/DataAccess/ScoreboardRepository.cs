using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using cribbage_Royale_DataAccess;
using CribbageRoyale.Exceptions;


namespace cribbage_Royale_DataAccess.DataAccess
{
    public class ScoreboardRepository : IScoreboardRepository
    {
        private const string RankingQueryFailureMessage = "The player ranking could not be queried.";

        public List<vista_estadisticas_jugador> GetRanking()
        {
            List<vista_estadisticas_jugador> ranking;

            try
            {
                using (CribbageEntities context = new CribbageEntities())
                {
                    ranking = context.vista_estadisticas_jugador
                        .OrderByDescending(record => record.nivel_habilidad)
                        .ThenByDescending(record => record.victorias)
                        .ToList();
                }
            }
            catch (EntityException entityException)
            {
                throw new DataAccessException(RankingQueryFailureMessage, entityException);
            }
            catch (SqlException sqlException)
            {
                throw new DataAccessException(RankingQueryFailureMessage, sqlException);
            }

            return ranking;
        }
    }
}