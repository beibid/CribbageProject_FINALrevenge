using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_DataAccess.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class MatchResultPage : Page
    {
        private const string SkillLevelChangeFormat = "+#;-#;0";

        private const int SamplePlayerScore = 61;
        private const int SampleOpponentScore = 47;
        private const int SampleDurationMinutes = 12;
        private const int SampleDurationSeconds = 34;
        private const int SampleSkillLevel = 1012;
        private const int SampleSkillLevelChange = 12;

        private readonly MatchResultItem _matchResult;

        public MatchResultPage(MatchResultItem matchResult = null)
        {
            InitializeComponent();

            _matchResult = matchResult ?? BuildSampleResult();

            ShowResult();
        }

        private void ShowResult()
        {
            ShowOutcome();
            ShowScore();
            ShowMatchDetails();
        }

        private void ShowOutcome()
        {
            victoryTitleTextBlock.Visibility = _matchResult.IsVictory
                ? Visibility.Visible
                : Visibility.Collapsed;

            defeatTitleTextBlock.Visibility = _matchResult.IsVictory
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void ShowScore()
        {
            string opponentName = string.IsNullOrWhiteSpace(_matchResult.OpponentName)
                ? Lang.MatchResult_OpponentUnknown
                : _matchResult.OpponentName;

            opponentTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.MatchResult_OpponentFormat,
                opponentName);

            scoreTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.MatchResult_ScoreFormat,
                _matchResult.PlayerScore,
                _matchResult.OpponentScore);

            endReasonTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.MatchResult_ReasonFormat,
                GetEndReasonText());
        }

        private void ShowMatchDetails()
        {
            durationTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.MatchResult_DurationFormat,
                (int)_matchResult.Duration.TotalMinutes,
                _matchResult.Duration.Seconds);

            skillLevelTextBlock.Text = _matchResult.SkillLevel.ToString(
                CultureInfo.CurrentCulture);

            string changeText = _matchResult.SkillLevelChange.ToString(
                SkillLevelChangeFormat,
                CultureInfo.CurrentCulture);

            bool isSkillLevelGain = _matchResult.SkillLevelChange >= 0;

            skillLevelGainTextBlock.Text = changeText;
            skillLevelLossTextBlock.Text = changeText;

            skillLevelGainTextBlock.Visibility = isSkillLevelGain
                ? Visibility.Visible
                : Visibility.Collapsed;

            skillLevelLossTextBlock.Visibility = isSkillLevelGain
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private string GetEndReasonText()
        {
            switch (_matchResult.EndReason)
            {
                case MatchEndReason.Surrender:
                    return Lang.MatchResult_ReasonSurrender;
                case MatchEndReason.Inactivity:
                    return Lang.MatchResult_ReasonInactivity;
                case MatchEndReason.Disconnection:
                    return Lang.MatchResult_ReasonDisconnection;
                case MatchEndReason.PointsVictory:
                default:
                    return Lang.MatchResult_ReasonPointsVictory;
            }
        }

        private MatchResultItem BuildSampleResult()
        {
            return new MatchResultItem
            {
                OpponentName = null,
                PlayerScore = SamplePlayerScore,
                OpponentScore = SampleOpponentScore,
                IsVictory = true,
                EndReason = MatchEndReason.PointsVictory,
                Duration = new TimeSpan(0, SampleDurationMinutes, SampleDurationSeconds),
                SkillLevel = SampleSkillLevel,
                SkillLevelChange = SampleSkillLevelChange
            };
        }

        private void BackToLobbyButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Lobby());
        }
    }
}