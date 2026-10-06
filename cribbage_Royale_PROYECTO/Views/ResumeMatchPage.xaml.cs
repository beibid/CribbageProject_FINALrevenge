using System;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.DataAccess;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_PROYECTO.Session;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class ResumeMatchPage : Page
    {
        private readonly MatchRepository _matchRepository = new MatchRepository();
        private int _accountId;
        private UnfinishedMatchSummary _unfinishedMatch;

        public ResumeMatchPage()
        {
            InitializeComponent();
            ApplyLocalization();
        }

        private void ApplyLocalization()
        {
            titleTextBlock.Text = Lang.ResumeMatch_TitleText;
            resumeButton.Content = Lang.ResumeMatch_ResumeButton;
            abandonButton.Content = Lang.ResumeMatch_AbandonButton;
            retryButton.Content = Lang.Common_RetryButton;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _accountId = new UserRepository().GetUserId(CurrentSession.Username);
            LoadUnfinishedMatch();
        }

        private void LoadUnfinishedMatch()
        {
            messagePanel.HidePanel();
            retryButton.Visibility = Visibility.Collapsed;

            try
            {
                _unfinishedMatch = _matchRepository.FindUnfinishedMatch(_accountId);

                if (_unfinishedMatch == null)
                {
                    SetMatchActionsEnabled(false);
                    matchMessageTextBlock.Text = Lang.ResumeMatch_NoMatchMessage;
                }
                else
                {
                    SetMatchActionsEnabled(true);
                    matchMessageTextBlock.Text = string.Format(Lang.ResumeMatch_UnfinishedMatchMessage,
                        _unfinishedMatch.OpponentDisplayName,
                        _unfinishedMatch.ViewerScore,
                        _unfinishedMatch.OpponentScore);
                }
            }
            catch (Exception dataAccessException)
            {
                SetMatchActionsEnabled(false);
                matchMessageTextBlock.Text = string.Empty;
                messagePanel.ShowError(Lang.ResumeMatch_RecoverErrorMessage);
                retryButton.Visibility = Visibility.Visible;
            }
        }

        private void SetMatchActionsEnabled(bool isEnabled)
        {
            resumeButton.IsEnabled = isEnabled;
            abandonButton.IsEnabled = isEnabled;
        }

        private void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUnfinishedMatch();
        }

        private void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool updated = _matchRepository.MarkParticipantConnected(_unfinishedMatch.MatchId, _accountId);

                if (!updated)
                {
                    messagePanel.ShowError(Lang.ResumeMatch_RecoverErrorMessage);
                }
            }
            catch (Exception dataAccessException)
            {
                messagePanel.ShowError(Lang.ResumeMatch_RecoverErrorMessage);
            }
        }

        private void AbandonButton_Click(object sender, RoutedEventArgs e)
        {
            Window owner = Window.GetWindow(this);
            bool confirmed = MessageDialog.ShowConfirmation(owner, Lang.ResumeMatch_AbandonConfirmMessage);

            if (confirmed)
            {
                AbandonMatch();
            }
        }

        private void AbandonMatch()
        {
            try
            {
                string outcome = _matchRepository.RegisterAbandonment(_unfinishedMatch.MatchId, _accountId);

                if (outcome == AbandonmentOutcome.Registered)
                {
                    LoadUnfinishedMatch();
                }
                else
                {
                    messagePanel.ShowWarning(Lang.ResumeMatch_AlreadyConcludedMessage);
                    SetMatchActionsEnabled(false);
                }
            }
            catch (Exception dataAccessException)
            {
                messagePanel.ShowError(Lang.ResumeMatch_AbandonErrorMessage);
            }
        }
    }
}