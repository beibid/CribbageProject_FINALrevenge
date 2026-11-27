using cribbage_Royale_DataAccess;
using CribbageRoyale.Contracts.DataContracts;
using System;
using System.Collections.Generic;

namespace CribbageRoyale.Server.Projection
{
    public class ScoreboardProjection
    {
        private const decimal WinRateWithoutMatches = 0m;

        public List<ScoreboardEntry> BuildScoreboard(List<player_statistics_view> records)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            List<ScoreboardEntry> entries = new List<ScoreboardEntry>();

            foreach (player_statistics_view record in records)
            {
                entries.Add(BuildEntry(record));
            }

            return entries;
        }

        private ScoreboardEntry BuildEntry(player_statistics_view record)
        {
            return new ScoreboardEntry
            {
                UserName = record.username,
                DisplayName = record.display_name,
                SkillLevel = record.skill_level,
                MatchesPlayed = record.matches_played,
                Wins = record.wins,
                Losses = record.losses,
                WinRate = record.win_percentage ?? WinRateWithoutMatches
            };
        }
    }
}
