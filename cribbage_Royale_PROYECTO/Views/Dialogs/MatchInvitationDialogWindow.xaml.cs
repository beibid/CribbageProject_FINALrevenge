using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using CribbageRoyale.Contracts.DataContracts;
using cribbage_Royale_PROYECTO.Localization;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.ServiceAccess;
using cribbage_Royale_PROYECTO.Session;

namespace cribbage_Royale_PROYECTO.Views.Dialogs
{
    public partial class MatchInvitationDialogWindow : Window
    {
        private readonly MatchInvitationClient _matchInvitationClient;
        private readonly MatchInvitation _invitation;

        public MatchInvitationDialogWindow(MatchInvitation invitation)
        {
            InitializeComponent();

            _matchInvitationClient = ((App)Application.Current).MatchInvitationClient;
            _invitation = invitation;

            ApplyLocalization();
            ShowInvitation();
        }

        private void ApplyLocalization()
        {
            Title = Lang.MatchInvite_TitleText;
            titleTextBlock.Text = Lang.MatchInvite_TitleText;
            acceptButton.Content = Lang.MatchInvite_AcceptButton;
            rejectButton.Content = Lang.MatchInvite_RejectButton;
        }

        private void ShowInvitation()
        {
            invitationMessageTextBlock.Text = string.Format(
                CultureInfo.CurrentCulture,
                Lang.MatchInvite_InvitationMessage,
                _invitation.Sender.DisplayName);
        }

        private async void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            await RespondAsync(true);
        }

        private async void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            await RespondAsync(false);
        }

        private async Task RespondAsync(bool accepted)
        {
            acceptButton.IsEnabled = false;
            rejectButton.IsEnabled = false;

            InvitationResult result = await _matchInvitationClient.RespondAsync(
                CurrentSession.SessionToken,
                _invitation.InvitationId,
                accepted);

            ShowOutcome(result, accepted);
        }

        private void ShowOutcome(InvitationResult result, bool accepted)
        {
            if (result.Succeeded)
            {
                DialogResult = accepted;
            }
            else
            {
                messagePanel.ShowError(ResolveError(result));
                acceptButton.IsEnabled = true;
                rejectButton.IsEnabled = true;
            }
        }

        private string ResolveError(InvitationResult result)
        {
            string message = Lang.Invitation_ErrorMessage;

            if (result.Failure.HasValue)
            {
                message = InvitationMessages.ResolveRejection(result.Failure.Value);
            }

            return message;
        }
    }
}