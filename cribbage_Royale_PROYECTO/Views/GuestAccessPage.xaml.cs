using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CribbageRoyale.Contracts.FaultContracts;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.ServiceAccess;
using cribbage_Royale_PROYECTO.Session;
using cribbage_Royale_PROYECTO.Views.Dialogs;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class GuestAccessPage : Page
    {
        private readonly PlayerSessionClient _sessionClient;

        private bool _signInInProgress;

        public GuestAccessPage()
        {
            InitializeComponent();
            _sessionClient = ((App)Application.Current).SessionClient;
            displayNameTextBox.Focus();
        }

        private string GetDisplayName()
        {
            return displayNameTextBox.Text.Trim();
        }

        private bool IsDisplayNameEmpty()
        {
            return string.IsNullOrWhiteSpace(GetDisplayName());
        }

        private void StartGuestSession(GuestSignInResult result)
        {
            CurrentSession.StartGuestSession(result.Session.DisplayName, result.Session.SessionToken);
            NavigationService.Navigate(new Lobby());
        }

        private void ShowRejection(SessionFailure failure)
        {
            switch (failure)
            {
                case SessionFailure.GuestNameUnavailable:
                    MessageDialog.ShowWarning(Window.GetWindow(this), Lang.GuestAccess_NameAlreadyInUseMessage);
                    break;
                case SessionFailure.InvalidGuestName:
                default:
                    MessageDialog.ShowWarning(Window.GetWindow(this), Lang.GuestAccess_InvalidNameMessage);
                    break;
            }
        }

        private void ShowServiceFailure(ServiceFailureReason reason)
        {
            switch (reason)
            {
                case ServiceFailureReason.Timeout:
                    MessageDialog.ShowError(Window.GetWindow(this), Lang.GuestAccess_TimeoutMessage);
                    break;
                case ServiceFailureReason.ServerError:
                    MessageDialog.ShowError(Window.GetWindow(this), Lang.GuestAccess_ServerErrorMessage);
                    break;
                case ServiceFailureReason.ServerUnavailable:
                default:
                    MessageDialog.ShowError(Window.GetWindow(this), Lang.GuestAccess_ServerUnavailableMessage);
                    break;
            }
        }

        private void ProcessSignInResult(GuestSignInResult result)
        {
            if (result.Succeeded)
            {
                StartGuestSession(result);
            }
            else if (result.Reason == ServiceFailureReason.SessionRejected)
            {
                ShowRejection(result.Failure.Value);
            }
            else
            {
                ShowServiceFailure(result.Reason);
            }
        }

        private async Task SignInAsGuest()
        {
            _signInInProgress = true;
            GuestSignInResult result = await _sessionClient.SignInAsGuestAsync(GetDisplayName());
            _signInInProgress = false;

            ProcessSignInResult(result);
        }

        private async void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_signInInProgress)
            {
                if (IsDisplayNameEmpty())
                {
                    MessageDialog.ShowWarning(Window.GetWindow(this), Lang.GuestAccess_NameRequiredMessage);
                }
                else
                {
                    await SignInAsGuest();
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StartMenu());
        }
    }
}