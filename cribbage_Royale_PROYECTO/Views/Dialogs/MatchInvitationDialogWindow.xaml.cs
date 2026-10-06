using System;
using System.Windows;
using cribbage_Royale_PROYECTO.DataAccess;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Session;

namespace cribbage_Royale_PROYECTO.Views.Dialogs
{
    public partial class MatchInvitationDialogWindow : Window
    {
        private readonly long _invitationId;
        private readonly InvitationRepository _invitationRepository = new InvitationRepository();
        private int _recipientAccountId;

        public long CreatedMatchId { get; private set; }
        public bool GoToMatchRequested { get; private set; }

        public MatchInvitationDialogWindow(long invitationId)
        {
            InitializeComponent();
            _invitationId = invitationId;
            ApplyLocalization();
        }

        private void ApplyLocalization()
        {
            Title = Lang.MatchInvite_TitleText;
            titleTextBlock.Text = Lang.MatchInvite_TitleText;
            acceptButton.Content = Lang.MatchInvite_AcceptButton;
            rejectButton.Content = Lang.MatchInvite_RejectButton;
            goToMatchButton.Content = Lang.MatchInvite_GoToMatchButton;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _recipientAccountId = new UserRepository().GetUserId(CurrentSession.Username);

            try
            {
                MatchInvitationSummary invitation = _invitationRepository.GetInvitation(_invitationId);
                bool isAvailable = invitation != null
                    && invitation.Status == InvitationStatus.Pending
                    && invitation.RecipientAccountId == _recipientAccountId;

                if (isAvailable)
                {
                    invitationMessageTextBlock.Text = string.Format(Lang.MatchInvite_InvitationMessage,
                        invitation.SenderDisplayName);
                }
                else
                {
                    ShowUnavailableAndClose();
                }
            }
            catch (Exception dataAccessException)
            {
                messagePanel.ShowError(Lang.Dialog_DatabaseAccessErrorMessage);
                acceptButton.IsEnabled = false;
            }
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InvitationResponseResult response = _invitationRepository
                    .AcceptInvitation(_invitationId, _recipientAccountId);

                if (response.Outcome == InvitationAcceptOutcome.Accepted)
                {
                    CreatedMatchId = response.MatchId;
                    DialogResult = true;
                }
                else if (response.Outcome == InvitationAcceptOutcome.HasUnfinishedMatch)
                {
                    messagePanel.ShowWarning(Lang.MatchInvite_UnfinishedMatchMessage);
                    acceptButton.IsEnabled = false;
                    goToMatchButton.Visibility = Visibility.Visible;
                }
                else
                {
                    ShowUnavailableAndClose();
                }
            }
            catch (Exception dataAccessException)
            {
                messagePanel.ShowError(Lang.Dialog_DatabaseAccessErrorMessage);
            }
        }

        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _invitationRepository.RejectInvitation(_invitationId);
                DialogResult = false;
            }
            catch (Exception dataAccessException)
            {
                messagePanel.ShowError(Lang.Dialog_DatabaseAccessErrorMessage);
            }
        }

        private void GoToMatchButton_Click(object sender, RoutedEventArgs e)
        {
            GoToMatchRequested = true;
            DialogResult = false;
        }

        private void ShowUnavailableAndClose()
        {
            MessageDialog.ShowWarning(this, Lang.MatchInvite_UnavailableMessage);
            Close();
        }
    }
}