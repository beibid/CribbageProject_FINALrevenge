using System;
using System.Collections.Generic;
using cribbage_Royale_DataAccess;
using CribbageRoyale.Contracts.DataContracts;
using CribbageRoyale.Server.Projection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CribbageRoyale.Server.Tests.Projection
{
    [TestClass]
    public class ScoreboardProjectionTests
    {
        private const string UserName = "player_one";
        private const string DisplayName = "Player One";
        private const short SkillLevel = 3;
        private const int MatchesPlayed = 10;
        private const int Wins = 6;
        private const int Losses = 4;
        private const decimal WinPercentage = 60.00m;

        private ScoreboardProjection _scoreboardProjection;

        [TestInitialize]
        public void SetUp()
        {
            _scoreboardProjection = new ScoreboardProjection();
        }

        [TestMethod]
        public void BuildScoreboard_OneRecord_MapsAllFields()
        {
            List<player_statistics_view> records = new List<player_statistics_view> { BuildRecord(WinPercentage) };

            ScoreboardEntry entry = _scoreboardProjection.BuildScoreboard(records)[0];

            Assert.AreEqual(UserName, entry.UserName);
            Assert.AreEqual(DisplayName, entry.DisplayName);
            Assert.AreEqual(SkillLevel, entry.SkillLevel);
            Assert.AreEqual(MatchesPlayed, entry.MatchesPlayed);
            Assert.AreEqual(Wins, entry.Wins);
            Assert.AreEqual(Losses, entry.Losses);
            Assert.AreEqual(WinPercentage, entry.WinRate);
        }

        [TestMethod]
        public void BuildScoreboard_RecordWithoutWinPercentage_ReturnsZeroWinRate()
        {
            List<player_statistics_view> records = new List<player_statistics_view> { BuildRecord(null) };

            ScoreboardEntry entry = _scoreboardProjection.BuildScoreboard(records)[0];

            Assert.AreEqual(0m, entry.WinRate);
        }

        [TestMethod]
        public void BuildScoreboard_EmptyList_ReturnsEmptyList()
        {
            List<player_statistics_view> records = new List<player_statistics_view>();

            List<ScoreboardEntry> entries = _scoreboardProjection.BuildScoreboard(records);

            Assert.AreEqual(0, entries.Count);
        }

        [TestMethod]
        public void BuildScoreboard_NullList_ThrowsArgumentNullException()
        {
            List<player_statistics_view> records = null;

            Assert.ThrowsException<ArgumentNullException>(() => _scoreboardProjection.BuildScoreboard(records));
        }

        private static player_statistics_view BuildRecord(decimal? winPercentage)
        {
            player_statistics_view record = new player_statistics_view();
            record.username = UserName;
            record.display_name = DisplayName;
            record.skill_level = SkillLevel;
            record.matches_played = MatchesPlayed;
            record.wins = Wins;
            record.losses = Losses;
            record.win_percentage = winPercentage;

            return record;
        }
    }
}
