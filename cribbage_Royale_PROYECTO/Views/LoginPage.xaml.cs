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
    public partial class Login : Page
    {
        private readonly PlayerSessionClient _sessionClient;

        private bool _signInInProgress;

        public Login()
        {
            InitializeComponent();
            _sessionClient = ((App)Application.Current).SessionClient;
            usernameTextBox.Focus();
        }

        private string GetUsernameOrEmail()
        {
            return usernameTextBox.Text.Trim();
        }

        private string GetPassword()
        {
            return passwordBox.Password;
        }

        private bool AreCredentialsEmpty()
        {
            bool areEmpty = false;

            if (string.IsNullOrEmpty(GetUsernameOrEmail()) || string.IsNullOrEmpty(GetPassword()))
            {
                areEmpty = true;
            }

            return areEmpty;
        }

        private void ClearPasswordField()
        {
            passwordBox.Clear();
            passwordBox.Focus();
        }

        private void ClearForm()
        {
            usernameTextBox.Clear();
            passwordBox.Clear();
            loginMessagePanel.HidePanel();
        }

        private void ShowLobby()
        {
            NavigationService.Navigate(new Lobby());
        }

        private void StartSession(SignInResult result)
        {
            loginMessagePanel.HidePanel();
            MessageDialog.ShowSuccess(Window.GetWindow(this), Lang.Login_SuccessMessage);
            CurrentSession.StartRegisteredSession(result.Session.Account.DisplayName,result.Session.EmailAddress,
            result.Session.SessionToken);
            ShowLobby();
        }

        private void ShowRejection(SessionFailure failure)
        {
            switch (failure)
            {
                case SessionFailure.AccountNotVerified:
                    loginMessagePanel.ShowError(Lang.Login_AccountNotVerifiedMessage);
                    break;
                case SessionFailure.AccountSuspended:
                    loginMessagePanel.ShowError(Lang.Login_AccountSuspendedMessage);
                    break;
                case SessionFailure.AccountDeleted:
                    loginMessagePanel.ShowError(Lang.Login_AccountDeletedMessage);
                    break;
                case SessionFailure.InvalidCredentials:
                default:
                    loginMessagePanel.ShowError(Lang.Login_InvalidCredentialsMessage);
                    ClearPasswordField();
                    break;
            }
        }

        private void ShowServiceFailure(ServiceFailureReason reason)
        {
            switch (reason)
            {
                case ServiceFailureReason.Timeout:
                    MessageDialog.ShowError(Window.GetWindow(this), Lang.Login_TimeoutMessage);
                    break;
                case ServiceFailureReason.ServerError:
                    MessageDialog.ShowError(Window.GetWindow(this), Lang.Login_ServerErrorMessage);
                    break;
                case ServiceFailureReason.ServerUnavailable:
                default:
                    MessageDialog.ShowError(Window.GetWindow(this), Lang.Login_ServerUnavailableMessage);
                    break;
            }
        }

        private void ProcessSignInResult(SignInResult result)
        {
            if (result.Succeeded)
            {
                StartSession(result);
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

        private async Task SignIn()
        {
            _signInInProgress = true;
            SignInResult result = await _sessionClient.SignInAsync(GetUsernameOrEmail(), GetPassword());
            _signInInProgress = false;

            ProcessSignInResult(result);
        }

        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_signInInProgress)
            {
                if (AreCredentialsEmpty())
                {
                    loginMessagePanel.ShowError(Lang.Validation_EmptyFields);
                }
                else
                {
                    await SignIn();
                }
            }
        }

        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Register());
        }

        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RecoverPasswordPage());
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            NavigationService.Navigate(new StartMenu());
        }
    }
}