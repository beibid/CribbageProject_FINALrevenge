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

        public List<player_statistics_view> GetRanking()
        {
            List<player_statistics_view> ranking;

            try
            {
                using (CribbageEntities context = new CribbageEntities())
                {
                    ranking = context.player_statistics_view
                        .OrderByDescending(record => record.skill_level)
                        .ThenByDescending(record => record.wins)
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