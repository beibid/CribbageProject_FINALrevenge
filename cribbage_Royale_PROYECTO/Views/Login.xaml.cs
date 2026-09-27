using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using cribbage_Royale_PROYECTO.DataAccess;
using cribbage_Royale_PROYECTO.Models;
using cribbage_Royale_PROYECTO.Properties.Langs;
using cribbage_Royale_PROYECTO.Views.Dialogs;
using cribbage_Royale_PROYECTO.Session;

namespace cribbage_Royale_PROYECTO.Views
{
    public partial class Login : Page
    {
        private readonly UserRepository _userRepository = new UserRepository();

        public Login()
        {
            InitializeComponent();
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

        private void ShowAwaitingVerificationPage(string email)
        {
            NavigationService.Navigate(
                new AwaitingVerificationPage(email, VerificationNotice.AccountNotVerified));
        }

        private void ShowInvalidCredentials()
        {
            loginMessagePanel.ShowError(Lang.Login_InvalidCredentialsMessage);
            ClearPasswordField();
        }

        private void ShowAccountLocked(int remainingMinutes)
        {
            string message = string.Format(
                CultureInfo.CurrentCulture,
                Lang.Login_AccountLockedFormat,
                remainingMinutes);

            loginMessagePanel.ShowError(message);
        }

        private void ShowAccountSuspended()
        {
            loginMessagePanel.ShowError(Lang.Login_AccountSuspendedMessage);
        }

        private void ShowDatabaseError()
        {
            MessageDialog.ShowError(Window.GetWindow(this), Lang.Login_DatabaseErrorMessage);
        }

        private void ProcessAuthenticationResult(AuthenticationResult result)
        {
            switch (result.Outcome)
            {
                case AuthenticationOutcome.Success:
                    loginMessagePanel.HidePanel();
                    StartSession(result);
                    break;
                case AuthenticationOutcome.AccountNotVerified:
                    loginMessagePanel.HidePanel();
                    ShowAwaitingVerificationPage(result.Email);
                    break;
                case AuthenticationOutcome.AccountLocked:
                    ShowAccountLocked(result.LockoutRemainingMinutes);
                    break;
                case AuthenticationOutcome.AccountSuspended:
                    ShowAccountSuspended();
                    break;
                case AuthenticationOutcome.DatabaseError:
                    ShowDatabaseError();
                    break;
                case AuthenticationOutcome.InvalidCredentials:
                default:
                    ShowInvalidCredentials();
                    break;
            }
        }

        private void SignIn()
        {
            AuthenticationResult result = _userRepository.AuthenticateUser(
                GetUsernameOrEmail(),
                GetPassword());

            ProcessAuthenticationResult(result);
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            if (AreCredentialsEmpty())
            {
                loginMessagePanel.ShowError(Lang.Validation_EmptyFields);
            }
            else
            {
                SignIn();
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
        private void StartSession(AuthenticationResult result)
        {
            CurrentSession.StartRegisteredSession(result.Username, result.Email);
            ShowLobby();
        }
    }
}