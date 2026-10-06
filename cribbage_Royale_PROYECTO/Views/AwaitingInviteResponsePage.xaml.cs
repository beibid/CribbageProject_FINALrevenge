using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using cribbage_Royale_PROYECTO.DataAccess;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Session;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class AwaitingInviteResponsePage : Page
    {
        private const int PollingIntervalSeconds = 2;

        private readonly int _friendAccountId;
        private readonly string _friendDisplayName;
        private readonly InvitationRepository _invitationRepository = new InvitationRepository();
        private readonly DispatcherTimer _pollingTimer = new DispatcherTimer();
        private long _invitationId;
        private int _senderAccountId;

        public AwaitingInviteResponsePage(int friendAccountId, string friendDisplayName)
        {
            InitializeComponent();
            _friendAccountId = friendAccountId;
            _friendDisplayName = friendDisplayName;
            _pollingTimer.Interval = TimeSpan.FromSeconds(PollingIntervalSeconds);
            _pollingTimer.Tick += PollingTimer_Tick;
            ApplyLocalization();
        }

        private void ApplyLocalization()
        {
            titleTextBlock.Text = Lang.AwaitingInvite_TitleText;
            cancelInvitationButton.Content = Lang.AwaitingInvite_CancelButton;
            inviteAgainButton.Content = Lang.AwaitingInvite_InviteAgainButton;
            backButton.Content = Lang.AwaitingInvite_BackButton;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _senderAccountId = new UserRepository().GetUserId(CurrentSession.Username);
            SendInvitation();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _pollingTimer.Stop();
        }

        private void SendInvitation()
        {
            messagePanel.HidePanel();
            inviteAgainButton.Visibility = Visibility.Collapsed;

            try
            {
                InvitationCreationResult result = _invitationRepository
                    .CreateInvitation(_senderAccountId, _friendAccountId);

                if (result.Outcome == InvitationCreationOutcome.Created)
                {
                    _invitationId = result.InvitationId;
                    ShowWaitingState();
                    _pollingTimer.Start();
                }
                else
                {
                    ShowFinishedState();
                    messagePanel.ShowWarning(GetCreationOutcomeMessage(result.Outcome));
                }
            }
            catch (Exception dataAccessException)
            {
                ShowFinishedState();
                messagePanel.ShowError(Lang.Dialog_DatabaseAccessErrorMessage);
            }
        }

        private string GetCreationOutcomeMessage(string outcome)
        {
            string message = Lang.MatchInvite_UnfinishedMatchMessage;

            if (outcome == InvitationCreationOutcome.RecipientHasUnfinishedMatch)
            {
                message = Lang.AwaitingInvite_RecipientInMatchMessage;
            }
            else if (outcome == InvitationCreationOutcome.RecipientOffline)
            {
                message = Lang.AwaitingInvite_RecipientOfflineMessage;
            }
            else if (outcome == InvitationCreationOutcome.AlreadyInvitedByYou)
            {
                message = Lang.AwaitingInvite_AlreadyInvitedMessage;
            }

            return message;
        }

        private void ShowWaitingState()
        {
            waitingTextBlock.Text = string.Format(Lang.AwaitingInvite_WaitingMessage, _friendDisplayName);
            cancelInvitationButton.Visibility = Visibility.Visible;
            cancelInvitationButton.IsEnabled = true;
        }

        private void ShowFinishedState()
        {
            waitingTextBlock.Text = string.Empty;
            cancelInvitationButton.Visibility = Visibility.Collapsed;
        }

        private void PollingTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                MatchInvitationSummary invitation = _invitationRepository.GetInvitation(_invitationId);

                if (invitation != null)
                {
                    HandleInvitationStatus(invitation);
                }
            }
            catch (Exception dataAccessException)
            {
                _pollingTimer.Stop();
                ShowFinishedState();
                messagePanel.ShowError(Lang.Dialog_DatabaseAccessErrorMessage);
            }
        }

        private void HandleInvitationStatus(MatchInvitationSummary invitation)
        {
            if (invitation.Status == InvitationStatus.Accepted)
            {
                _pollingTimer.Stop();
                cancelInvitationButton.IsEnabled = false;
            }
            else if (invitation.Status == InvitationStatus.Rejected)
            {
                _pollingTimer.Stop();
                ShowFinishedState();
                messagePanel.ShowWarning(string.Format(Lang.AwaitingInvite_RecipientRejectedMessage,
                    _friendDisplayName));
            }
            else if (invitation.Status == InvitationStatus.Expired)
            {
                _pollingTimer.Stop();
                _invitationRepository.ExpireInvitation(_invitationId);
                ShowFinishedState();
                messagePanel.ShowWarning(Lang.AwaitingInvite_ExpiredMessage);
                inviteAgainButton.Visibility = Visibility.Visible;
            }
            else if (invitation.Status == InvitationStatus.Cancelled)
            {
                _pollingTimer.Stop();
                ShowFinishedState();
            }
        }

        private void InviteAgainButton_Click(object sender, RoutedEventArgs e)
        {
            SendInvitation();
        }

        private void CancelInvitationButton_Click(object sender, RoutedEventArgs e)
        {
            _pollingTimer.Stop();

            try
            {
                bool cancelled = _invitationRepository.CancelInvitation(_invitationId);

                if (cancelled)
                {
                    GoBackToPreviousPage();
                }
                else
                {
                    _pollingTimer.Start();
                }
            }
            catch (Exception dataAccessException)
            {
                _pollingTimer.Start();
                messagePanel.ShowError(Lang.Dialog_DatabaseAccessErrorMessage);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GoBackToPreviousPage();
        }

        private void GoBackToPreviousPage()
        {
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}