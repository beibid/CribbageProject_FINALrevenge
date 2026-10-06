using System;
using System.Windows;
using System.Windows.Threading;
using cribbage_Royale_PROYECTO.DataAccess;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Session;

namespace cribbage_Royale_PROYECTO.Views.Dialogs
{
    public partial class MatchInformationDialogWindow : Window
    {
        private const int RefreshIntervalSeconds = 5;
        private const string EmptyValueText = "-";

        private readonly long _matchId;
        private readonly MatchRepository _matchRepository = new MatchRepository();
        private readonly DispatcherTimer _clockTimer = new DispatcherTimer();
        private int _viewerAccountId;
        private int _secondsSinceRefresh;
        private MatchInformationSummary _lastSummary;

        public bool ReportRequested { get; private set; }
        public int OpponentAccountId { get; private set; }
        public string OpponentDisplayName { get; private set; }

        public long MatchId
        {
            get { return _matchId; }
        }

        public MatchInformationDialogWindow(long matchId)
        {
            InitializeComponent();
            _matchId = matchId;
            ApplyLocalization();
            _clockTimer.Interval = TimeSpan.FromSeconds(1);
            _clockTimer.Tick += ClockTimer_Tick;
        }

        private void ApplyLocalization()
        {
            Title = Lang.MatchInfo_TitleText;
            titleTextBlock.Text = Lang.MatchInfo_TitleText;
            opponentLabel.Text = Lang.MatchInfo_OpponentLabel;
            skillLabel.Text = Lang.MatchInfo_SkillLevelLabel;
            scoreLabel.Text = Lang.MatchInfo_ScoreLabel;
            handLabel.Text = Lang.MatchInfo_HandLabel;
            dealerLabel.Text = Lang.MatchInfo_DealerLabel;
            cutCardLabel.Text = Lang.MatchInfo_CutCardLabel;
            countLabel.Text = Lang.MatchInfo_CountLabel;
            opponentCardsLabel.Text = Lang.MatchInfo_OpponentCardsLabel;
            turnTimeLabel.Text = Lang.MatchInfo_TurnTimeLabel;
            opponentStatusLabel.Text = Lang.MatchInfo_OpponentStatusLabel;
            reportPlayerButton.Content = Lang.MatchInfo_ReportPlayerButton;
            closeButton.Content = Lang.Common_CloseButton;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewerAccountId = new UserRepository().GetUserId(CurrentSession.Username);
            LoadInformation();
            _clockTimer.Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _clockTimer.Stop();
        }

        private void LoadInformation()
        {
            try
            {
                MatchInformationSummary summary = _matchRepository.GetMatchInformation(_matchId, _viewerAccountId);

                if (summary == null)
                {
                    messagePanel.ShowError(Lang.Dialog_DatabaseAccessErrorMessage);
                    reportPlayerButton.IsEnabled = false;
                }
                else
                {
                    _lastSummary = summary;
                    messagePanel.HidePanel();
                    ShowSummary(summary);
                }
            }
            catch (Exception dataAccessException)
            {
                ShowLoadFailure();
            }
        }

        private void ShowLoadFailure()
        {
            if (_lastSummary == null)
            {
                messagePanel.ShowError(Lang.Dialog_DatabaseAccessErrorMessage);
                reportPlayerButton.IsEnabled = false;
            }
            else
            {
                messagePanel.ShowWarning(Lang.MatchInfo_StaleStateWarning);
            }
        }

        private void ShowSummary(MatchInformationSummary summary)
        {
            OpponentAccountId = summary.OpponentAccountId;
            OpponentDisplayName = summary.OpponentDisplayName;

            opponentValue.Text = summary.OpponentDisplayName;
            skillValue.Text = summary.OpponentSkillLevel.ToString();
            scoreValue.Text = string.Format("{0} - {1}", summary.ViewerScore, summary.OpponentScore);
            handValue.Text = summary.HandNumber.ToString();
            dealerValue.Text = summary.DealerDisplayName;
            countValue.Text = summary.RunningCount.ToString();
            opponentCardsValue.Text = summary.OpponentRemainingCards.ToString();
            cutCardValue.Text = EmptyValueText;
            opponentStatusValue.Text = Lang.MatchInfo_ReconnectingStatus;

            if (!string.IsNullOrEmpty(summary.CutCardText))
            {
                cutCardValue.Text = summary.CutCardText;
            }

            if (summary.OpponentConnected)
            {
                opponentStatusValue.Text = Lang.MatchInfo_ConnectedStatus;
            }

            UpdateTurnTime();
        }

        private void UpdateTurnTime()
        {
            turnTimeValue.Text = EmptyValueText;

            if (_lastSummary != null && _lastSummary.TurnDeadline.HasValue)
            {
                double remainingSeconds = (_lastSummary.TurnDeadline.Value - DateTime.Now).TotalSeconds;
                int shownSeconds = (int)Math.Max(0, remainingSeconds);
                turnTimeValue.Text = string.Format(Lang.MatchInfo_SecondsFormat, shownSeconds);
            }
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            _secondsSinceRefresh++;
            UpdateTurnTime();

            if (_secondsSinceRefresh >= RefreshIntervalSeconds)
            {
                _secondsSinceRefresh = 0;
                LoadInformation();
            }
        }

        private void ReportPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            ReportRequested = true;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}