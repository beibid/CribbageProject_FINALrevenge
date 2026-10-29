using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using cribbage_Royale_DataAccess;
using CribbageRoyale.Contracts.DataContracts;

namespace CribbageRoyale.Server.Projection
{
    public class ScoreboardProjection
    {
        private const decimal WinRateWithoutMatches = 0m;

        public List<ScoreboardEntry> BuildScoreboard(List<vista_estadisticas_jugador> records)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            List<ScoreboardEntry> entries = new List<ScoreboardEntry>();

            foreach (vista_estadisticas_jugador record in records)
            {
                entries.Add(BuildEntry(record));
            }

            return entries;
        }

        private ScoreboardEntry BuildEntry(vista_estadisticas_jugador record)
        {
            return new ScoreboardEntry
            {
                UserName = record.nombre_usuario,
                DisplayName = record.nombre_mostrado,
                SkillLevel = record.nivel_habilidad,
                MatchesPlayed = record.partidas_jugadas,
                Wins = record.victorias,
                Losses = record.derrotas,
                WinRate = record.porcentaje_victorias ?? WinRateWithoutMatches
            };
        }
    }
}
