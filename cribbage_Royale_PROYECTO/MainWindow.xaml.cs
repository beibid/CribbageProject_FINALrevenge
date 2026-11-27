using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Navigation;
using CribbageRoyale.Contracts.DataContracts;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.ServiceAccess;
using cribbage_Royale_PROYECTO.Views;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO
{
    public partial class MainWindow : Window
    {
        private readonly PlayerSessionClient _sessionClient;

        private bool _sessionEnded;

        public MainWindow()
        {
            InitializeComponent();
            _sessionClient = ((App)Application.Current).SessionClient;
            _sessionClient.ConnectionLost += SessionClient_ConnectionLost;
            ((App)Application.Current).SessionCallback.RevocationReceived += SessionCallback_RevocationReceived;
            ((App)Application.Current).SessionCallback.SocialEventReceived += SessionCallback_SocialEventReceived;
            ((App)Application.Current).SessionCallback.InvitationArrived += SessionCallback_InvitationArrived;
            ((App)Application.Current).SessionCallback.InvitationWithdrawn += SessionCallback_InvitationWithdrawn;
            ((App)Application.Current).SessionCallback.MatchStartReceived += SessionCallback_MatchStartReceived;
            MainFrame.Navigated += MainFrame_Navigated;
            MainFrame.Navigate(new StartMenu());
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            RequestSignOut();
            base.OnClosing(e);
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is Lobby)
            {
                _sessionEnded = false;
            }
        }
        private void RequestSignOut()
        {
            if (!_sessionEnded && _sessionClient.IsConnected())
            {
                _sessionEnded = true;
                _sessionClient.SignOutAsync();
            }
        }

        private void SessionClient_ConnectionLost(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => ShowConnectionLost()));
        }

        private void SessionCallback_RevocationReceived(object sender, SessionRevokedEventArgs e)
        {
            if (!_sessionEnded)
            {
                _sessionEnded = true;
                MessageDialog.ShowError(this, Lang.Session_SignedInElsewhereMessage);
                ReturnToLogin();
            }
        }

        private void SessionCallback_SocialEventReceived(object sender, SocialEventReceivedEventArgs e)
        {
            string messageFormat = ResolveSocialMessageFormat(e.EventType);

            if (!_sessionEnded && !string.IsNullOrEmpty(messageFormat))
            {
                ShowSocialEvent(messageFormat, e.Social);
            }
        }

        private void SessionCallback_InvitationArrived(object sender, MatchInvitationEventArgs e)
        {
            if (!_sessionEnded)
            {
                ShowInvitation(e.Invitation);
            }
        }

        private void SessionCallback_InvitationWithdrawn(object sender, MatchInvitationEventArgs e)
        {
            if (!_sessionEnded)
            {
                ShowInvitationWithdrawn(e.Invitation);
            }
        }

        private void SessionCallback_MatchStartReceived(object sender, MatchStartedEventArgs e)
        {
            if (!_sessionEnded)
            {
                MainFrame.Navigate(new GamePage(e.MatchStart));
            }
        }

        private void ShowInvitation(MatchInvitation invitation)
        {
            MatchInvitationDialogWindow dialog = new MatchInvitationDialogWindow(invitation);
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void ShowInvitationWithdrawn(MatchInvitation invitation)
        {
            string message = string.Format(
                CultureInfo.CurrentCulture,
                Lang.AwaitingInvite_RecipientRejectedMessage,
                invitation.Recipient.DisplayName);

            MessageDialog.ShowWarning(this, message);
        }

        private string ResolveSocialMessageFormat(SocialEventType eventType)
        {
            string messageFormat;

            switch (eventType)
            {
                case SocialEventType.FriendRequestReceived:
                    messageFormat = Lang.MainWindow_FriendRequestReceivedFormat;
                    break;
                case SocialEventType.FriendRequestAccepted:
                    messageFormat = Lang.MainWindow_FriendRequestAcceptedFormat;
                    break;
                case SocialEventType.FriendRequestRejected:
                    messageFormat = Lang.MainWindow_FriendRequestRejectedFormat;
                    break;
                default:
                    messageFormat = string.Empty;
                    break;
            }

            return messageFormat;
        }

        private void ShowSocialEvent(string messageFormat, SocialEvent social)
        {
            string message = string.Format(
                CultureInfo.CurrentCulture,
                messageFormat,
                social.Counterpart.DisplayName);

            MessageDialog.ShowWarning(this, message);
        }

        private void ShowConnectionLost()
        {
            if (!_sessionEnded)
            {
                _sessionEnded = true;
                MessageDialog.ShowError(this, Lang.Session_ConnectionLostMessage);
                ReturnToLogin();
            }
        }

        private void ReturnToLogin()
        {
            MainFrame.Navigate(new Login());
        }
    }
}   